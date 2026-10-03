using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asm.AspNetCore;

/// <summary>
/// An <see cref="IExceptionHandler"/> that translates ASM exception types into RFC 9457
/// problem-detail responses, written through the registered <see cref="IProblemDetailsService"/>.
/// </summary>
/// <remarks>
/// Register with <c>services.AddAsmExceptionHandler()</c> and wire into the pipeline with
/// <c>app.UseExceptionHandler()</c> (or the equivalent <c>app.UseStandardExceptionHandler()</c>).
/// Exceptions this handler does not recognise are left unhandled so that other
/// <see cref="IExceptionHandler"/> implementations, or the framework's default problem-details
/// response, can deal with them.
/// <para>
/// A recognised exception is always reported as handled, which suppresses the middleware's own
/// diagnostics, so the handler logs the ones that point at a fault: server errors at
/// <see cref="LogLevel.Error"/>, malformed requests at <see cref="LogLevel.Warning"/>. Expected
/// outcomes — not found, already exists, validation and authorisation failures — are not logged.
/// </para>
/// </remarks>
/// <param name="problemDetailsService">The service used to write the problem-detail response.</param>
/// <param name="logger">The logger the handled exception is recorded on.</param>
public sealed class AsmExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<AsmExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var problemDetails = Map(exception);

        if (problemDetails is null)
        {
            // Not one of ours: leave it for other handlers or the framework default.
            return false;
        }

        var status = problemDetails.Status!.Value;

        if (LogLevelFor(exception, status) is { } level)
        {
            logger.Log(level, exception, "{Method} {Path} answered {StatusCode}", LogSanitiser.Sanitise(httpContext.Request.Method), LogSanitiser.Sanitise(httpContext.Request.Path.Value), status);
        }

        httpContext.Response.StatusCode = status;

        // Must report handled even when nothing is written: false hands the exception back to the middleware, which logs it again at Error.
        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });

        return true;
    }

    private static LogLevel? LogLevelFor(Exception exception, int status) => exception switch
    {
        _ when status >= StatusCodes.Status500InternalServerError => LogLevel.Error,
        ValidationException or BadHttpRequestException or InvalidOperationException => LogLevel.Warning,
        ExistsException => LogLevel.Warning,
        _ => null,
    };

    private static ProblemDetails? Map(Exception exception) => exception switch
    {
        ValidationException validationException => new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = validationException.Message,
            Type = "about:blank",
            Extensions =
            {
                // Emitted as a top-level "errors" member (RFC 9457 extension), which serialises
                // correctly through the base ProblemDetails writer.
                ["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
            },
        },
        BadHttpRequestException or InvalidOperationException => new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad request",
            Detail = exception.Message,
            Type = "about:blank",
        },
        NotFoundException => new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not found",
            Detail = exception.Message,
            Type = "about:blank",
        },
        ExistsException => new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Already exists",
            Detail = exception.Message,
            Type = "http://andrewmclachlan.com/error/exists",
        },
        NotAuthorisedException => new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = exception.Message,
            Type = "about:blank",
        },
        AsmException asmException => new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected error occurred",
            Detail = exception.Message,
            Type = "about:blank",
            Extensions = { ["Code"] = asmException.ErrorId },
        },
        _ => null,
    };
}
