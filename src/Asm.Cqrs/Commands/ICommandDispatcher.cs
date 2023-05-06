namespace Asm.Cqrs.Commands;

public interface ICommandDispatcher
{
    public Task<T> Dispatch<T>(ICommand<T> command, CancellationToken cancellationToken = default);

    public Task Dispatch(ICommand command, CancellationToken cancellationToken = default);
}