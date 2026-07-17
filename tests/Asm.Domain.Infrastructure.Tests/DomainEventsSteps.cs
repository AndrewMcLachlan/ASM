using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class DomainEventsSteps(ScenarioContext context)
{
    private IServiceProvider _serviceProvider = null!;

    [BeforeScenario]
    public void Setup()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
        services.AddDomainEvents(Assembly.GetExecutingAssembly());
        services.AddSingleton(context);

        _serviceProvider = services.BuildServiceProvider();
    }

    [Given(@"An entity defines a domain event")]
    public void GivenAnEntityDefinesADomainEvent()
    {
        TestEntity testEntity = new(1);
        testEntity.TriggerDomainEvent();

        TestDbContext dbContext = _serviceProvider.GetRequiredService<TestDbContext>();
        dbContext.Add(testEntity);
    }

    [Given("An entity defines a multi domain event")]
    public void GivenAnEntityDefinesAMultiDomainEvent()
    {
        TestEntity testEntity = new(1);
        testEntity.TriggerDomainEventMulti();

        TestDbContext dbContext = _serviceProvider.GetRequiredService<TestDbContext>();
        dbContext.Add(testEntity);
    }

    [Given("An entity defines a two-phase domain event")]
    public void GivenAnEntityDefinesATwoPhaseDomainEvent()
    {
        TestEntity testEntity = new(1);
        testEntity.TriggerTwoPhaseDomainEvent();

        TestDbContext dbContext = _serviceProvider.GetRequiredService<TestDbContext>();
        dbContext.Add(testEntity);
    }

    [When(@"I call SaveChanges")]
    public void WhenICallSaveChanges()
    {
        TestDbContext dbContext = _serviceProvider.GetRequiredService<TestDbContext>();
        dbContext.SaveChanges();
    }

    [Then(@"The domain event is handled")]
    public void ThenTheDomainEventIsHandled()
    {
        Assert.True(context.GetResult<bool>());
    }

    [Then("The domain event is handled multiple times")]
    public void ThenTheDomainEventIsHandledMultipleTimes()
    {
        var result1 = context.Get<int>("Handler1");
        var result2 = context.Get<int>("Handler2");

        Assert.Equal(1, result1);
        Assert.Equal(2, result2);
    }

    [Then("The domain event is handled pre-save and post-save")]
    public void ThenTheDomainEventIsHandledPreSaveAndPostSave()
    {
        Assert.True(context.Get<bool>("PreSave"));
        Assert.True(context.Get<bool>("PostSave"));
    }
}
