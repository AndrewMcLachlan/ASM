namespace Asm.Domain;

/// <summary>
/// A repository that can read and write entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IWritableRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : KeyedEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity.</returns>

    TEntity Add(TEntity entity);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity.</returns>
    TEntity Update(TEntity entity);
}
