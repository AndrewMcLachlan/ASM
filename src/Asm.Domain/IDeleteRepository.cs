namespace Asm.Domain;

public interface IDeleteRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    void Delete(TKey id);
}
