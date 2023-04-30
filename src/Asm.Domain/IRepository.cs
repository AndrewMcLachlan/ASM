namespace Asm.Domain;

public interface IRepository<TEntity, TKey>
    where TEntity : KeyedEntity<TKey>
    where TKey : struct
{
    IQueryable<TEntity> Queryable();

    Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken = default);

    Task<TEntity> Get(TKey Id, CancellationToken cancellationToken = default);

    TEntity Add(TEntity item);
}