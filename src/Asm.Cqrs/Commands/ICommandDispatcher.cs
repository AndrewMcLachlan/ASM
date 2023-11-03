namespace Asm.Cqrs.Commands;

/// <summary>
/// Dispatches commands.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command to a command handler and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The response from the command.</returns>
    public ValueTask<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command of an unknown type to a command handler.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>An object or <c>null</c>.</returns>
    public ValueTask<object?> Dispatch(object command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that does not return a response.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task.</returns>
    public ValueTask Dispatch(ICommand command, CancellationToken cancellationToken = default);
}