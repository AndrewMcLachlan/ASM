namespace Asm.Domain.Tests;

[Binding]
public class EntitySteps
{
    private TestEntity _entity;

    [When(@"I create a new test entity")]
    public void WhenICreateANewTestEntity()
    {
        _entity = new TestEntity();
    }

    [Given(@"I have a test entity")]
    public void GivenIHaveATestEntity()
    {
        _entity = new TestEntity();
    }

    [Given(@"I have a test entity with (.*) domain events")]
    public void GivenIHaveATestEntityWithDomainEvents(int count)
    {
        _entity = new TestEntity();
        for (int i = 0; i < count; i++)
        {
            _entity.Events.Add(new TestDomainEvent());
        }
    }

    [When(@"I add a domain event to the entity")]
    public void WhenIAddADomainEventToTheEntity()
    {
        _entity.Events.Add(new TestDomainEvent());
    }

    [When(@"I add (.*) domain events to the entity")]
    public void WhenIAddDomainEventsToTheEntity(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _entity.Events.Add(new TestDomainEvent());
        }
    }

    [When(@"I clear the domain events")]
    public void WhenIClearTheDomainEvents()
    {
        _entity.Events.Clear();
    }

    [Then(@"the entity should have (.*) events")]
    public void ThenTheEntityShouldHaveEvents(int count)
    {
        Assert.Equal(count, _entity.Events.Count);
    }
}
