namespace Asm.Domain;

public interface IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    IQueryable<TEntity> Queryable();

    Task<IEnumerable<TEntity>> Get();

    Task<TEntity> Get(TKey Id);

    TEntity Add(TEntity item);
}