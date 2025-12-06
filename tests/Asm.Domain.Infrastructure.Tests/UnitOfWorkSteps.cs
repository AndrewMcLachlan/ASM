using Asm.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class UnitOfWorkSteps(ScenarioContext context)
{
    private IServiceCollection _services = null!;
    private IServiceCollection _result = null!;
    private ServiceProvider _serviceProvider = null!;

    #region Test Types

    public class TestUnitOfWork : IUnitOfWork
    {
        public int SaveChanges(bool acceptAllChangesOnSuccess = true) => 0;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    #endregion

    [Given(@"I have a null service collection for UnitOfWork")]
    public void GivenIHaveANullServiceCollectionForUnitOfWork()
    {
        _services = null!;
    }

    [Given(@"I have a service collection for UnitOfWork")]
    public void GivenIHaveAServiceCollectionForUnitOfWork()
    {
        _services = new ServiceCollection();
    }

    [Given(@"the concrete UnitOfWork type is registered")]
    public void GivenTheConcreteUnitOfWorkTypeIsRegistered()
    {
        _services.AddScoped<TestUnitOfWork>();
    }

    [When(@"I call AddUnitOfWork on the null collection")]
    public void WhenICallAddUnitOfWorkOnTheNullCollection()
    {
        context.CatchException(() => _result = _services.AddUnitOfWork<TestUnitOfWork>());
    }

    [When(@"I call AddUnitOfWork")]
    public void WhenICallAddUnitOfWork()
    {
        _result = _services.AddUnitOfWork<TestUnitOfWork>();
    }

    [When(@"I call AddUnitOfWork twice")]
    public void WhenICallAddUnitOfWorkTwice()
    {
        _services.AddUnitOfWork<TestUnitOfWork>();
        _result = _services.AddUnitOfWork<TestUnitOfWork>();
    }

    [When(@"I build the UnitOfWork service provider")]
    public void WhenIBuildTheUnitOfWorkServiceProvider()
    {
        _serviceProvider = _services.BuildServiceProvider();
    }

    [Then(@"the same UnitOfWork service collection should be returned")]
    public void ThenTheSameUnitOfWorkServiceCollectionShouldBeReturned()
    {
        Assert.Same(_services, _result);
    }

    [Then(@"IUnitOfWork should be registered with scoped lifetime")]
    public void ThenIUnitOfWorkShouldBeRegisteredWithScopedLifetime()
    {
        var serviceDescriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IUnitOfWork));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
    }

    [Then(@"IUnitOfWork can be resolved as the concrete type")]
    public void ThenIUnitOfWorkCanBeResolvedAsTheConcreteType()
    {
        var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
        Assert.NotNull(unitOfWork);
        Assert.IsType<TestUnitOfWork>(unitOfWork);
    }

    [Then(@"resolving IUnitOfWork throws InvalidOperationException")]
    public void ThenResolvingIUnitOfWorkThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _serviceProvider.GetRequiredService<IUnitOfWork>());
    }

    [Then(@"two IUnitOfWork registrations should exist")]
    public void ThenTwoIUnitOfWorkRegistrationsShouldExist()
    {
        var descriptorCount = _services.Count(sd => sd.ServiceType == typeof(IUnitOfWork));
        Assert.Equal(2, descriptorCount);
    }

    [AfterScenario]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }
}
