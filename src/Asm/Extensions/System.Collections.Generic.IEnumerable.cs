namespace System.Collections.Generic;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Page<T>(this IEnumerable<T> enumerable, int pageSize, int pageNumber)
    {
        return enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
