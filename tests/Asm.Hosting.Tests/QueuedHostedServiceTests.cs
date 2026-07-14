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
    }

    private sealed class ScopedMarker
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
