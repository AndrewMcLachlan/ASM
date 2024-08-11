using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A base class for a repository that can delete entities.
/// </summary>
/// <typeparam name="TContext">The DB context type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's key type.</typeparam>
/// <param name="context">The DB context.</param>
public abstract class RepositoryDeleteBase<TContext, TEntity, TKey>(TContext context) : RepositoryWriteBase<TContext, TEntity, TKey>(context), IDeletableRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    /// <inheritdoc/>
    public abstract void Delete(TKey id);

    /// <inheritdoc/>
    public virtual void Delete(TEntity entity)
    {
        Entities.Remove(entity);
    }
}
