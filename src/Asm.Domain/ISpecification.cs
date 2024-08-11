namespace Asm.Domain;

/// <summary>
/// A specification that can be applied to a query.
/// </summary>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
public interface ISpecification<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Applies the specification to a query.
    /// </summary>
    /// <param name="query">The query to apply the specification to.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with the specification applied.</returns>
    IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}
