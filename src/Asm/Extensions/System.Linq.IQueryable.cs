using System.Linq.Expressions;

namespace System.Linq;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/> interface.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Skip / takes a page of data.
    /// </summary>
    /// <typeparam name="TSource"> The type of the data in the data source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="pageSize">The number of data items per page.</param>
    /// <param name="pageNumber">The page number to retrieve. Pages start from 1.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains the specified number of elements from
    /// the page.
    /// </returns>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageSize, int pageNumber)
    {
        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Converts the supplied expressions into a single where/or expression.
    /// </summary>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="queryable">The <see cref="IQueryable{T}"/> instance that this method extends.</param>
    /// <param name="predicates">An enumerable collection of predicate expressions.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with the predicates applied.</returns>
    public static IQueryable<T> WhereAny<T>(this IQueryable<T> queryable, IEnumerable<Expression<Func<T, bool>>> predicates)
    {
        var parameter = Expression.Parameter(typeof(T));
        return queryable.Where(Expression.Lambda<Func<T, bool>>(
            predicates.Aggregate<Expression<Func<T, bool>>, Expression>(
                null!,
                (current, predicate) =>
                {
                    var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                    return current != null ? Expression.OrElse(current, visitor.Visit(predicate.Body)) : visitor.Visit(predicate.Body);
                }),
            parameter));
    }
}

file class ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return ReferenceEquals(node, source) ? destination : base.VisitParameter(node);
    }
}
