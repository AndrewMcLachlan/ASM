using Asm.Cqrs.Queries;

namespace Asm.Cqrs.Tests.Queries;

internal class TestQuery : IQuery<string>
{
    public string Input { get; init; }
}
