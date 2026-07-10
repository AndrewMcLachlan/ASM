namespace Asm;

/// <summary>
/// Thrown when a <see cref="Bounded"/> loop reaches its iteration limit while more work remains.
/// </summary>
/// <remarks>
/// Derives from <see cref="InvalidOperationException"/> so existing handlers continue to catch it.
/// </remarks>
public sealed class BoundExceededException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundExceededException"/> class.
    /// </summary>
    /// <param name="maxIterations">The iteration limit that was reached.</param>
    public BoundExceededException(int maxIterations)
        : base($"Bounded loop reached its limit of {maxIterations} iteration(s) before completing.")
        => MaxIterations = maxIterations;

    /// <summary>
    /// Gets the iteration limit that was reached.
    /// </summary>
    public int MaxIterations { get; }
}
