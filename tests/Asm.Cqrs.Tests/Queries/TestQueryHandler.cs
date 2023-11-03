using Asm.Cqrs.Queries;

namespace Asm.Cqrs.Tests.Queries;

internal class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public ValueTask<string> Handle(TestQuery request, CancellationToken cancellationToken) =>
        new(request.Input.ToUpper());
}
