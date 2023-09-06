using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;
public static class IQueryableExtensions
{
    public static Task<TEntity?> FindAsync<TEntity, TKey>(this IQueryable<TEntity> queryable, TKey id, CancellationToken cancellationToken = default) where TEntity : KeyedEntity<TKey> =>
        queryable.SingleOrDefaultAsync(e => e.Id != null && e.Id.Equals(id), cancellationToken);
}
