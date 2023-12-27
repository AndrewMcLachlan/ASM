using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryDeleteBase<TContext, TEntity, TKey>(TContext context) : RepositoryWriteBase<TContext, TEntity, TKey>(context), IDeletableRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    public abstract void Delete(TKey id);

    public virtual void Delete(TEntity entity)
    {
        Entities.Remove(entity);
    }
}
