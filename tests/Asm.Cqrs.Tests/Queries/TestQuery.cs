using Asm.Cqrs.Queries;

namespace Asm.Cqrs.Tests.Queries;

internal class TestQuery : IQuery<string>
{
    public required string Input { get; init; }
}
