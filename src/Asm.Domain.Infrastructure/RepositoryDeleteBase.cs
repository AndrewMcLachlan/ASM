using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryDeleteBase<TContext, TEntity, TKey> : RepositoryBase<TContext, TEntity, TKey>, IDeleteRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    public RepositoryDeleteBase(TContext context) : base(context) { }

    public abstract void Delete(TKey id);
}
