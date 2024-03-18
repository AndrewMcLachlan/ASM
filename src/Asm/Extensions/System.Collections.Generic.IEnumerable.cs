namespace System.Collections.Generic;

/// <summary>
/// Extension methods for the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Pages an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable object that this method extends.</param>
    /// <param name="pageSize">The number of elements in a page.</param>
    /// <param name="pageNumber">The 1-based page number to return.</param>
    /// <returns>The elements at the given page.</returns>
    public static IEnumerable<T> Page<T>(this IEnumerable<T> enumerable, int pageSize, int pageNumber)
    {
        return enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Shuffles the elements of an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to shuffle.</param>
    /// <returns>The same enumerable with the elements reordered.</returns>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="enumerable"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the enumerable is read-only.</exception>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) => enumerable.ToList().Shuffle();
}
