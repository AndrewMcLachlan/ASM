using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A base class for a repository that can write entities.
/// </summary>
/// <typeparam name="TContext">The DB context type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's key type.</typeparam>
/// <param name="context">The DB context.</param>
public abstract class RepositoryWriteBase<TContext, TEntity, TKey>(TContext context) : RepositoryBase<TContext, TEntity, TKey>(context), IWritableRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    /// <inheritdoc/>
    public virtual TEntity Add(TEntity entity)
    {
        return Entities.Add(entity).Entity;
    }

    /// <inheritdoc/>
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    /// <inheritdoc/>
    public TEntity Update(TEntity entity)
    {
        return Entities.Update(entity).Entity;
    }
}
