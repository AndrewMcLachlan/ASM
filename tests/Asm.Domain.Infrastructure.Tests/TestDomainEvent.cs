namespace Asm.Domain.Infrastructure.Tests;

internal class TestDomainEvent : IDomainEvent
{
}

internal class TestDomainEventHandler(ScenarioContext context) : IDomainEventHandler<TestDomainEvent>
{
    public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        context.AddResult(true);
        return Task.CompletedTask;
    }
}
