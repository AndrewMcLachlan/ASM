using Asm.Domain;

namespace System.Linq;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/> interface.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Apply a specification to a query.
    /// </summary>
    /// <typeparam name="T">The type being queried.</typeparam>
    /// <param name="query">The queryable that this method extends.</param>
    /// <param name="specification">The specification to be applied.</param>
    /// <returns>The <paramref name="query"/> with the specification applied.</returns>
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification) where T : Entity
        => specification == null ? query : specification.Apply(query);
}
