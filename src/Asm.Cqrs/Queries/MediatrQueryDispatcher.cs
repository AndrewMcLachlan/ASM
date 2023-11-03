using MediatR;

namespace Asm.Cqrs.Queries;

internal class MediatRQueryDispatcher : Mediator, IQueryDispatcher
{
    public MediatRQueryDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public async ValueTask<T> Dispatch<T>(IQuery<T> query, CancellationToken cancellationToken = default) =>
        await Send(query, cancellationToken);

    public async ValueTask<object?> Dispatch(object query, CancellationToken cancellationToken = default) =>
        await Send(query, cancellationToken);
}
