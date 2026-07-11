using System.Reflection;
using Asm.Domain;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

/// <summary>
/// Tests that <c>AddDomainEvents</c> registers handlers under every phase contract they implement.
/// </summary>
public class TwoPhaseDomainEventRegistrationTests
{
    #region Test Types

    private sealed record SampleEvent : IDomainEvent;

    private sealed class PreSaveHandler : IPreSaveDomainEventHandler<SampleEvent>
    {
        public ValueTask Handle(SampleEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    private sealed class PostSaveHandler : IPostSaveDomainEventHandler<SampleEvent>
    {
        public ValueTask Handle(SampleEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    private sealed class BothHandler : IPreSaveDomainEventHandler<SampleEvent>, IPostSaveDomainEventHandler<SampleEvent>
    {
        public ValueTask Handle(SampleEvent domainEvent, CancellationToken cancellationToken = default)
            => ValueTask.CompletedTask;
    }

    #endregion

    private static IServiceCollection Register(params Type[] handlerTypes)
    {
        var services = new ServiceCollection();
        var assembly = new Mock<Assembly>();
        assembly.Setup(a => a.DefinedTypes).Returns(handlerTypes.Select(t => t.GetTypeInfo()).ToList());
        return services.AddDomainEvents(assembly.Object);
    }

    /// <summary>
    /// Given a pre-save handler,
    /// when AddDomainEvents scans it,
    /// then it is registered under both the pre-save contract and the base contract the pre-save phase resolves.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void PreSaveHandlerRegisteredUnderPreSaveAndBaseContracts()
    {
        var services = Register(typeof(PreSaveHandler));

        Assert.Contains(services, sd => sd.ServiceType == typeof(IDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(PreSaveHandler));
        Assert.Contains(services, sd => sd.ServiceType == typeof(IPreSaveDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(PreSaveHandler));
        Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IPostSaveDomainEventHandler<SampleEvent>));
    }

    /// <summary>
    /// Given a post-save handler,
    /// when AddDomainEvents scans it,
    /// then it is registered under the post-save contract only and not under the pre-save contract.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void PostSaveHandlerRegisteredUnderPostSaveContractOnly()
    {
        var services = Register(typeof(PostSaveHandler));

        Assert.Contains(services, sd => sd.ServiceType == typeof(IPostSaveDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(PostSaveHandler));
        Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IDomainEventHandler<SampleEvent>));
    }

    /// <summary>
    /// Given a handler implementing both phase interfaces,
    /// when AddDomainEvents scans it,
    /// then it is registered under all three contracts.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BothHandlerRegisteredUnderAllContracts()
    {
        var services = Register(typeof(BothHandler));

        Assert.Contains(services, sd => sd.ServiceType == typeof(IDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(BothHandler));
        Assert.Contains(services, sd => sd.ServiceType == typeof(IPreSaveDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(BothHandler));
        Assert.Contains(services, sd => sd.ServiceType == typeof(IPostSaveDomainEventHandler<SampleEvent>) && sd.ImplementationType == typeof(BothHandler));
    }
}
