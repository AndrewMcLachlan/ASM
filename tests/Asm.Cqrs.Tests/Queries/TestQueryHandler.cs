using Asm.Cqrs.Queries;

namespace Asm.Cqrs.Tests.Queries;

internal class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> Handle(TestQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Input.ToUpper());
    }
}
