using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryBase<TContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    protected TContext Context { get; private set; }

    protected DbSet<TEntity> Entities { get => Context.Set<TEntity>(); }

    public RepositoryBase(TContext context)
    {
        Context = context;
    }

    public virtual TEntity Add(TEntity entity)
    {
        return Entities.Add(entity).Entity;
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    public virtual IQueryable<TEntity> Queryable()
    {
        return Entities.AsQueryable();
    }

    public virtual async Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken = default) =>
        await Entities.ToArrayAsync(cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> Get(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await specification.Apply(Entities).ToArrayAsync(cancellationToken);

    public virtual async Task<TEntity> Get(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await Entities.SingleOrDefaultAsync(t => t.Id.Equals(id), cancellationToken);
        return entity ?? throw new NotFoundException();
    }

    public async Task<TEntity> Get(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = specification.Apply(Entities);

        var entity = await query.SingleOrDefaultAsync(t => t.Id.Equals(id), cancellationToken);
        return entity ?? throw new NotFoundException();
    }
}
