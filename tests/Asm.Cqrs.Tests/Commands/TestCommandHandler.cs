using Asm.Cqrs.Commands;

namespace Asm.Cqrs.Tests.Commands;

internal class TestCommandHandler : ICommandHandler<TestCommand, bool>
{
    public ValueTask<bool> Handle(TestCommand request, CancellationToken cancellationToken) =>
        new(request.Input.Equals(request.Input.ToUpperInvariant()));
}
