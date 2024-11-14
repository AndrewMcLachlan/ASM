namespace Asm;

/// <summary>
/// Represents a pageable object.
/// </summary>
public interface IPageable
{
    /// <summary>
    /// Gets the page number to retrieve. Pages start from 1.
    /// </summary>
    int PageNumber { get; init; }

    /// <summary>
    /// Gets the number of data items per page.
    /// </summary>
    int PageSize { get; init; }
}
