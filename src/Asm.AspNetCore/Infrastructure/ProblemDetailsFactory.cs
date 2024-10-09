using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore;

/// <inheritdoc/>
/// <summary>
/// Initializes a new instance of the <see cref="ProblemDetailsFactory"/> class.
/// </summary>
/// <param name="hostEnvironment"></param>
/// <exception cref="ArgumentNullException"></exception>
public class ProblemDetailsFactory(IHostEnvironment hostEnvironment) : Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory
{
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

    private static readonly Dictionary<Type, Func<HttpContext, IExceptionHandlerFeature, ProblemDetails>> HandlersInternal = [];

    /// <summary>
    /// Gets a dictionary of registered handlers.
    /// </summary>
    public static IReadOnlyDictionary<Type, Func<HttpContext, IExceptionHandlerFeature, ProblemDetails>> Handlers { get => HandlersInternal; }

    /// <inheritdoc/>
    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        var errorContext = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (errorContext?.Error == null)
        {
            return new ProblemDetails
            {
                Detail = detail,
                Instance = instance,
                Status = statusCode ?? StatusCodes.Status500InternalServerError,
                Title = title,
                Type = type,
            };
        }

        if (Handlers.ContainsKey(errorContext.Error.GetType()))
        {
            var customProblemDetails = Handlers[errorContext.Error.GetType()](httpContext, errorContext);
            customProblemDetails.Status ??= 500;
            return customProblemDetails;
        }

        ProblemDetails problemDetails = new();

        switch (errorContext.Error)
        {
            case ValidationException validationException:
                problemDetails = new HttpValidationProblemDetails(validationException.Errors.ToDictionary<ValidationFailure, string, string[]>(e => e.ErrorMessage, e => [e.PropertyName]))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation error",
                    Detail = validationException.Message,
                    Type = "about:blank",
                };
                break;
            case BadHttpRequestException _:
            case InvalidOperationException _:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad request";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "about:blank";
                break;
            case NotFoundException _:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not found";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "about:blank";
                break;
            case ExistsException _:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Title = "Already exists";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "http://andrewmclachlan.com/error/exists";
                break;
            case NotAuthorisedException _:
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Type = "about:blank";
                break;
            case AsmException asmException:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Unexpected error occurred";
                problemDetails.Detail = errorContext.Error.Message;
                problemDetails.Extensions.Add("Code", asmException.ErrorId);
                problemDetails.Type = "about:blank";
                break;
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Unexpected error occurred";
                problemDetails.Detail = !_hostEnvironment.IsProduction() ? errorContext.Error.ToString() : null;
                problemDetails.Type = "about:blank";
                break;
        }

        problemDetails.Status = httpContext.Response.StatusCode;

        AddExtensions(httpContext, problemDetails);

        return problemDetails;
    }

    /// <inheritdoc/>
    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        statusCode ??= StatusCodes.Status400BadRequest;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        if (title != null)
        {
            // For validation problem details, don't overwrite the default title with null.
            problemDetails.Title = title;
        }

        AddExtensions(httpContext, problemDetails);

        return problemDetails;
    }

    /// <summary>
    /// Add a custom handler for a specific exception type.
    /// </summary>
    /// <typeparam name="T">The type of exception to handle.</typeparam>
    /// <param name="handler">A delegate that returns a problem details object.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddHandler<T>(Func<HttpContext, IExceptionHandlerFeature, ProblemDetails> handler) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(handler);

        HandlersInternal.Add(typeof(T), handler);
    }

    private static void AddExtensions(HttpContext httpContext, ProblemDetails problemDetails)
    {
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }
    }
}
