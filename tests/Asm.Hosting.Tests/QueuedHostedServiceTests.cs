using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asm.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Asm.Hosting.Tests;

public class QueuedHostedServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ProcessesQueuedItemsInOrder()
    {
        var queue = new BackgroundWorkQueue<int>();
        using var provider = BuildProvider();
        var service = new RecordingHostedService(queue, provider.GetRequiredService<IServiceScopeFactory>(), NullLoggerFactory.Instance);

        await service.StartAsync(CancellationToken.None);
        queue.Queue(1);
        queue.Queue(2);
        queue.Queue(3);
        await WaitForAsync(() => service.Processed.Count == 3);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal([1, 2, 3], service.Processed.ToArray());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ContinuesAfterAProcessingError()
    {
        var queue = new BackgroundWorkQueue<int>();
        using var provider = BuildProvider();
        var service = new RecordingHostedService(queue, provider.GetRequiredService<IServiceScopeFactory>(), NullLoggerFactory.Instance);

        await service.StartAsync(CancellationToken.None);
        queue.Queue(-1); // throws inside ProcessAsync
        queue.Queue(7);
        await WaitForAsync(() => service.Processed.Contains(7));
        await service.StopAsync(CancellationToken.None);

        Assert.Contains(7, service.Processed);
        Assert.DoesNotContain(-1, service.Processed);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LogsTheFailingWorkItemOnError()
    {
        var queue = new BackgroundWorkQueue<int>();
        using var provider = BuildProvider();
        var loggerFactory = new CapturingLoggerFactory();
        var service = new RecordingHostedService(queue, provider.GetRequiredService<IServiceScopeFactory>(), loggerFactory);

        await service.StartAsync(CancellationToken.None);
        queue.Queue(-1); // throws inside ProcessAsync
        await WaitForAsync(() => loggerFactory.Messages.Any(m => m.Contains("Error processing work item")));
        await service.StopAsync(CancellationToken.None);

        Assert.Contains(loggerFactory.Messages, m => m.Contains("Error processing work item -1"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DoesNotLogWorkItemContentByDefault()
    {
        var queue = new BackgroundWorkQueue<int>();
        using var provider = BuildProvider();
        var loggerFactory = new CapturingLoggerFactory();
        var service = new AlwaysThrowingHostedService(queue, provider.GetRequiredService<IServiceScopeFactory>(), loggerFactory);

        await service.StartAsync(CancellationToken.None);
        queue.Queue(-1);
        await WaitForAsync(() => loggerFactory.Messages.Any(m => m.Contains("Error processing")));
        await service.StopAsync(CancellationToken.None);

        // The default descriptor is null, so the item's value must not appear in the log.
        Assert.Contains(loggerFactory.Messages, m => m.Contains("Error processing a work item"));
        Assert.DoesNotContain(loggerFactory.Messages, m => m.Contains("-1"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatesAFreshScopePerItem()
    {
        var queue = new BackgroundWorkQueue<int>();
        using var provider = BuildProvider();
        var service = new RecordingHostedService(queue, provider.GetRequiredService<IServiceScopeFactory>(), NullLoggerFactory.Instance);

        await service.StartAsync(CancellationToken.None);
        queue.Queue(1);
        queue.Queue(2);
        await WaitForAsync(() => service.ScopeIds.Count == 2);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal(2, service.ScopeIds.Distinct().Count());
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<ScopedMarker>();
        return services.BuildServiceProvider();
    }

    private static async Task WaitForAsync(Func<bool> condition, int timeoutMs = 2000)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition() && stopwatch.ElapsedMilliseconds < timeoutMs)
        {
            await Task.Delay(20);
        }

        Assert.True(condition(), "Condition was not met within the timeout.");
    }

    private sealed class RecordingHostedService(IBackgroundWorkQueue<int> queue, IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        : QueuedHostedService<int>(queue, scopeFactory, loggerFactory)
    {
        public ConcurrentQueue<int> Processed { get; } = new();

        public ConcurrentQueue<Guid> ScopeIds { get; } = new();

        protected override ValueTask ProcessAsync(IServiceProvider services, int workItem, CancellationToken cancellationToken)
        {
            if (workItem < 0)
            {
                throw new InvalidOperationException("Simulated processing failure.");
            }

            ScopeIds.Enqueue(services.GetRequiredService<ScopedMarker>().Id);
            Processed.Enqueue(workItem);
            return ValueTask.CompletedTask;
        }

        // An int is not sensitive, so opt in to logging it.
        protected override object DescribeWorkItem(int workItem) => workItem;
    }

    // Throws for every item and does not override DescribeWorkItem, exercising the safe default.
    private sealed class AlwaysThrowingHostedService(IBackgroundWorkQueue<int> queue, IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        : QueuedHostedService<int>(queue, scopeFactory, loggerFactory)
    {
        protected override ValueTask ProcessAsync(IServiceProvider services, int workItem, CancellationToken cancellationToken)
            => throw new InvalidOperationException("Simulated processing failure.");
    }

    private sealed class ScopedMarker
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    private sealed class CapturingLoggerFactory : ILoggerFactory
    {
        public ConcurrentQueue<string> Messages { get; } = new();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(Messages);

        public void AddProvider(ILoggerProvider provider) { }

        public void Dispose() { }
    }

    private sealed class CapturingLogger(ConcurrentQueue<string> messages) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => messages.Enqueue(formatter(state, exception));
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();

        public void Dispose() { }
    }
}
