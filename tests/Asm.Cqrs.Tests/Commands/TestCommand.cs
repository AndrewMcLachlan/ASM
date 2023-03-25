using Asm.Cqrs.Commands;

namespace Asm.Cqrs.Tests.Commands;

internal class TestCommand : ICommand<bool>
{
    public string Input { get; init; }
}
