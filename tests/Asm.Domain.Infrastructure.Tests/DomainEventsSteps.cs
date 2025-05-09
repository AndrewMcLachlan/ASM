using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class DomainEventsSteps(ScenarioContext context)
{
    private IServiceProvider _serviceProvider;

    [BeforeScenario]
    public void Setup()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("TestDb"));
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
}
