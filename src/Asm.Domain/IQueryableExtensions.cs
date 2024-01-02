namespace Asm.Domain;
public static class IQueryableExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification) where T : Entity
        => specification == null ? query : specification.Apply(query);
}
