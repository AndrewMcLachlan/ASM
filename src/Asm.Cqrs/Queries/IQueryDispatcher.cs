namespace Asm.Cqrs.Queries;

public interface IQueryDispatcher
{
    public Task<T> Dispatch<T>(IQuery<T> query, CancellationToken cancellationToken = default);
}