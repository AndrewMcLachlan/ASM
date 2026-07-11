namespace Asm.Domain;

/// <summary>
/// A repository that can read and write entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IWritableRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : KeyedEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity.</returns>
    TEntity Add(TEntity entity);

    /// <summary>
    /// Adds a range of entities.
    /// </summary>
    /// <remarks>
    /// The default implementation adds each entity in turn via <see cref="Add(TEntity)"/>; override for a
    /// provider-optimised bulk add.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (TEntity entity in entities)
        {
            Add(entity);
        }
    }

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity.</returns>
    TEntity Update(TEntity entity);
}
