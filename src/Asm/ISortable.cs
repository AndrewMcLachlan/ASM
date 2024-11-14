namespace Asm;

/// <summary>
/// Represents a sortable object.
/// </summary>
public interface ISortable
{
    /// <summary>
    /// Gets the field to sort by.
    /// </summary>
    string? SortField { get; init; }

    /// <summary>
    /// Gets the sort direction.
    /// </summary>
    SortDirection SortDirection { get; init; }
}
