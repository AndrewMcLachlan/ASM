using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asm.Domain
{
    public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
    {
        IQueryable<TEntity> Queryable();

        Task<IEnumerable<TEntity>> Get();

        Task<TEntity> Get(TKey Id);

        TEntity Add(TEntity item);

        void Delete(TKey id);
    }
}