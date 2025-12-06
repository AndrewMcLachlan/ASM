using System.Reflection;
using Asm.Domain;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class AggregateRootsSteps(ScenarioContext context)
{
    private IServiceCollection _services = null!;
    private IServiceCollection _result = null!;
    private Assembly _assembly = null!;

    #region Test Types

    [AggregateRoot]
    public class TestAggregateRoot
    {
    }

    [AggregateRoot]
    public class SecondTestAggregateRoot
    {
    }

    public class NonAggregateEntity
    {
    }

    #endregion

    [Given(@"I have a service collection for AggregateRoots")]
    public void GivenIHaveAServiceCollectionForAggregateRoots()
    {
        _services = new ServiceCollection();
    }

    [Given(@"I have a null service collection for AggregateRoots")]
    public void GivenIHaveANullServiceCollectionForAggregateRoots()
    {
        _services = null!;
    }

    [Given(@"I have a null entity assembly")]
    public void GivenIHaveANullEntityAssembly()
    {
        _assembly = null!;
    }

    [Given(@"I have an assembly with no aggregate root types")]
    public void GivenIHaveAnAssemblyWithNoAggregateRootTypes()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes()).Returns([typeof(NonAggregateEntity)]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an empty entity assembly")]
    public void GivenIHaveAnEmptyEntityAssembly()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes()).Returns(Array.Empty<Type>());
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with one aggregate root type")]
    public void GivenIHaveAnAssemblyWithOneAggregateRootType()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes()).Returns([typeof(TestAggregateRoot)]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with two aggregate root types")]
    public void GivenIHaveAnAssemblyWithTwoAggregateRootTypes()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes())
            .Returns([typeof(TestAggregateRoot), typeof(SecondTestAggregateRoot)]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with mixed aggregate and non-aggregate types")]
    public void GivenIHaveAnAssemblyWithMixedAggregateAndNonAggregateTypes()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes())
            .Returns([typeof(TestAggregateRoot), typeof(NonAggregateEntity), typeof(SecondTestAggregateRoot)]);
        _assembly = mockAssembly.Object;
    }

    [When(@"I call AddAggregateRoots with the null assembly")]
    public void WhenICallAddAggregateRootsWithTheNullAssembly()
    {
        context.CatchException(() => _result = _services.AddAggregateRoots<IReadOnlyDbContext>(_assembly));
    }

    [When(@"I call AddAggregateRoots")]
    public void WhenICallAddAggregateRoots()
    {
        _result = _services.AddAggregateRoots<IReadOnlyDbContext>(_assembly);
    }

    [When(@"I call AddAggregateRoots on the null collection")]
    public void WhenICallAddAggregateRootsOnTheNullCollection()
    {
        context.CatchException(() => _result = _services.AddAggregateRoots<IReadOnlyDbContext>(_assembly));
    }

    [When(@"I call AddAggregateRoots on the null collection without exception")]
    public void WhenICallAddAggregateRootsOnTheNullCollectionWithoutException()
    {
        _result = _services.AddAggregateRoots<IReadOnlyDbContext>(_assembly);
    }

    [Then(@"no services should be registered")]
    public void ThenNoServicesShouldBeRegistered()
    {
        Assert.Empty(_services);
    }

    [Then(@"one service descriptor should be added")]
    public void ThenOneServiceDescriptorShouldBeAdded()
    {
        Assert.Single(_services);
    }

    [Then(@"two service descriptors should be added")]
    public void ThenTwoServiceDescriptorsShouldBeAdded()
    {
        Assert.Equal(2, _services.Count);
    }

    [Then(@"only aggregate root types should be registered")]
    public void ThenOnlyAggregateRootTypesShouldBeRegistered()
    {
        Assert.Equal(2, _services.Count);

        var serviceTypes = _services.Select(d => d.ServiceType).ToList();
        Assert.Contains(typeof(IQueryable<TestAggregateRoot>), serviceTypes);
        Assert.Contains(typeof(IQueryable<SecondTestAggregateRoot>), serviceTypes);
    }

    [Then(@"the service type should be IQueryable of the aggregate root")]
    public void ThenTheServiceTypeShouldBeIQueryableOfTheAggregateRoot()
    {
        var descriptor = _services.First();
        Assert.Equal(typeof(IQueryable<TestAggregateRoot>), descriptor.ServiceType);
    }

    [Then(@"the service should have transient lifetime")]
    public void ThenTheServiceShouldHaveTransientLifetime()
    {
        var descriptor = _services.First();
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Then(@"the same AggregateRoots service collection should be returned")]
    public void ThenTheSameAggregateRootsServiceCollectionShouldBeReturned()
    {
        Assert.Same(_services, _result);
    }

    [Then(@"the service should use a factory implementation")]
    public void ThenTheServiceShouldUseAFactoryImplementation()
    {
        var descriptor = _services.First();
        Assert.NotNull(descriptor.ImplementationFactory);
        Assert.Null(descriptor.ImplementationType);
        Assert.Null(descriptor.ImplementationInstance);
    }

    [Then(@"the AggregateRoots result should be null")]
    public void ThenTheAggregateRootsResultShouldBeNull()
    {
        Assert.Null(_result);
    }
}
