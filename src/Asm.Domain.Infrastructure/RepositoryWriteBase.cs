using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryWriteBase<TContext, TEntity, TKey>(TContext context) : RepositoryBase<TContext, TEntity, TKey>(context), IWritableRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    public virtual TEntity Add(TEntity entity)
    {
        return Entities.Add(entity).Entity;
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }


    public TEntity Update(TEntity entity)
    {
        return Entities.Update(entity).Entity;
    }
}
