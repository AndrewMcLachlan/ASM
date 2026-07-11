using System.Linq.Expressions;

namespace Asm.Domain.Tests;

public class SpecificationCompositionTests
{
    private static IQueryable<TestSpecifiableEntity> Entities(params int[] ids) =>
        ids.Select(id => new TestSpecifiableEntity(id)).AsQueryable();

    [Fact]
    [Trait("Category", "Unit")]
    public void And_MatchesEntitiesSatisfyingBothCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().And(new IdLessThanFiveSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5, 6)).Select(e => e.Id).ToList();

        // > 2 AND < 5 => 3, 4
        Assert.Equal([3, 4], result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Or_MatchesEntitiesSatisfyingEitherCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Or(new IdIsEvenSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5)).Select(e => e.Id).ToList();

        // > 2 OR even => 2, 3, 4, 5
        Assert.Equal([2, 3, 4, 5], result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Not_NegatesTheCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Not();

        var result = spec.Apply(Entities(1, 2, 3, 4)).Select(e => e.Id).ToList();

        // NOT (> 2) => 1, 2
        Assert.Equal([1, 2], result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Composition_CanBeChained()
    {
        // (> 2 AND < 5) OR even
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification()
            .And(new IdLessThanFiveSpecification())
            .Or(new IdIsEvenSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5, 6)).Select(e => e.Id).ToList();

        // (3,4) OR (2,4,6) => 2, 3, 4, 6
        Assert.Equal([2, 3, 4, 6], result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void And_CombinedCriteria_IsASingleTranslatableExpression()
    {
        // A hallmark of correct parameter rebinding: the combined body is a BinaryExpression (AndAlso),
        // not an InvocationExpression that most query providers cannot translate.
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().And(new IdLessThanFiveSpecification());

        Expression<Func<TestSpecifiableEntity, bool>> criteria = spec.Criteria;

        Assert.Single(criteria.Parameters);
        Assert.Equal(System.Linq.Expressions.ExpressionType.AndAlso, criteria.Body.NodeType);
        Assert.DoesNotContain(criteria.Body.ToString(), "Invoke");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Specification_OverNonEntityReadModel_Filters()
    {
        var people = new[]
        {
            new PersonReadModel { Name = "Alice", Age = 30 },
            new PersonReadModel { Name = "Bob", Age = 12 },
            new PersonReadModel { Name = "Amy", Age = 17 },
        }.AsQueryable();

        var adults = new AdultSpecification().Apply(people).Select(p => p.Name).ToList();

        Assert.Equal(["Alice"], adults);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Composition_OverNonEntityReadModel_Filters()
    {
        var people = new[]
        {
            new PersonReadModel { Name = "Alice", Age = 30 },
            new PersonReadModel { Name = "Bob", Age = 40 },
            new PersonReadModel { Name = "Amy", Age = 17 },
        }.AsQueryable();

        // Adult AND name starts with 'A'
        ISpecification<PersonReadModel> spec = new AdultSpecification().And(new NameStartsWithASpecification());

        var result = spec.Apply(people).Select(p => p.Name).ToList();

        Assert.Equal(["Alice"], result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DefaultCriteria_MatchesEverything()
    {
        // GreaterThanTwoSpecification only overrides Apply, so its Criteria is the default (match all).
        ISpecification<TestSpecifiableEntity> spec = new GreaterThanTwoSpecification();
        var result = spec.Criteria.Compile();

        Assert.True(result(new TestSpecifiableEntity(1)));
        Assert.True(result(new TestSpecifiableEntity(99)));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void And_NullSpecification_Throws()
    {
        var spec = new IdGreaterThanTwoSpecification();

        Assert.Throws<ArgumentNullException>(() => spec.And(null!));
    }
}
