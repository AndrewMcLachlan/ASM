namespace Asm.Domain;

public interface IDeletableRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    void Delete(TKey id);

    void Delete(TEntity entity);
}
