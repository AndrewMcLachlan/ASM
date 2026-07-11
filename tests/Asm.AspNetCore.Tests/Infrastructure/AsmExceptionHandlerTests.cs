using Asm.AspNetCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore.Tests.Infrastructure;

public class AsmExceptionHandlerTests
{
    /// <summary>
    /// Given a <see cref="NotFoundException"/>
    /// When the handler runs
    /// Then a 404 Not Found problem-detail is written.
    /// </summary>
    [Fact]
    public async Task NotFoundExceptionMapsToNotFound()
    {
        var (handled, context, statusCode) = await HandleAsync(new NotFoundException("missing"));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
        Assert.Equal(StatusCodes.Status404NotFound, context!.ProblemDetails.Status);
        Assert.Equal("Not found", context.ProblemDetails.Title);
    }

    /// <summary>
    /// Given an <see cref="ExistsException"/>
    /// When the handler runs
    /// Then a 409 Conflict problem-detail with the exists type is written.
    /// </summary>
    [Fact]
    public async Task ExistsExceptionMapsToConflict()
    {
        var (handled, context, statusCode) = await HandleAsync(new ExistsException("dupe"));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, statusCode);
        Assert.Equal("Already exists", context!.ProblemDetails.Title);
        Assert.Equal("http://andrewmclachlan.com/error/exists", context.ProblemDetails.Type);
    }

    /// <summary>
    /// Given a <see cref="NotAuthorisedException"/>
    /// When the handler runs
    /// Then a 403 Forbidden problem-detail is written.
    /// </summary>
    [Fact]
    public async Task NotAuthorisedExceptionMapsToForbidden()
    {
        var (handled, context, statusCode) = await HandleAsync(new NotAuthorisedException("nope"));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status403Forbidden, statusCode);
        Assert.Equal("Forbidden", context!.ProblemDetails.Title);
    }

    /// <summary>
    /// Given a FluentValidation <see cref="ValidationException"/>
    /// When the handler runs
    /// Then a 400 Bad Request problem-detail carrying the grouped errors is written.
    /// </summary>
    [Fact]
    public async Task ValidationExceptionMapsToBadRequestWithErrors()
    {
        var failures = new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Name", "Name is too short"),
        };

        var (handled, context, statusCode) = await HandleAsync(new ValidationException(failures));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
        Assert.Equal("Validation error", context!.ProblemDetails.Title);

        var errors = Assert.IsType<Dictionary<string, string[]>>(context.ProblemDetails.Extensions["errors"]);
        Assert.Equal(["Name is required", "Name is too short"], errors["Name"]);
    }

    /// <summary>
    /// Given an <see cref="InvalidOperationException"/>
    /// When the handler runs
    /// Then a 400 Bad Request problem-detail is written.
    /// </summary>
    [Fact]
    public async Task InvalidOperationExceptionMapsToBadRequest()
    {
        var (handled, context, statusCode) = await HandleAsync(new InvalidOperationException("bad state"));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
        Assert.Equal("Bad request", context!.ProblemDetails.Title);
    }

    /// <summary>
    /// Given an <see cref="AsmException"/> carrying an error id
    /// When the handler runs
    /// Then a 500 problem-detail exposing the code is written.
    /// </summary>
    [Fact]
    public async Task AsmExceptionMapsToInternalServerErrorWithCode()
    {
        var (handled, context, statusCode) = await HandleAsync(new TestAsmException("boom", 42));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
        Assert.Equal("Unexpected error occurred", context!.ProblemDetails.Title);
        Assert.Equal(42, context.ProblemDetails.Extensions["Code"]);
    }

    /// <summary>
    /// Given an exception the handler does not recognise
    /// When the handler runs
    /// Then it reports the exception as unhandled and writes nothing.
    /// </summary>
    [Fact]
    public async Task UnmappedExceptionIsNotHandled()
    {
        var (handled, context, _) = await HandleAsync(new TimeoutException("slow"));

        Assert.False(handled);
        Assert.Null(context);
    }

    private static async Task<(bool Handled, ProblemDetailsContext Context, int StatusCode)> HandleAsync(Exception exception)
    {
        var service = new CapturingProblemDetailsService();
        var handler = new AsmExceptionHandler(service);
        var httpContext = new DefaultHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        return (handled, service.Captured, httpContext.Response.StatusCode);
    }

    private sealed class TestAsmException(string message, int errorId) : AsmException(message, errorId);

    private sealed class CapturingProblemDetailsService : IProblemDetailsService
    {
        public ProblemDetailsContext Captured { get; private set; }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            Captured = context;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            Captured = context;
            return ValueTask.FromResult(true);
        }
    }
}
