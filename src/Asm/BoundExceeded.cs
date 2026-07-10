namespace Asm;

/// <summary>
/// Controls what happens when a <see cref="Bounded"/> loop reaches its iteration limit
/// without its body signalling completion.
/// </summary>
public enum BoundExceeded
{
    /// <summary>
    /// Throw a <see cref="BoundExceededException"/>. Use when reaching the limit indicates a bug
    /// (for example a loop that should always converge).
    /// </summary>
    Throw,

    /// <summary>
    /// Stop iterating silently, as a plain capped loop would.
    /// </summary>
    Stop,
}
