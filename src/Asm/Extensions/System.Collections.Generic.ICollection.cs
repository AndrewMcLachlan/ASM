namespace Asm.Extensions;

public static class ICollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (collection.IsReadOnly) throw new InvalidOperationException("Collection is readonly");

        if (items == null) return;

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
