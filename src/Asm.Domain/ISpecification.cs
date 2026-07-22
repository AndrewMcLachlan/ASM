using System.Linq.Expressions;

namespace Asm.Domain;

/// <summary>
/// A specification that can be applied to a query.
/// </summary>
/// <remarks>
/// A specification carries two complementary concerns:
/// <list type="bullet">
/// <item>
/// <see cref="Criteria"/> is a boolean predicate describing <em>which</em> entities match. It is an
/// expression tree so it stays translatable by query providers such as EF Core, and it is what the
/// <see cref="And"/>, <see cref="Or"/> and <see cref="Not"/> composition operators combine.
/// </item>
/// <item>
/// <see cref="Apply"/> shapes the query — <c>Include</c>, <c>OrderBy</c>, paging etc. — and by default
/// simply filters by <see cref="Criteria"/>. Composition operates on <see cref="Criteria"/> only; it does
/// <em>not</em> attempt to merge the <see cref="Apply"/>-side shaping of two specifications (there is no
/// meaningful way to combine, say, two <c>OrderBy</c> clauses under an <c>OR</c>).
/// </item>
/// </list>
/// The constraint is <see langword="class"/> (not <c>Entity</c>) so specifications can target read models,
/// DTOs or projected interfaces as well as domain entities.
/// </remarks>
/// <typeparam name="TEntity">The type of entity the specification applies to.</typeparam>
public interface ISpecification<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets the predicate describing which entities the specification matches.
    /// </summary>
    /// <remarks>
    /// The default matches every entity (<c>_ =&gt; true</c>). Override to express the filter as an
    /// expression tree so that it can be translated by the query provider and composed with
    /// <see cref="And"/>, <see cref="Or"/> and <see cref="Not"/>.
    /// </remarks>
    Expression<Func<TEntity, bool>> Criteria => static _ => true;

    /// <summary>
    /// Applies the specification to a query.
    /// </summary>
    /// <remarks>
    /// The default applies <see cref="Criteria"/> via
    /// <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, Boolean}})"/>.
    /// Override to add shaping such as <c>Include</c>, <c>OrderBy</c> or paging.
    /// </remarks>
    /// <param name="query">The query to apply the specification to.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with the specification applied.</returns>
    IQueryable<TEntity> Apply(IQueryable<TEntity> query) => query.Where(Criteria);

    /// <summary>
    /// Combines this specification with another using a logical <c>AND</c>.
    /// </summary>
    /// <remarks>Only the <see cref="Criteria"/> of the two specifications are combined.</remarks>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A specification matching entities that satisfy both.</returns>
    ISpecification<TEntity> And(ISpecification<TEntity> specification) => new AndSpecification<TEntity>(this, specification);

    /// <summary>
    /// Combines this specification with another using a logical <c>OR</c>.
    /// </summary>
    /// <remarks>Only the <see cref="Criteria"/> of the two specifications are combined.</remarks>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A specification matching entities that satisfy either.</returns>
    ISpecification<TEntity> Or(ISpecification<TEntity> specification) => new OrSpecification<TEntity>(this, specification);

    /// <summary>
    /// Negates this specification using a logical <c>NOT</c>.
    /// </summary>
    /// <remarks>Only the <see cref="Criteria"/> is negated.</remarks>
    /// <returns>A specification matching entities that do <em>not</em> satisfy this one.</returns>
    ISpecification<TEntity> Not() => new NotSpecification<TEntity>(this);
}
