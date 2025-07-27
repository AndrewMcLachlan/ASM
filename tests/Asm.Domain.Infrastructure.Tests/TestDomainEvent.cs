namespace Asm.Domain.Infrastructure.Tests;

internal class TestDomainEvent : IDomainEvent
{
}

internal class TestDomainEventHandler(ScenarioContext context) : IDomainEventHandler<TestDomainEvent>
{
    public ValueTask Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        context.AddResult(true);
        return ValueTask.CompletedTask;
    }
}

internal class TestDomainEventMulti : IDomainEvent
{
}

internal class TestDomainEventMultiHandler1(ScenarioContext context) : IDomainEventHandler<TestDomainEventMulti>
{
    public ValueTask Handle(TestDomainEventMulti notification, CancellationToken cancellationToken)
    {
        context.Add("Handler1", 1);
        return ValueTask.CompletedTask;
    }
}

internal class TestDomainEventMultiHandler2(ScenarioContext context) : IDomainEventHandler<TestDomainEventMulti>
{
    public ValueTask Handle(TestDomainEventMulti notification, CancellationToken cancellationToken)
    {
        context.Add("Handler2", 2);
        return ValueTask.CompletedTask;
    }
}