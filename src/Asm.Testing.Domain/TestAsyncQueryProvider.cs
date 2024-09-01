using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Asm.Testing.Domain;

/// <summary>
/// An AsyncQueryProvider that can be used to test async queries.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <param name="inner">The inner query provider.</param>
public class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
{
    /// <summary>
    /// Creates a query from the expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>A queryable.</returns>
    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    /// <summary>
    /// Creates a query from the expression.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>A queryable.</returns>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <param name="expression">The expression to execute</param>
    /// <returns>The result of the query.</returns>
    public object? Execute(Expression expression) => inner.Execute(expression);

    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression to execute</param>
    /// <returns>The result of the query.</returns>
    public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <param name="expression">The expression to execute</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    public Task<object?> ExecuteAsync(Expression expression, CancellationToken cancellationToken) =>
        Task.FromResult(Execute(expression));

    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression to execute</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) =>
        Execute<TResult>(expression);
}
