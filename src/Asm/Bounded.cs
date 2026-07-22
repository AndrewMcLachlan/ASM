namespace Asm;

/// <summary>
/// Helpers for running otherwise-unbounded ("<c>while (true)</c>") loops with a hard iteration
/// limit, so a loop that fails to converge either throws or stops rather than hanging.
/// </summary>
public static class Bounded
{
    /// <summary>
    /// Produces a bounded sequence of iteration indices (<c>0 .. maxIterations - 1</c>) for use
    /// with <c>foreach</c>. Exit the loop early with <c>break</c> for the normal case; if the body
    /// runs <paramref name="maxIterations"/> times without breaking, the loop either throws a
    /// <see cref="BoundExceededException"/> or stops, per <paramref name="onExceeded"/>.
    /// </summary>
    /// <remarks>
    /// The sequence itself is synchronous, so the <c>foreach</c> body may freely <c>await</c>:
    /// <code>
    /// foreach (var _ in Bounded.While(100))
    /// {
    ///     if (done) break;
    ///     await WorkAsync();
    /// }
    /// </code>
    /// </remarks>
    /// <param name="maxIterations">The maximum number of iterations to allow.</param>
    /// <param name="onExceeded">What to do when the limit is reached without the body breaking.</param>
    /// <returns>A sequence of iteration indices.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxIterations"/> is negative.</exception>
    /// <exception cref="BoundExceededException">
    /// Thrown when the limit is reached and <paramref name="onExceeded"/> is <see cref="BoundExceeded.Throw"/>.
    /// </exception>
    public static IEnumerable<int> While(int maxIterations, BoundExceeded onExceeded = BoundExceeded.Throw)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxIterations);

        for (int i = 0; i < maxIterations; i++)
        {
            yield return i;
        }

        if (onExceeded == BoundExceeded.Throw)
        {
            throw new BoundExceededException(maxIterations);
        }
    }

    /// <summary>
    /// Runs <paramref name="body"/> while <paramref name="condition"/> is <see langword="true"/>,
    /// up to <paramref name="maxIterations"/> times. If the condition is still <see langword="true"/>
    /// after the limit is reached, either throws a <see cref="BoundExceededException"/> or stops,
    /// per <paramref name="onExceeded"/>.
    /// </summary>
    /// <param name="condition">Evaluated before each iteration; the loop ends when it returns <see langword="false"/>.</param>
    /// <param name="body">The loop body.</param>
    /// <param name="maxIterations">The maximum number of iterations to allow.</param>
    /// <param name="onExceeded">What to do when the limit is reached with the condition still true.</param>
    /// <returns>The number of iterations that ran.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> or <paramref name="body"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxIterations"/> is negative.</exception>
    /// <exception cref="BoundExceededException">
    /// Thrown when the limit is reached with the condition still true and <paramref name="onExceeded"/>
    /// is <see cref="BoundExceeded.Throw"/>.
    /// </exception>
    public static int While(Func<bool> condition, Action body, int maxIterations, BoundExceeded onExceeded = BoundExceeded.Throw)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(body);
        ArgumentOutOfRangeException.ThrowIfNegative(maxIterations);

        int iterations = 0;
        for (; iterations < maxIterations; iterations++)
        {
            if (!condition())
            {
                return iterations;
            }

            body();
        }

        if (condition() && onExceeded == BoundExceeded.Throw)
        {
            throw new BoundExceededException(maxIterations);
        }

        return iterations;
    }

    /// <summary>
    /// Asynchronous counterpart to <see cref="While(Func{Boolean}, Action, Int32, BoundExceeded)"/>.
    /// </summary>
    /// <param name="condition">Evaluated before each iteration; the loop ends when it returns <see langword="false"/>.</param>
    /// <param name="body">The loop body, passed the <paramref name="cancellationToken"/>.</param>
    /// <param name="maxIterations">The maximum number of iterations to allow.</param>
    /// <param name="onExceeded">What to do when the limit is reached with the condition still true.</param>
    /// <param name="cancellationToken">A token to cancel the loop.</param>
    /// <returns>The number of iterations that ran.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> or <paramref name="body"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxIterations"/> is negative.</exception>
    /// <exception cref="BoundExceededException">
    /// Thrown when the limit is reached with the condition still true and <paramref name="onExceeded"/>
    /// is <see cref="BoundExceeded.Throw"/>.
    /// </exception>
    public static async Task<int> WhileAsync(Func<bool> condition, Func<CancellationToken, Task> body, int maxIterations, BoundExceeded onExceeded = BoundExceeded.Throw, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(body);
        ArgumentOutOfRangeException.ThrowIfNegative(maxIterations);

        int iterations = 0;
        for (; iterations < maxIterations; iterations++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!condition())
            {
                return iterations;
            }

            await body(cancellationToken).ConfigureAwait(false);
        }

        if (condition() && onExceeded == BoundExceeded.Throw)
        {
            throw new BoundExceededException(maxIterations);
        }

        return iterations;
    }
}
