using Asm.Cqrs.Commands;

namespace Asm.Cqrs.Tests.Commands;

internal class TestCommandHandler : ICommandHandler<TestCommand, bool>
{
    public ValueTask<bool> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(request.Input == request.Input.ToUpper());
    }
}
