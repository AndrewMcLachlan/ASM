using System;
using System.Threading;
using System.Threading.Tasks;
using Asm.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Hosting.Tests;

public class BackgroundWorkQueueTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task DequeueReturnsQueuedItem()
    {
        var queue = new BackgroundWorkQueue<int>();
        queue.Queue(42);

        var item = await queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(42, item);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DequeueReturnsItemsInFifoOrder()
    {
        var queue = new BackgroundWorkQueue<int>();
        queue.Queue(1);
        queue.Queue(2);
        queue.Queue(3);

        Assert.Equal(1, await queue.DequeueAsync(CancellationToken.None));
        Assert.Equal(2, await queue.DequeueAsync(CancellationToken.None));
        Assert.Equal(3, await queue.DequeueAsync(CancellationToken.None));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DequeueWaitsUntilAnItemIsQueued()
    {
        var queue = new BackgroundWorkQueue<int>();

        var dequeueTask = queue.DequeueAsync(CancellationToken.None);
        Assert.False(dequeueTask.IsCompleted);

        queue.Queue(99);

        var item = await dequeueTask.WaitAsync(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        Assert.Equal(99, item);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void QueueAfterDisposeThrows()
    {
        var queue = new BackgroundWorkQueue<int>();
        queue.Dispose();

        Assert.Throws<ObjectDisposedException>(() => queue.Queue(1));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddBackgroundWorkQueueRegistersSingleton()
    {
        var services = new ServiceCollection();
        services.AddBackgroundWorkQueue<int>();
        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IBackgroundWorkQueue<int>>();
        var second = provider.GetRequiredService<IBackgroundWorkQueue<int>>();

        Assert.Same(first, second);
        Assert.IsType<BackgroundWorkQueue<int>>(first);
    }
}
