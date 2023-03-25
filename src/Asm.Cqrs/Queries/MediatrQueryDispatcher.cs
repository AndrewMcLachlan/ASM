using MediatR;

namespace Asm.Cqrs.Queries;

internal class MediatrQueryDispatcher : Mediator, IQueryDispatcher
{
    public MediatrQueryDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public Task<T> Dispatch<T>(IQuery<T> command, CancellationToken cancellationToken = default) =>
        Send(command, cancellationToken);
}
