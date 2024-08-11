using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// Base class for a repository.
/// </summary>
/// <typeparam name="TContext">The DB context type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's key type.</typeparam>
/// <param name="context">The DB context.</param>
public abstract class RepositoryBase<TContext, TEntity, TKey>(TContext context) : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TContext : DbContext
    where TKey : struct
{
    /// <summary>
    /// Gets the DB context.
    /// </summary>
    protected TContext Context { get; private init; } = context;

    /// <summary>
    /// Gets the entities.
    /// </summary>
    protected DbSet<TEntity> Entities { get => Context.Set<TEntity>(); }

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> Queryable() => Entities.AsQueryable();


    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken = default) =>
        await Entities.ToArrayAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<TEntity>> Get(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await specification.Apply(Entities).ToArrayAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<TEntity> Get(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await Entities.SingleOrDefaultAsync(t => t.Id.Equals(id), cancellationToken);
        return entity ?? throw new NotFoundException();
    }

    /// <inheritdoc/>
    public async Task<TEntity> Get(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = specification.Apply(Entities);

        var entity = await query.SingleOrDefaultAsync(t => t.Id.Equals(id), cancellationToken);
        return entity ?? throw new NotFoundException();
    }
}
