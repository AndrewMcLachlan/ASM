using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Asm.Tests.Extensions;

public class EnumerableAsyncExtensionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task SelectAsyncProjectsInSourceOrder()
    {
        var result = await new[] { 1, 2, 3 }.SelectAsync(async x => { await Task.Yield(); return x * 2; });

        Assert.Equal(new[] { 2, 4, 6 }, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SelectAsyncEmptySourceReturnsEmpty()
    {
        var result = await Array.Empty<int>().SelectAsync(x => Task.FromResult(x));

        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SelectAsyncAwaitsSequentially()
    {
        int concurrent = 0;
        int maxConcurrent = 0;

        await Enumerable.Range(0, 5).SelectAsync(async x =>
        {
            var current = Interlocked.Increment(ref concurrent);
            maxConcurrent = Math.Max(maxConcurrent, current);
            await Task.Delay(10);
            Interlocked.Decrement(ref concurrent);
            return x;
        });

        Assert.Equal(1, maxConcurrent);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SelectAsyncNullSourceThrows()
    {
        IEnumerable<int> source = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(() => source.SelectAsync(x => Task.FromResult(x)));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SelectAsyncNullSelectorThrows()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new[] { 1 }.SelectAsync<int, int>(null!));
    }
}
