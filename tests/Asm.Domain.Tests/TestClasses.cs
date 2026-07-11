using System.Linq.Expressions;

namespace Asm.Domain.Tests;

internal class TestKeyedEntity(int id) : KeyedEntity<int>(id)
{
}

internal class OtherKeyedEntity(int id) : KeyedEntity<int>(id)
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

// Criteria-based specification built on the abstract base class.
internal sealed class IdGreaterThanTwoSpecification : Specification<TestSpecifiableEntity>
{
    public override Expression<Func<TestSpecifiableEntity, bool>> Criteria => e => e.Id > 2;
}

// Criteria-based specification implementing the interface directly (exercises the default Apply/And/Or/Not).
internal sealed class IdLessThanFiveSpecification : ISpecification<TestSpecifiableEntity>
{
    public Expression<Func<TestSpecifiableEntity, bool>> Criteria => e => e.Id < 5;
}

internal sealed class IdIsEvenSpecification : Specification<TestSpecifiableEntity>
{
    public override Expression<Func<TestSpecifiableEntity, bool>> Criteria => e => e.Id % 2 == 0;
}

// A read model / DTO that is NOT an Entity, to prove specifications work over any class.
internal sealed class PersonReadModel
{
    public int Age { get; init; }

    public string Name { get; init; } = String.Empty;
}

internal sealed class AdultSpecification : Specification<PersonReadModel>
{
    public override Expression<Func<PersonReadModel, bool>> Criteria => p => p.Age >= 18;
}

internal sealed class NameStartsWithASpecification : ISpecification<PersonReadModel>
{
    public Expression<Func<PersonReadModel, bool>> Criteria => p => p.Name.StartsWith("A");
}