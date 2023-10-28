using MediatR;

namespace Asm.Cqrs.Queries;

internal class MediatrQueryDispatcher : Mediator, IQueryDispatcher
{
    public MediatrQueryDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public Task<T> Dispatch<T>(IQuery<T> query, CancellationToken cancellationToken = default) =>
        Send(query, cancellationToken);

    public Task<object?> Dispatch(object query, CancellationToken cancellationToken = default) =>
        Send(query, cancellationToken);
}
