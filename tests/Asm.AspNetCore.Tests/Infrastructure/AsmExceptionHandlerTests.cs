using Asm.AspNetCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asm.AspNetCore.Tests.Infrastructure;

[Trait("Category", "Unit")]

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

    /// <summary>
    /// Given a mapped exception and a client that accepts no problem-details format
    /// When the handler runs
    /// Then it still reports the exception handled with the mapped status.
    /// </summary>
    [Fact]
    public async Task UnwritableProblemDetailsIsStillHandled()
    {
        var (handled, _, statusCode) = await HandleAsync(new NotFoundException("missing"), canWrite: false);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
    }

    /// <summary>
    /// Given a malformed request
    /// When the handler runs
    /// Then it logs the exception once, at warning.
    /// </summary>
    [Fact]
    public async Task BadRequestIsLoggedAsWarning()
    {
        var exception = new BadHttpRequestException("Failed to bind parameter");
        var logger = new CapturingLogger();

        await HandleAsync(exception, logger: logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Same(exception, entry.Exception);
    }

    /// <summary>
    /// Given an exception that maps to a server error
    /// When the handler runs
    /// Then it logs the exception once, at error.
    /// </summary>
    [Fact]
    public async Task ServerErrorIsLoggedAsError()
    {
        var exception = new TestAsmException("boom", 42);
        var logger = new CapturingLogger();

        await HandleAsync(exception, logger: logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
    }

    /// <summary>
    /// Given a logged exception on a request whose path carries a line break
    /// When the handler runs
    /// Then the logged path has the line break stripped, so it cannot forge a log entry.
    /// </summary>
    [Fact]
    public async Task RequestPathIsSanitisedInTheLog()
    {
        var logger = new CapturingLogger();

        await HandleAsync(new BadHttpRequestException("Failed to bind parameter"), logger: logger, path: $"/accounts{Environment.NewLine}forged");

        var entry = Assert.Single(logger.Entries);
        Assert.Contains("/accountsforged", entry.Message);
        Assert.DoesNotContain(Environment.NewLine, entry.Message);
    }

    public static TheoryData<Exception> ExpectedOutcomes() =>
    [
        new NotFoundException("missing"),
        new NotAuthorisedException("nope"),
    ];

    /// <summary>
    /// Given an exception that maps to an expected outcome rather than a fault
    /// When the handler runs
    /// Then it writes problem details and logs nothing.
    /// </summary>
    [Theory]
    [MemberData(nameof(ExpectedOutcomes))]
    public async Task ExpectedOutcomeIsNotLogged(Exception exception)
    {
        var logger = new CapturingLogger();

        var (handled, _, _) = await HandleAsync(exception, logger: logger);

        Assert.True(handled);
        Assert.Empty(logger.Entries);
    }

    /// <summary>
    /// Given an exception the handler does not recognise
    /// When the handler runs
    /// Then it logs nothing, leaving the exception to the middleware's diagnostics.
    /// </summary>
    [Fact]
    public async Task UnmappedExceptionIsNotLogged()
    {
        var logger = new CapturingLogger();

        await HandleAsync(new TimeoutException("slow"), logger: logger);

        Assert.Empty(logger.Entries);
    }

    private static async Task<(bool Handled, ProblemDetailsContext? Context, int StatusCode)> HandleAsync(Exception exception, bool canWrite = true, CapturingLogger? logger = null, string path = "/")
    {
        var service = new CapturingProblemDetailsService(canWrite);
        var handler = new AsmExceptionHandler(service, logger ?? new CapturingLogger());
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = path;

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        return (handled, service.Captured, httpContext.Response.StatusCode);
    }

    private sealed class TestAsmException(string message, int errorId) : AsmException(message, errorId);

    private sealed class CapturingProblemDetailsService(bool canWrite) : IProblemDetailsService
    {
        public ProblemDetailsContext? Captured { get; private set; }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            Captured = context;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            Captured = context;
            return ValueTask.FromResult(canWrite);
        }
    }

    private sealed class CapturingLogger : ILogger<AsmExceptionHandler>
    {
        public List<(LogLevel Level, Exception? Exception, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, exception, formatter(state, exception)));
    }
}
