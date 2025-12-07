using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class QueryableSteps
{
    private IServiceCollection _services = null!;
    private IServiceCollection _result = null!;

    #region Test Types

    public class QueryableTestEntity
    {
    }

    public class AnotherQueryableTestEntity
    {
    }

    public class QueryableTestDbContext : DbContext, IReadOnlyDbContext
    {
        public QueryableTestDbContext(DbContextOptions<QueryableTestDbContext> options) : base(options)
        {
        }

        public DbSet<QueryableTestEntity> TestEntities { get; set; } = null!;
        public DbSet<AnotherQueryableTestEntity> AnotherEntities { get; set; } = null!;
    }

    #endregion

    [Given(@"I have a service collection for Queryable")]
    public void GivenIHaveAServiceCollectionForQueryable()
    {
        _services = new ServiceCollection();
    }

    [Given(@"I have an empty service collection for Queryable")]
    public void GivenIHaveAnEmptyServiceCollectionForQueryable()
    {
        _services = new ServiceCollection();
    }

    [When(@"I call AddQueryable for an entity type")]
    public void WhenICallAddQueryableForAnEntityType()
    {
        _result = _services.AddQueryable<QueryableTestEntity, QueryableTestDbContext>();
    }

    [When(@"I call AddQueryable for two different entity types")]
    public void WhenICallAddQueryableForTwoDifferentEntityTypes()
    {
        _services.AddQueryable<QueryableTestEntity, QueryableTestDbContext>();
        _result = _services.AddQueryable<AnotherQueryableTestEntity, QueryableTestDbContext>();
    }

    [When(@"I call AddQueryable twice for the same entity type")]
    public void WhenICallAddQueryableTwiceForTheSameEntityType()
    {
        _services.AddQueryable<QueryableTestEntity, QueryableTestDbContext>();
        _result = _services.AddQueryable<QueryableTestEntity, QueryableTestDbContext>();
    }

    [Then(@"the same Queryable service collection should be returned")]
    public void ThenTheSameQueryableServiceCollectionShouldBeReturned()
    {
        Assert.Same(_services, _result);
    }

    [Then(@"IQueryable of the entity should be registered as transient")]
    public void ThenIQueryableOfTheEntityShouldBeRegisteredAsTransient()
    {
        var descriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryable<QueryableTestEntity>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.NotNull(descriptor.ImplementationFactory);
    }

    [Then(@"both IQueryable services should be registered")]
    public void ThenBothIQueryableServicesShouldBeRegistered()
    {
        var testEntityDescriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryable<QueryableTestEntity>));
        var anotherEntityDescriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryable<AnotherQueryableTestEntity>));

        Assert.NotNull(testEntityDescriptor);
        Assert.NotNull(anotherEntityDescriptor);
        Assert.Equal(2, _services.Count(sd => sd.Lifetime == ServiceLifetime.Transient));
    }

    [Then(@"exactly one service descriptor should be added")]
    public void ThenExactlyOneServiceDescriptorShouldBeAdded()
    {
        Assert.Single(_services);
    }

    [Then(@"two IQueryable registrations should exist for the entity")]
    public void ThenTwoIQueryableRegistrationsShouldExistForTheEntity()
    {
        var descriptorCount = _services.Count(sd => sd.ServiceType == typeof(IQueryable<QueryableTestEntity>));
        Assert.Equal(2, descriptorCount);
    }
}
