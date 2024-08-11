namespace Asm.Domain;

/// <summary>
/// A repository that can read entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    /// <summary>
    /// Gets a queryable collection of entities.
    /// </summary>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<TEntity> Queryable();

    /// <summary>
    /// Gets a collection of entities.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a collection of entities and returns the fields that match a specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> Get(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its key.
    /// </summary>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An entity.</returns>
    Task<TEntity> Get(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its key and returns the fields that match a specification.
    /// </summary>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An entity.</returns>
    Task<TEntity> Get(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}