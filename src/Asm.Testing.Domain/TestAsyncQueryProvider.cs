using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Asm.Testing.Domain;

/// <summary>
/// An AsyncQueryProvider that can be used to test async queries.
/// </summary>
/// <remarks>
/// <para>
/// This provider supports chained LINQ operations including projections (Select),
/// filtering (Where), and terminal async operations (SingleOrDefaultAsync, ToListAsync, etc.).
/// </para>
/// <para>
/// The key feature is that it compiles and executes the full expression tree directly,
/// rather than delegating to the inner provider. This allows projections like
/// <c>.Select(x => new Model()).SingleOrDefaultAsync()</c> to work correctly.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <param name="inner">The inner query provider. This parameter is kept for API compatibility but is not used internally.</param>
#pragma warning disable CS9113 // Parameter is unread - kept for API compatibility
public class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
#pragma warning restore CS9113
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
    /// Executes a query by compiling the expression tree.
    /// </summary>
    /// <param name="expression">The expression to execute</param>
    /// <returns>The result of the query.</returns>
    public object? Execute(Expression expression) => CompileAndExecute(expression);

    /// <summary>
    /// Executes a query by compiling the expression tree.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression to execute</param>
    /// <returns>The result of the query.</returns>
    public TResult Execute<TResult>(Expression expression) => (TResult)CompileAndExecute(expression)!;

    /// <summary>
    /// Executes a query asynchronously.
    /// </summary>
    /// <param name="expression">The expression to execute</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    public Task<object?> ExecuteAsync(Expression expression, CancellationToken cancellationToken) =>
        Task.FromResult(CompileAndExecute(expression));

    /// <summary>
    /// Executes a query asynchronously.
    /// </summary>
    /// <remarks>
    /// EF Core expects this method to return <c>Task&lt;T&gt;</c> when called from async methods
    /// like <c>SingleOrDefaultAsync</c>. This implementation detects when <typeparamref name="TResult"/>
    /// is <c>Task&lt;T&gt;</c> and wraps the result appropriately.
    /// </remarks>
    /// <typeparam name="TResult">The type of the result (often Task&lt;T&gt; for async operations).</typeparam>
    /// <param name="expression">The expression to execute</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var result = CompileAndExecute(expression);

        // EF Core expects ExecuteAsync to return Task<T> for async operations
        // TResult will be Task<T>, so we need to wrap the result in a Task
        var resultType = typeof(TResult);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var taskFromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(innerType);
            return (TResult)taskFromResultMethod.Invoke(null, [result])!;
        }

        return (TResult)result!;
    }

    /// <summary>
    /// Compiles and executes an expression tree.
    /// </summary>
    /// <remarks>
    /// This approach works for projections because it compiles the entire expression tree
    /// (including Select, Where, and terminal operations like SingleOrDefault) into a
    /// delegate that can be invoked directly. This bypasses the inner provider which
    /// may not understand expressions built against projected types.
    /// </remarks>
    /// <param name="expression">The expression tree to execute.</param>
    /// <returns>The result of executing the expression.</returns>
    private static object? CompileAndExecute(Expression expression)
    {
        var lambda = Expression.Lambda(expression);
        var compiled = lambda.Compile();
        return compiled.DynamicInvoke();
    }
}
