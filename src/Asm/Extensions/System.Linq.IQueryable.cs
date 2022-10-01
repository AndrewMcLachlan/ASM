namespace System.Linq;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/> interface.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Skip / takes a page of data.
    /// </summary>
    /// <typeparam name="TSource"> The type of the data in the data source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="pageSize">The number of data items per page.</param>
    /// <param name="pageNumber">The page number to retrieve. Pages start from 1.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains the specified number of elements from
    /// the page.
    /// </returns>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageSize, int pageNumber)
    {
        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
