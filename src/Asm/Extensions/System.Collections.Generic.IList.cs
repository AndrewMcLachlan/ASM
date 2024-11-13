namespace System.Collections.Generic;

/// <summary>
/// Extensions for the <see cref="IList{T}"/> interface.
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Shuffles the elements of an list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to shuffle.</param>
    /// <returns>The same list with the elements reordered.</returns>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="list"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the list is read-only.</exception>
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (list.IsReadOnly) throw new InvalidOperationException("List is readonly");

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }

    /// <summary>
    /// Determines whether a list is <c>null</c> or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to check.</param>
    /// <returns><c>true</c> if the list is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(this IList<T>? list) =>
        (list?.Count ?? 0) == 0;

    /// <summary>
    /// Determines whether a list is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to check.</param>
    /// <returns><c>true</c> if the list is empty; otherwise, <c>false</c>.</returns>
    public static bool Empty<T>(this IList<T> list) =>
        list.Count == 0;
}
