using Microsoft.Extensions.Logging;

namespace Asm.Logging;

/// <summary>
/// A logger that owns its underlying logging providers.
/// </summary>
/// <remarks>
/// Dispose this logger to flush buffered log events (e.g. those queued for Seq) and
/// release the underlying providers. Created via <see cref="BootstrapLoggerFactory.Create"/>.
/// </remarks>
public sealed class BootstrapLogger : ILogger, IDisposable
{
    private readonly ILogger _inner;
    private readonly ILoggerFactory _factory;
    private readonly IDisposable? _appScope;

    internal BootstrapLogger(ILogger inner, ILoggerFactory factory, IDisposable? appScope)
    {
        _inner = inner;
        _factory = factory;
        _appScope = appScope;
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _inner.BeginScope(state);

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel);

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        _inner.Log(logLevel, eventId, state, exception, formatter);

    /// <summary>
    /// Flushes any buffered log events and releases the underlying logging providers.
    /// </summary>
    public void Dispose()
    {
        _appScope?.Dispose();
        _factory.Dispose();
    }
}
