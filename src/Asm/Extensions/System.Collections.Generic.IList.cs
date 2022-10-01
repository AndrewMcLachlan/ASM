namespace System.Collections.Generic;

public static class IListExtensions
{
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
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
}
