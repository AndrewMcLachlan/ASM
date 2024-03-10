using Asm.Testing;

namespace Asm.Domain.Infrastructure.Tests;
internal class TestDomainEvent : IDomainEvent
{
}

internal class TestDomainEventHandler(ScenarioResult<bool> result) : IDomainEventHandler<TestDomainEvent>
{
    public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        result.Value = true;
        return Task.CompletedTask;
    }
}
