using Asm;

namespace Asm.Tests;

public class BoundedTests
{
    // ── Iterator form ─────────────────────────────────────────────────────────

    [Fact]
    [Trait("Category", "Unit")]
    public void Iterator_BreaksEarly_RunsUntilBreakAndDoesNotThrow()
    {
        int runs = 0;
        foreach (var _ in Bounded.While(100))
        {
            runs++;
            if (runs == 3) break;
        }

        Assert.Equal(3, runs);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Iterator_NeverBreaks_ThrowsAfterMaxIterations()
    {
        int runs = 0;

        var exception = Assert.Throws<BoundExceededException>(() =>
        {
            foreach (var _ in Bounded.While(5))
            {
                runs++;
            }
        });

        Assert.Equal(5, runs);
        Assert.Equal(5, exception.MaxIterations);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Iterator_NeverBreaks_Stop_DoesNotThrow()
    {
        int runs = 0;
        foreach (var _ in Bounded.While(5, BoundExceeded.Stop))
        {
            runs++;
        }

        Assert.Equal(5, runs);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Iterator_YieldsSequentialIndices()
    {
        Assert.Equal([0, 1, 2, 3], Bounded.While(4, BoundExceeded.Stop));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Iterator_NegativeMax_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounded.While(-1).GetEnumerator().MoveNext());
    }

    // ── Condition form (sync) ─────────────────────────────────────────────────

    [Fact]
    [Trait("Category", "Unit")]
    public void Condition_StopsWhenConditionBecomesFalse_ReturnsIterationCount()
    {
        int counter = 0;
        int ran = Bounded.While(() => counter < 3, () => counter++, maxIterations: 100);

        Assert.Equal(3, ran);
        Assert.Equal(3, counter);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Condition_AlwaysTrue_ThrowsAfterMaxIterations()
    {
        int ran = 0;

        var exception = Assert.Throws<BoundExceededException>(
            () => Bounded.While(() => true, () => ran++, maxIterations: 10));

        Assert.Equal(10, ran);
        Assert.Equal(10, exception.MaxIterations);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Condition_AlwaysTrue_Stop_ReturnsMax()
    {
        int ran = 0;
        int result = Bounded.While(() => true, () => ran++, maxIterations: 10, BoundExceeded.Stop);

        Assert.Equal(10, result);
        Assert.Equal(10, ran);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Condition_NullArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => Bounded.While(null!, () => { }, 10));
        Assert.Throws<ArgumentNullException>(() => Bounded.While(() => true, null!, 10));
    }

    // ── Condition form (async) ────────────────────────────────────────────────

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsync_StopsWhenConditionBecomesFalse_ReturnsIterationCount()
    {
        int counter = 0;
        int ran = await Bounded.WhileAsync(
            () => counter < 3,
            _ => { counter++; return Task.CompletedTask; },
            maxIterations: 100,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, ran);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsync_AlwaysTrue_ThrowsAfterMaxIterations()
    {
        await Assert.ThrowsAsync<BoundExceededException>(
            () => Bounded.WhileAsync(() => true, _ => Task.CompletedTask, maxIterations: 10));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsync_Cancellation_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => Bounded.WhileAsync(() => true, _ => Task.CompletedTask, maxIterations: 10, cancellationToken: cts.Token));
    }
}
