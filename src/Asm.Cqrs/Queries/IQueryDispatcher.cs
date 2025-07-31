namespace Asm.Cqrs.Queries;

/// <summary>
/// Dispatches a query to a query handler.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query to a query handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of response from the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    ValueTask<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

}