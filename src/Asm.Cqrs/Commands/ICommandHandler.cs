using MediatR;

namespace Asm.Cqrs.Commands;

/// <summary>
/// A handler for a command that returns a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command instance to handle.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The command response.</returns>
    new ValueTask<TResponse> Handle(TCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the command. Use this method if you are bypassing the <see cref="ICommandDispatcher"/> and using <see cref="MediatR"/> directly.
    /// </summary>
    /// <param name="command">The command instance to handle.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TResponse> IRequestHandler<TCommand, TResponse>.Handle(TCommand command, CancellationToken cancellationToken) => Handle(command, cancellationToken).AsTask();
}

/// <summary>
/// A handler for a command that does not return a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command instance to handle.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task.</returns>
    new ValueTask Handle(TCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the command. Use this method if you are bypassing the <see cref="ICommandDispatcher"/> and using MediatR directly.
    /// </summary>
    /// <param name="command">The command instance to handle.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task IRequestHandler<TCommand>.Handle(TCommand command, CancellationToken cancellationToken) => Handle(command, cancellationToken).AsTask();
}
