namespace Asm.Domain;

public interface IWritableRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : KeyedEntity<TKey> where TKey : struct
{

    TEntity Add(TEntity entity);

    TEntity Update(TEntity entity);
}
