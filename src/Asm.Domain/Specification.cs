using System.Linq.Expressions;

namespace Asm.Domain;

/// <summary>
/// A convenience base class for building a <see cref="ISpecification{TEntity}"/> from a
/// <see cref="Criteria"/> predicate.
/// </summary>
/// <remarks>
/// Derive from this class and provide <see cref="Criteria"/> to describe the filter. The default
/// <see cref="Apply"/> filters by <see cref="Criteria"/>; override it to add shaping such as
/// <c>Include</c>, <c>OrderBy</c> or paging.
/// </remarks>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    /// <inheritdoc/>
    public abstract Expression<Func<TEntity, bool>> Criteria { get; }

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> Apply(IQueryable<TEntity> query) => query.Where(Criteria);

    /// <inheritdoc/>
    public virtual ISpecification<TEntity> And(ISpecification<TEntity> specification) => new AndSpecification<TEntity>(this, specification);

    /// <inheritdoc/>
    public virtual ISpecification<TEntity> Or(ISpecification<TEntity> specification) => new OrSpecification<TEntity>(this, specification);

    /// <inheritdoc/>
    public virtual ISpecification<TEntity> Not() => new NotSpecification<TEntity>(this);
}

/// <summary>
/// A specification matching entities that satisfy both operands' criteria.
/// </summary>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
internal sealed class AndSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndSpecification{TEntity}"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public AndSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        Criteria = left.Criteria.AndAlso(right.Criteria);
    }

    /// <inheritdoc/>
    public override Expression<Func<TEntity, bool>> Criteria { get; }
}

/// <summary>
/// A specification matching entities that satisfy either operand's criteria.
/// </summary>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
internal sealed class OrSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrSpecification{TEntity}"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public OrSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        Criteria = left.Criteria.OrElse(right.Criteria);
    }

    /// <inheritdoc/>
    public override Expression<Func<TEntity, bool>> Criteria { get; }
}

/// <summary>
/// A specification matching entities that do <em>not</em> satisfy the wrapped specification's criteria.
/// </summary>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
internal sealed class NotSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotSpecification{TEntity}"/> class.
    /// </summary>
    /// <param name="specification">The specification to negate.</param>
    public NotSpecification(ISpecification<TEntity> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        Criteria = specification.Criteria.Not();
    }

    /// <inheritdoc/>
    public override Expression<Func<TEntity, bool>> Criteria { get; }
}
