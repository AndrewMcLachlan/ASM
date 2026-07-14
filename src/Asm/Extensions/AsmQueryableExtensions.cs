using System.Linq.Expressions;
using Asm;

namespace System.Linq;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/> interface.
/// </summary>
public static class AsmQueryableExtensions
{
    /// <summary>
    /// Skip / takes a page of data.
    /// </summary>
    /// <typeparam name="TSource"> The type of the data in the data source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="page">The page specification.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains the specified number of elements from
    /// the page.
    /// </returns>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, IPageable page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return source.Page(page.PageSize, page.PageNumber);
    }

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
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageSize"/> or <paramref name="pageNumber"/> is less than one.</exception>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageSize, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Converts the supplied expressions into a single OR'd where expression.
    /// </summary>
    /// <remarks>
    /// An empty <paramref name="predicates"/> collection applies no filter and the source is
    /// returned unchanged.
    /// </remarks>
    /// <typeparam name="T">The type of the data in the data source.</typeparam>
    /// <param name="queryable">The <see cref="IQueryable{T}"/> instance that this method extends.</param>
    /// <param name="predicates">An enumerable collection of predicate expressions.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with the predicates applied.</returns>
    public static IQueryable<T> WhereAny<T>(this IQueryable<T> queryable, IEnumerable<Expression<Func<T, bool>>> predicates)
    {
        // Materialise once to avoid multiple enumeration and to test for emptiness.
        var predicateList = predicates as IReadOnlyList<Expression<Func<T, bool>>> ?? [.. predicates];

        if (predicateList.Count == 0)
        {
            return queryable;
        }

        var parameter = Expression.Parameter(typeof(T));
        Expression? body = null;
        foreach (var predicate in predicateList)
        {
            var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
            var visited = visitor.Visit(predicate.Body);
            body = body is null ? visited : Expression.OrElse(body, visited);
        }

        return queryable.Where(Expression.Lambda<Func<T, bool>>(body!, parameter));
    }
}

file class ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) =>
        ReferenceEquals(node, source) ? destination : base.VisitParameter(node);
}
