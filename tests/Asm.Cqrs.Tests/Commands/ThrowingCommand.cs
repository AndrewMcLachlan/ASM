using Asm.Cqrs.Commands;

namespace Asm.Cqrs.Tests.Commands;

internal class ThrowingCommand : ICommand
{
}

internal class ThrowingCommandHandler : ICommandHandler<ThrowingCommand>
{
    // Throws synchronously (the method is not async) so the throw propagates through
    // MethodInfo.Invoke — exercising BindingFlags.DoNotWrapExceptions.
    public ValueTask Handle(ThrowingCommand request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("boom");
}
