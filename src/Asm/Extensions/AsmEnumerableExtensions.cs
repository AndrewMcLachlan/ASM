using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

/// <summary>
/// Extension methods for the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class AsmEnumerableExtensions
{
    /// <summary>
    /// Pages an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable object that this method extends.</param>
    /// <param name="pageSize">The number of elements in a page. Must be greater than zero.</param>
    /// <param name="pageNumber">The 1-based page number to return. Must be greater than zero.</param>
    /// <returns>The elements at the given page.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerable"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageSize"/> or <paramref name="pageNumber"/> is less than one.</exception>
    public static IEnumerable<T> Page<T>(this IEnumerable<T> enumerable, int pageSize, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);

        return enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Shuffles the elements of an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to shuffle.</param>
    /// <returns>A new sequence containing the elements in a random order. The source enumerable is not modified.</returns>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="enumerable"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) => enumerable.ToList().Shuffle();

    /// <summary>
    /// Determines whether an enumerable is <c>null</c> or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <returns><c>true</c> if the enumerable is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? enumerable) =>
        !enumerable?.Any() ?? true;

    /// <summary>
    /// Determines whether a enumerable is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <returns><c>true</c> if the enumerable is empty; otherwise, <c>false</c>.</returns>
    public static bool Empty<T>(this IEnumerable<T> enumerable) =>
        !enumerable.Any();

    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Projects each element of a sequence using an asynchronous selector, awaiting each element in turn.
        /// </summary>
        /// <remarks>
        /// Elements are awaited sequentially rather than concurrently, making this safe for selectors that
        /// share non-thread-safe state such as an Entity Framework Core <c>DbContext</c>.
        /// </remarks>
        /// <typeparam name="TResult">The type of the projected elements.</typeparam>
        /// <param name="selector">An asynchronous transform function to apply to each element.</param>
        /// <returns>A list of the projected elements, in source order.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="selector"/> is <c>null</c>.</exception>
        public async Task<List<TResult>> SelectAsync<TResult>(Func<TSource, Task<TResult>> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            List<TResult> results = source.TryGetNonEnumeratedCount(out var count) ? new(count) : [];

            foreach (var item in source)
            {
                results.Add(await selector(item));
            }

            return results;
        }
    }
}
