﻿using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public abstract class RepositoryBase<TContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>, new()
        where TContext : DbContext
{
    protected TContext Context { get; private set; }

    protected DbSet<TEntity> Entities { get => Context.Set<TEntity>(); }

    public RepositoryBase(TContext context)
    {
        Context = context;
    }

    public virtual TEntity Add(TEntity entity)
    {
        return Entities.Add(entity).Entity;
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    public virtual IQueryable<TEntity> Queryable()
    {
        return Entities.AsQueryable();
    }

    public virtual async Task<IEnumerable<TEntity>> Get()
    {
        return await Entities.ToArrayAsync();
    }

    public virtual async Task<TEntity> Get(TKey id)
    {
        var entity = await Entities.SingleOrDefaultAsync(t => t.Id != null && t.Id.Equals(id));
        return entity ?? throw new NotFoundException();
    }

    public abstract void Delete(TKey id);
}