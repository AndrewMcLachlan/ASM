namespace Asm.Domain;

/// <summary>
/// A repository that can delete entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IDeletableRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    /// <summary>
    /// Deletes an entity by its key.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    void Delete(TKey id);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);
}
