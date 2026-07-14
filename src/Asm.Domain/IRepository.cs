namespace Asm.Domain;

/// <summary>
/// A repository that can read entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : notnull
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
    /// Gets an entity by its key, throwing if it does not exist.
    /// </summary>
    /// <remarks>
    /// Implementations throw <c>NotFoundException</c> when no entity with the given key exists. Use
    /// <see cref="Find(TKey, CancellationToken)"/> or <see cref="TryGet(TKey, CancellationToken)"/> to get
    /// <see langword="null"/> instead of an exception.
    /// </remarks>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity.</returns>
    Task<TEntity> Get(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its key and returns the fields that match a specification, throwing if it does not exist.
    /// </summary>
    /// <remarks>
    /// Implementations throw <c>NotFoundException</c> when no matching entity exists. Use
    /// <see cref="Find(TKey, ISpecification{TEntity}, CancellationToken)"/> or
    /// <see cref="TryGet(TKey, ISpecification{TEntity}, CancellationToken)"/> to get <see langword="null"/>
    /// instead of an exception.
    /// </remarks>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity.</returns>
    Task<TEntity> Get(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an entity by its key, returning <see langword="null"/> if it does not exist.
    /// </summary>
    /// <remarks>
    /// This is the null-returning counterpart to <see cref="Get(TKey, CancellationToken)"/>, which throws
    /// <c>NotFoundException</c> when the entity is absent. Prefer <c>Find</c> when a missing entity is an
    /// expected, non-exceptional outcome. The default implementation filters the full collection; derived
    /// repositories override it with a keyed query.
    /// </remarks>
    /// <param name="id">The ID of the entity to find.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none exists.</returns>
    async Task<TEntity?> Find(TKey id, CancellationToken cancellationToken = default)
    {
        var entities = await Get(cancellationToken).ConfigureAwait(false);
        return entities.SingleOrDefault(entity => EqualityComparer<TKey>.Default.Equals(entity.Id, id));
    }

    /// <summary>
    /// Finds an entity by its key applying a specification, returning <see langword="null"/> if it does not exist.
    /// </summary>
    /// <remarks>
    /// This is the null-returning counterpart to
    /// <see cref="Get(TKey, ISpecification{TEntity}, CancellationToken)"/>, which throws
    /// <c>NotFoundException</c> when the entity is absent. The default implementation filters the specified
    /// collection; derived repositories override it with a keyed query.
    /// </remarks>
    /// <param name="id">The ID of the entity to find.</param>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none exists.</returns>
    async Task<TEntity?> Find(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var entities = await Get(specification, cancellationToken).ConfigureAwait(false);
        return entities.SingleOrDefault(entity => EqualityComparer<TKey>.Default.Equals(entity.Id, id));
    }

    /// <summary>
    /// Tries to get an entity by its key, returning <see langword="null"/> if it does not exist.
    /// </summary>
    /// <remarks>An ergonomic alias for <see cref="Find(TKey, CancellationToken)"/>.</remarks>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none exists.</returns>
    Task<TEntity?> TryGet(TKey id, CancellationToken cancellationToken = default) => Find(id, cancellationToken);

    /// <summary>
    /// Tries to get an entity by its key applying a specification, returning <see langword="null"/> if it does not exist.
    /// </summary>
    /// <remarks>An ergonomic alias for <see cref="Find(TKey, ISpecification{TEntity}, CancellationToken)"/>.</remarks>
    /// <param name="id">The ID of the entity to get.</param>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none exists.</returns>
    Task<TEntity?> TryGet(TKey id, ISpecification<TEntity> specification, CancellationToken cancellationToken = default) => Find(id, specification, cancellationToken);
}