namespace Asm.Cqrs.Queries;

public interface IQueryDispatcher
{
    public Task<T> Dispatch<T>(IQuery<T> query, CancellationToken cancellationToken = default);

    public Task<object?> Dispatch(object query, CancellationToken cancellationToken = default);
}