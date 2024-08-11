using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// Extension methods for <see cref="IQueryable{TEntity}"/>.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Finds an entity with the given primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's key.</typeparam>
    /// <param name="queryable">The queryable collection of entities.</param>
    /// <param name="id">The ID to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The entity if it is found, otherwise <c>null</c>.</returns>
    public static Task<TEntity?> FindAsync<TEntity, TKey>(this IQueryable<TEntity> queryable, TKey id, CancellationToken cancellationToken = default) where TEntity : KeyedEntity<TKey> =>
        queryable.SingleOrDefaultAsync(e => e.Id != null && e.Id.Equals(id), cancellationToken);
}
