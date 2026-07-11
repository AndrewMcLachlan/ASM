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
    where TKey : notnull
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
        var entity = await GetById(id).SingleOrDefaultAsync(cancellationToken);
        return entity ?? throw new NotFoundException();
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> Get(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var entity = await specification.Apply(GetById(id)).SingleOrDefaultAsync(cancellationToken);
        return entity ?? throw new NotFoundException();
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> Find(TKey id, CancellationToken cancellationToken = default) =>
        await GetById(id).SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<TEntity?> Find(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await specification.Apply(GetById(id)).SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual Task<TEntity?> TryGet(TKey id, CancellationToken cancellationToken = default) =>
        Find(id, cancellationToken);

    /// <inheritdoc/>
    public virtual Task<TEntity?> TryGet(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        Find(id, specification, cancellationToken);

    /// <summary>
    /// Gets a query for a single entity by ID.
    /// </summary>
    /// <remarks>
    /// Override to apply filtering (e.g. tenancy or ownership checks) that all
    /// <see cref="Get(TKey, CancellationToken)"/> overloads should honour.
    /// </remarks>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>A query filtered to the matching entity.</returns>
    protected virtual IQueryable<TEntity> GetById(TKey id) => Entities.Where(t => t.Id.Equals(id));
}
