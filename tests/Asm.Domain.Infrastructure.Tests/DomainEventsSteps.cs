using System.Reflection;
using Asm.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Asm.Domain.Infrastructure.Tests
{
    [Binding]
    public class DomainEventsSteps(ScenarioResult<bool> result)
    {
        IServiceProvider _serviceProvider;

        [BeforeScenario]
        public void Setup()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("TestDb"));
            services.AddDomainEvents(Assembly.GetExecutingAssembly());
            services.AddSingleton(result);

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
            Assert.True(result.Value);
        }
    }
}
