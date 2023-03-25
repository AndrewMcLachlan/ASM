using Asm.Cqrs.Commands;

namespace Asm.Cqrs.Tests.Commands;

internal class TestCommandHandler : ICommandHandler<TestCommand, bool>
{
    public Task<bool> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Input == request.Input.ToUpper());
    }
}
