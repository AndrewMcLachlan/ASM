using MediatR;

namespace Asm.Cqrs.Commands;

internal class MediatRCommandDispatcher : Mediator, ICommandDispatcher
{
    public MediatRCommandDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public async ValueTask<T> Dispatch<T>(ICommand<T> command, CancellationToken cancellationToken = default) =>
        await Send(command, cancellationToken);

    public async ValueTask Dispatch(ICommand command, CancellationToken cancellationToken = default) =>
        await Send(command, cancellationToken);

    public async ValueTask<object?> Dispatch(object query, CancellationToken cancellationToken = default) =>
        await Send(query, cancellationToken);
}
