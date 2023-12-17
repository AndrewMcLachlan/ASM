namespace Asm.Domain;
public interface ISpecification<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}
