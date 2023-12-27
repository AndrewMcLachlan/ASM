using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryBase<TContext, TEntity, TKey>(TContext context) : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    protected TContext Context { get; private init; } = context;

    protected DbSet<TEntity> Entities { get => Context.Set<TEntity>(); }

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
