namespace Asm.Domain;

public interface IRepository<TEntity, TKey> where TEntity : KeyedEntity<TKey>
{
    IQueryable<TEntity> Queryable();

    Task<IEnumerable<TEntity>> Get();

    Task<TEntity> Get(TKey Id);

    TEntity Add(TEntity item);

    void Delete(TKey id);
}