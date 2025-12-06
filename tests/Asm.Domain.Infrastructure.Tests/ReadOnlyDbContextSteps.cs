using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class ReadOnlyDbContextSteps(ScenarioContext context)
{
    private IServiceCollection _services = null!;
    private IServiceCollection _result = null!;
    private ServiceProvider _serviceProvider = null!;
    private bool _optionsActionInvoked;
    private IServiceProvider _capturedServiceProvider = null!;

    #region Test Types

    public class TestReadOnlyDbContext : DbContext, IReadOnlyDbContext
    {
        public TestReadOnlyDbContext(DbContextOptions<TestReadOnlyDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TestDb");
            }
        }
    }

    public interface ITestReadOnlyDbContext : IReadOnlyDbContext
    {
    }

    public class TestDbContextWithInterface : DbContext, ITestReadOnlyDbContext
    {
        public TestDbContextWithInterface(DbContextOptions<TestDbContextWithInterface> options) : base(options)
        {
        }
    }

    #endregion

    [Given(@"I have a service collection for DbContext")]
    public void GivenIHaveAServiceCollectionForDbContext()
    {
        _services = new ServiceCollection();
    }

    [When(@"I call AddReadOnlyDbContext with default parameters")]
    public void WhenICallAddReadOnlyDbContextWithDefaultParameters()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>();
    }

    [When(@"I call AddReadOnlyDbContext with null optionsAction")]
    public void WhenICallAddReadOnlyDbContextWithNullOptionsAction()
    {
        Action<DbContextOptionsBuilder> optionsAction = null!;
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(optionsAction);
    }

    [When(@"I call AddReadOnlyDbContext with an optionsAction")]
    public void WhenICallAddReadOnlyDbContextWithAnOptionsAction()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            builder => builder.UseInMemoryDatabase("TestDb"));
    }

    [When(@"I call AddReadOnlyDbContext with an in-memory database")]
    public void WhenICallAddReadOnlyDbContextWithAnInMemoryDatabase()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            builder => builder.UseInMemoryDatabase("TestDb"));
    }

    [When(@"I call AddReadOnlyDbContext with scoped lifetime")]
    public void WhenICallAddReadOnlyDbContextWithScopedLifetime()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            builder => builder.UseInMemoryDatabase("TestDb"),
            contextLifetime: ServiceLifetime.Scoped);
    }

    [When(@"I call AddReadOnlyDbContext with a tracking optionsAction")]
    public void WhenICallAddReadOnlyDbContextWithATrackingOptionsAction()
    {
        _optionsActionInvoked = false;
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(builder =>
        {
            _optionsActionInvoked = true;
            builder.UseInMemoryDatabase("TestDb");
        });
    }

    [When(@"I call AddReadOnlyDbContext with singleton lifetime")]
    public void WhenICallAddReadOnlyDbContextWithSingletonLifetime()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            optionsAction: builder => builder.UseInMemoryDatabase("TestDb"),
            contextLifetime: ServiceLifetime.Singleton);
    }

    [When(@"I call AddReadOnlyDbContext with transient lifetime")]
    public void WhenICallAddReadOnlyDbContextWithTransientLifetime()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            optionsAction: builder => builder.UseInMemoryDatabase("TestDb"),
            contextLifetime: ServiceLifetime.Transient);
    }

    [When(@"I call AddReadOnlyDbContext with context lifetime '(.*)' and options lifetime '(.*)'")]
    public void WhenICallAddReadOnlyDbContextWithContextLifetimeAndOptionsLifetime(string contextLifetime, string optionsLifetime)
    {
        var ctxLifetime = Enum.Parse<ServiceLifetime>(contextLifetime);
        var optLifetime = Enum.Parse<ServiceLifetime>(optionsLifetime);

        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            optionsAction: (sp, builder) => builder.UseInMemoryDatabase($"TestDb_{contextLifetime}_{optionsLifetime}"),
            contextLifetime: ctxLifetime,
            optionsLifetime: optLifetime);
    }

    [When(@"I call AddReadOnlyDbContext with only context lifetime '(.*)'")]
    public void WhenICallAddReadOnlyDbContextWithContextLifetime(string lifetime)
    {
        var serviceLifetime = Enum.Parse<ServiceLifetime>(lifetime);
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            optionsAction: builder => builder.UseInMemoryDatabase($"TestDb_{lifetime}"),
            contextLifetime: serviceLifetime);
    }

    [When(@"I call AddReadOnlyDbContext twice")]
    public void WhenICallAddReadOnlyDbContextTwice()
    {
        context.CatchException(() =>
        {
            _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(ServiceLifetime.Scoped);
            _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(ServiceLifetime.Scoped);
        });
    }

    [When(@"I call AddReadOnlyDbContext with IServiceProvider optionsAction")]
    public void WhenICallAddReadOnlyDbContextWithIServiceProviderOptionsAction()
    {
        _result = _services.AddReadOnlyDbContext<TestReadOnlyDbContext>(
            optionsAction: (sp, builder) =>
            {
                _capturedServiceProvider = sp;
                builder.UseInMemoryDatabase("TestDbProvider");
            });
    }

    [When(@"I call AddReadOnlyDbContext with TContextService and TContextImplementation")]
    public void WhenICallAddReadOnlyDbContextWithTContextServiceAndTContextImplementation()
    {
        _result = _services.AddReadOnlyDbContext<IReadOnlyDbContext, TestReadOnlyDbContext>(
            optionsAction: builder => builder.UseInMemoryDatabase("TestDb"));
    }

    [When(@"I build the DbContext service provider")]
    public void WhenIBuildTheDbContextServiceProvider()
    {
        _serviceProvider = _services.BuildServiceProvider();
    }

    [When(@"I resolve IReadOnlyDbContext")]
    public void WhenIResolveIReadOnlyDbContext()
    {
        _ = _serviceProvider.GetService<IReadOnlyDbContext>();
    }

    [Then(@"the same service collection should be returned")]
    public void ThenTheSameServiceCollectionShouldBeReturned()
    {
        Assert.Same(_services, _result);
    }

    [Then(@"IReadOnlyDbContext can be resolved")]
    public void ThenIReadOnlyDbContextCanBeResolved()
    {
        var dbContext = _serviceProvider.GetService<IReadOnlyDbContext>();
        Assert.NotNull(dbContext);
        Assert.IsType<TestReadOnlyDbContext>(dbContext);
    }

    [Then(@"IReadOnlyDbContext can be resolved within a scope")]
    public void ThenIReadOnlyDbContextCanBeResolvedWithinAScope()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<IReadOnlyDbContext>();
        Assert.NotNull(dbContext);
    }

    [Then(@"the optionsAction should have been invoked")]
    public void ThenTheOptionsActionShouldHaveBeenInvoked()
    {
        Assert.True(_optionsActionInvoked);
    }

    [Then(@"resolving IReadOnlyDbContext twice returns the same instance")]
    public void ThenResolvingIReadOnlyDbContextTwiceReturnsTheSameInstance()
    {
        var instance1 = _serviceProvider.GetRequiredService<IReadOnlyDbContext>();
        var instance2 = _serviceProvider.GetRequiredService<IReadOnlyDbContext>();
        Assert.Same(instance1, instance2);
    }

    [Then(@"resolving IReadOnlyDbContext twice returns different instances")]
    public void ThenResolvingIReadOnlyDbContextTwiceReturnsDifferentInstances()
    {
        var instance1 = _serviceProvider.GetRequiredService<IReadOnlyDbContext>();
        var instance2 = _serviceProvider.GetRequiredService<IReadOnlyDbContext>();
        Assert.NotSame(instance1, instance2);
    }

    [Then(@"the concrete DbContext type should be registered")]
    public void ThenTheConcreteDbContextTypeShouldBeRegistered()
    {
        Assert.Contains(_services, s => s.ServiceType == typeof(TestReadOnlyDbContext));
    }

    [Then(@"the IServiceProvider should have been captured")]
    public void ThenTheIServiceProviderShouldHaveBeenCaptured()
    {
        Assert.NotNull(_capturedServiceProvider);
    }

    [Then(@"the IReadOnlyDbContext should be registered with '(.*)' lifetime")]
    public void ThenTheIReadOnlyDbContextShouldBeRegisteredWithLifetime(string lifetime)
    {
        var expectedLifetime = Enum.Parse<ServiceLifetime>(lifetime);
        var serviceDescriptor = _services.Single(sd => sd.ServiceType == typeof(IReadOnlyDbContext));
        Assert.Equal(expectedLifetime, serviceDescriptor.Lifetime);
    }

    [AfterScenario]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }
}
