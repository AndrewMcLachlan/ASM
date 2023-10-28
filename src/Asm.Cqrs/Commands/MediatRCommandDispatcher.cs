using MediatR;

namespace Asm.Cqrs.Commands;

internal class MediatRCommandDispatcher : Mediator, ICommandDispatcher
{
    public MediatRCommandDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public Task<T> Dispatch<T>(ICommand<T> command, CancellationToken cancellationToken = default) =>
        Send(command, cancellationToken);

    public Task Dispatch(ICommand command, CancellationToken cancellationToken = default) =>
        Send(command, cancellationToken);

    public Task<object?> Dispatch(object query, CancellationToken cancellationToken = default) =>
        Send(query, cancellationToken);
}
