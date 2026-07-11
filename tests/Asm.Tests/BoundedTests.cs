using Asm;

namespace Asm.Tests;

public class BoundedTests
{
    // ── Iterator form ─────────────────────────────────────────────────────────

    /// <summary>
    /// Given a Bounded.While iterator with a high limit that is broken out of early.
    /// When the loop breaks after three iterations.
    /// Then it runs exactly three times and does not throw.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IteratorBreaksEarlyRunsUntilBreakAndDoesNotThrow()
    {
        int runs = 0;
        foreach (var _ in Bounded.While(100))
        {
            runs++;
            if (runs == 3) break;
        }

        Assert.Equal(3, runs);
    }

    /// <summary>
    /// Given a Bounded.While iterator with a limit of five that is never broken out of.
    /// When the loop runs to the limit.
    /// Then it runs five times and throws a BoundExceededException reporting MaxIterations of five.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IteratorNeverBreaksThrowsAfterMaxIterations()
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

    /// <summary>
    /// Given a Bounded.While iterator with a limit of five and BoundExceeded.Stop.
    /// When the loop is never broken out of and reaches the limit.
    /// Then it runs five times and does not throw.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IteratorNeverBreaksStopDoesNotThrow()
    {
        int runs = 0;
        foreach (var _ in Bounded.While(5, BoundExceeded.Stop))
        {
            runs++;
        }

        Assert.Equal(5, runs);
    }

    /// <summary>
    /// Given a Bounded.While iterator over four items with BoundExceeded.Stop.
    /// When it is enumerated.
    /// Then it yields the sequential indices 0, 1, 2, 3.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IteratorYieldsSequentialIndices()
    {
        Assert.Equal([0, 1, 2, 3], Bounded.While(4, BoundExceeded.Stop));
    }

    /// <summary>
    /// Given a Bounded.While iterator constructed with a negative maximum.
    /// When enumeration is started.
    /// Then an ArgumentOutOfRangeException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IteratorNegativeMaxThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bounded.While(-1).GetEnumerator().MoveNext());
    }

    // ── Condition form (sync) ─────────────────────────────────────────────────

    /// <summary>
    /// Given a Bounded.While condition form whose condition becomes false after three iterations.
    /// When it is run with a high maximum.
    /// Then it returns three and the body ran three times.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConditionStopsWhenConditionBecomesFalseReturnsIterationCount()
    {
        int counter = 0;
        int ran = Bounded.While(() => counter < 3, () => counter++, maxIterations: 100);

        Assert.Equal(3, ran);
        Assert.Equal(3, counter);
    }

    /// <summary>
    /// Given a Bounded.While condition form whose condition is always true with a maximum of ten.
    /// When it is run.
    /// Then it throws a BoundExceededException after ten iterations, reporting MaxIterations of ten.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConditionAlwaysTrueThrowsAfterMaxIterations()
    {
        int ran = 0;

        var exception = Assert.Throws<BoundExceededException>(
            () => Bounded.While(() => true, () => ran++, maxIterations: 10));

        Assert.Equal(10, ran);
        Assert.Equal(10, exception.MaxIterations);
    }

    /// <summary>
    /// Given a Bounded.While condition form whose condition is always true with a maximum of ten and BoundExceeded.Stop.
    /// When it is run.
    /// Then it returns ten and the body ran ten times without throwing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConditionAlwaysTrueStopReturnsMax()
    {
        int ran = 0;
        int result = Bounded.While(() => true, () => ran++, maxIterations: 10, BoundExceeded.Stop);

        Assert.Equal(10, result);
        Assert.Equal(10, ran);
    }

    /// <summary>
    /// Given a Bounded.While condition form called with a null condition or a null body.
    /// When either invocation runs.
    /// Then an ArgumentNullException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConditionNullArgumentsThrow()
    {
        Assert.Throws<ArgumentNullException>(() => Bounded.While(null!, () => { }, 10));
        Assert.Throws<ArgumentNullException>(() => Bounded.While(() => true, null!, 10));
    }

    // ── Condition form (async) ────────────────────────────────────────────────

    /// <summary>
    /// Given a Bounded.WhileAsync condition form whose condition becomes false after three iterations.
    /// When it is awaited with a high maximum.
    /// Then it returns three.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsyncStopsWhenConditionBecomesFalseReturnsIterationCount()
    {
        int counter = 0;
        int ran = await Bounded.WhileAsync(
            () => counter < 3,
            _ => { counter++; return Task.CompletedTask; },
            maxIterations: 100,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, ran);
    }

    /// <summary>
    /// Given a Bounded.WhileAsync condition form whose condition is always true with a maximum of ten.
    /// When it is awaited.
    /// Then it throws a BoundExceededException after reaching the maximum.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsyncAlwaysTrueThrowsAfterMaxIterations()
    {
        await Assert.ThrowsAsync<BoundExceededException>(
            () => Bounded.WhileAsync(() => true, _ => Task.CompletedTask, maxIterations: 10));
    }

    /// <summary>
    /// Given a Bounded.WhileAsync condition form passed an already-cancelled cancellation token.
    /// When it is awaited.
    /// Then an OperationCanceledException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ConditionAsyncCancellationThrows()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => Bounded.WhileAsync(() => true, _ => Task.CompletedTask, maxIterations: 10, cancellationToken: cts.Token));
    }
}
