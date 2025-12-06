namespace Asm.Domain.Tests;

internal class TestKeyedEntity(int id) : KeyedEntity<int>(id)
{
}

internal class TestNamedEntity(int id) : NamedEntity<int>(id)
{
}

internal class TestEntity : Entity
{
}

internal class TestDomainEvent : IDomainEvent
{
}

internal class TestSpecifiableEntity(int id) : Entity
{
    public int Id { get; } = id;
}

internal class GreaterThanTwoSpecification : ISpecification<TestSpecifiableEntity>
{
    public IQueryable<TestSpecifiableEntity> Apply(IQueryable<TestSpecifiableEntity> query)
        => query.Where(e => e.Id > 2);
}