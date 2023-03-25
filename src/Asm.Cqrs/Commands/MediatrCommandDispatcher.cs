using MediatR;

namespace Asm.Cqrs.Commands;

internal class MediatrCommandDispatcher : Mediator, ICommandDispatcher
{
    public MediatrCommandDispatcher(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public Task<T> Dispatch<T>(ICommand<T> command, CancellationToken cancellationToken = default) =>
        Send(command, cancellationToken);

    public Task Dispatch<T>(ICommand command, CancellationToken cancellationToken = default) =>
        Send(command, cancellationToken);
}
