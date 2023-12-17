namespace Asm;

/// <summary>
/// A paged result including the total number of items.
/// </summary>
/// <typeparam name="T">The type of the paged item.</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// Gets the paged items.
    /// </summary>
    public required IEnumerable<T> Results { get; init; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public required int Total { get; init; }
}
