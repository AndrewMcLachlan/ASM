namespace Asm.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageSize, int pageNumber)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
