namespace System.Collections.Generic;

/// <summary>
/// Extensions for the <see cref="ICollection{T}"/> interface.
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">
    /// The <see cref="ICollection{T}"/> object that this method extends.
    /// </param>
    /// <param name="items">
    /// The collection whose elements should be added to the end of the <see cref="ICollection{T}"/>.
    /// The collection itself cannot be null, but it can contain elements that are <see langword="null" />,
    /// if type T is a reference type.
    /// </param>
    /// <exception cref="InvalidOperationException">Collection is readonly.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> are null.</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (collection.IsReadOnly) throw new InvalidOperationException("Collection is readonly");

        if (items == null) return;

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
