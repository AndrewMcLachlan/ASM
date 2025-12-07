using System.Reflection;
using Asm.Domain;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class DomainEventRegistrationSteps(ScenarioContext context)
{
    private IServiceCollection _services = null!;
    private IServiceCollection _result = null!;
    private Assembly _assembly = null!;
    private Assembly _assembly2 = null!;
    private ServiceProvider _serviceProvider = null!;

    #region Test Types

    public class TestDomainEvent : IDomainEvent { }

    public class TestDomainEvent2 : IDomainEvent { }

    public class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public ValueTask Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    public abstract class AbstractTestHandler : IDomainEventHandler<TestDomainEvent>
    {
        public abstract ValueTask Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }

    public class GenericTestHandler<T> : IDomainEventHandler<TestDomainEvent>
    {
        public ValueTask Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    public class NonHandlerClass { }

    public class MultiDomainEventHandler : IDomainEventHandler<TestDomainEvent>, IDomainEventHandler<TestDomainEvent2>
    {
        public ValueTask Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;

        public ValueTask Handle(TestDomainEvent2 domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    public class SecondTestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public ValueTask Handle(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    public class AnotherDomainEvent : IDomainEvent { }

    public class AnotherDomainEventHandler : IDomainEventHandler<AnotherDomainEvent>
    {
        public ValueTask Handle(AnotherDomainEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    #endregion

    [Given(@"I have a service collection")]
    public void GivenIHaveAServiceCollection()
    {
        _services = new ServiceCollection();
    }

    [Given(@"I have a null service collection")]
    public void GivenIHaveANullServiceCollection()
    {
        _services = null!;
    }

    [Given(@"I have a service collection with IPublisher already registered as singleton")]
    public void GivenIHaveAServiceCollectionWithIPublisherAlreadyRegisteredAsSingleton()
    {
        _services = new ServiceCollection();
        var mockPublisher = new Mock<IPublisher>();
        _services.AddSingleton(mockPublisher.Object);
    }

    [Given(@"I have an assembly with a valid domain event handler")]
    public void GivenIHaveAnAssemblyWithAValidDomainEventHandler()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns([typeof(TestDomainEventHandler).GetTypeInfo()]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with an abstract domain event handler")]
    public void GivenIHaveAnAssemblyWithAnAbstractDomainEventHandler()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns([typeof(AbstractTestHandler).GetTypeInfo()]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with a non-handler class")]
    public void GivenIHaveAnAssemblyWithANonHandlerClass()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns([typeof(NonHandlerClass).GetTypeInfo()]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an assembly with a multi-interface handler")]
    public void GivenIHaveAnAssemblyWithAMultiInterfaceHandler()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns([typeof(MultiDomainEventHandler).GetTypeInfo()]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have an empty assembly")]
    public void GivenIHaveAnEmptyAssembly()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns(new List<TypeInfo>());
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have a null assembly")]
    public void GivenIHaveANullAssembly()
    {
        _assembly = null!;
    }

    [Given(@"I have an assembly with mixed valid and invalid types")]
    public void GivenIHaveAnAssemblyWithMixedValidAndInvalidTypes()
    {
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.DefinedTypes)
            .Returns([
                typeof(TestDomainEventHandler).GetTypeInfo(),
                typeof(AbstractTestHandler).GetTypeInfo(),
                typeof(GenericTestHandler<>).GetTypeInfo(),
                typeof(NonHandlerClass).GetTypeInfo(),
                typeof(IDomainEvent).GetTypeInfo()
            ]);
        _assembly = mockAssembly.Object;
    }

    [Given(@"I have two assemblies with different handlers")]
    public void GivenIHaveTwoAssembliesWithDifferentHandlers()
    {
        var mockAssembly1 = new Mock<Assembly>();
        mockAssembly1.Setup(a => a.DefinedTypes)
            .Returns([typeof(TestDomainEventHandler).GetTypeInfo()]);
        _assembly = mockAssembly1.Object;

        var mockAssembly2 = new Mock<Assembly>();
        mockAssembly2.Setup(a => a.DefinedTypes)
            .Returns([typeof(MultiDomainEventHandler).GetTypeInfo()]);
        _assembly2 = mockAssembly2.Object;
    }

    [Given(@"I call AddDomainEvent for a specific handler")]
    public void GivenICallAddDomainEventForASpecificHandler()
    {
        _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>();
    }

    [When(@"I call AddDomainEvents with the assembly")]
    public void WhenICallAddDomainEventsWithTheAssembly()
    {
        _result = _services.AddDomainEvents(_assembly);
    }

    [When(@"I call AddDomainEvents with the null assembly")]
    public void WhenICallAddDomainEventsWithTheNullAssembly()
    {
        context.CatchException(() => _result = _services.AddDomainEvents(_assembly));
    }

    [When(@"I call AddDomainEvents with both assemblies")]
    public void WhenICallAddDomainEventsWithBothAssemblies()
    {
        _services.AddDomainEvents(_assembly);
        _result = _services.AddDomainEvents(_assembly2);
    }

    [When(@"I call AddDomainEvent for a specific handler")]
    public void WhenICallAddDomainEventForASpecificHandler()
    {
        _result = _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>();
    }

    [When(@"I call AddDomainEvent on the null collection")]
    public void WhenICallAddDomainEventOnTheNullCollection()
    {
        context.CatchException(() => _result = _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>());
    }

    [When(@"I call AddDomainEvent twice for the same handler")]
    public void WhenICallAddDomainEventTwiceForTheSameHandler()
    {
        _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>();
        _result = _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>();
    }

    [When(@"I call AddDomainEvent for two different handlers")]
    public void WhenICallAddDomainEventForTwoDifferentHandlers()
    {
        _services.AddDomainEvent<TestDomainEventHandler, TestDomainEvent>();
        _result = _services.AddDomainEvent<AnotherDomainEventHandler, AnotherDomainEvent>();
    }

    [When(@"I build the service provider")]
    public void WhenIBuildTheServiceProvider()
    {
        _serviceProvider = _services.BuildServiceProvider();
    }

    [Then(@"the handler should be registered as transient for the interface type")]
    public void ThenTheHandlerShouldBeRegisteredAsTransientForTheInterfaceType()
    {
        var descriptor = _services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>) &&
            sd.ImplementationType == typeof(TestDomainEventHandler));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Then(@"no handler should be registered")]
    public void ThenNoHandlerShouldBeRegistered()
    {
        var handlerDescriptors = _services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)).ToList();

        Assert.Empty(handlerDescriptors);
    }

    [Then(@"both handler interfaces should be registered")]
    public void ThenBothHandlerInterfacesShouldBeRegistered()
    {
        var descriptor1 = _services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>) &&
            sd.ImplementationType == typeof(MultiDomainEventHandler));

        var descriptor2 = _services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IDomainEventHandler<TestDomainEvent2>) &&
            sd.ImplementationType == typeof(MultiDomainEventHandler));

        Assert.NotNull(descriptor1);
        Assert.NotNull(descriptor2);
    }

    [Then(@"IPublisher should be registered as transient")]
    public void ThenIPublisherShouldBeRegisteredAsTransient()
    {
        var descriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IPublisher));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Then(@"IPublisher should be registered with Publisher implementation")]
    public void ThenIPublisherShouldBeRegisteredWithPublisherImplementation()
    {
        var descriptor = _services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IPublisher) &&
            sd.ImplementationType == typeof(Publisher));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Then(@"the same service collection instance should be returned")]
    public void ThenTheSameServiceCollectionInstanceShouldBeReturned()
    {
        Assert.Same(_services, _result);
    }

    [Then(@"only one IPublisher registration should exist")]
    public void ThenOnlyOneIPublisherRegistrationShouldExist()
    {
        var publisherDescriptors = _services.Where(sd => sd.ServiceType == typeof(IPublisher)).ToList();
        Assert.Single(publisherDescriptors);
    }

    [Then(@"the IPublisher should be singleton")]
    public void ThenTheIPublisherShouldBeSingleton()
    {
        var publisherDescriptor = _services.First(sd => sd.ServiceType == typeof(IPublisher));
        Assert.Equal(ServiceLifetime.Singleton, publisherDescriptor.Lifetime);
    }

    [Then(@"only the valid handler should be registered")]
    public void ThenOnlyTheValidHandlerShouldBeRegistered()
    {
        var handlerDescriptors = _services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)).ToList();

        Assert.Single(handlerDescriptors);
        Assert.Equal(typeof(TestDomainEventHandler), handlerDescriptors[0].ImplementationType);
    }

    [Then(@"both handlers should be registered")]
    public void ThenBothHandlersShouldBeRegistered()
    {
        var testEventHandlers = _services.Where(sd =>
            sd.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>)).ToList();

        Assert.Equal(2, testEventHandlers.Count);
    }

    [Then(@"an exception is thrown")]
    public void ThenAnExceptionIsThrown()
    {
        var ex = context.GetException();
        Assert.NotNull(ex);
    }

    [Then(@"one handler registration should exist")]
    public void ThenTwoHandlerRegistrationsShouldExist()
    {
        var handlerCount = _services.Count(d =>
            d.ServiceType == typeof(IDomainEventHandler<TestDomainEvent>) &&
            d.ImplementationType == typeof(TestDomainEventHandler));

        Assert.Equal(1, handlerCount);
    }

    [Then(@"IAppCache should be registered")]
    public void ThenIAppCacheShouldBeRegistered()
    {
        var lazyCacheDescriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IAppCache));
        Assert.NotNull(lazyCacheDescriptor);
    }

    [Then(@"the handler can be resolved")]
    public void ThenTheHandlerCanBeResolved()
    {
        var handler = _serviceProvider.GetService<IDomainEventHandler<TestDomainEvent>>();
        Assert.NotNull(handler);
        Assert.IsType<TestDomainEventHandler>(handler);
    }

    [AfterScenario]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }
}
