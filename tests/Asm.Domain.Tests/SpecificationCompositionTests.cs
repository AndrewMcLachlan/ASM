using System.Linq.Expressions;

namespace Asm.Domain.Tests;

public class SpecificationCompositionTests
{
    private static IQueryable<TestSpecifiableEntity> Entities(params int[] ids) =>
        ids.Select(id => new TestSpecifiableEntity(id)).AsQueryable();

    /// <summary>
    /// Given two specifications combined with And
    /// When applied to a set of entities
    /// Then only entities satisfying both criteria are returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AndMatchesEntitiesSatisfyingBothCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().And(new IdLessThanFiveSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5, 6)).Select(e => e.Id).ToList();

        // > 2 AND < 5 => 3, 4
        Assert.Equal([3, 4], result);
    }

    /// <summary>
    /// Given two specifications combined with Or
    /// When applied to a set of entities
    /// Then entities satisfying either criterion are returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void OrMatchesEntitiesSatisfyingEitherCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Or(new IdIsEvenSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5)).Select(e => e.Id).ToList();

        // > 2 OR even => 2, 3, 4, 5
        Assert.Equal([2, 3, 4, 5], result);
    }

    /// <summary>
    /// Given a specification negated with Not
    /// When applied to a set of entities
    /// Then only entities failing the original criterion are returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NotNegatesTheCriteria()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Not();

        var result = spec.Apply(Entities(1, 2, 3, 4)).Select(e => e.Id).ToList();

        // NOT (> 2) => 1, 2
        Assert.Equal([1, 2], result);
    }

    /// <summary>
    /// Given specifications chained with And then Or
    /// When applied to a set of entities
    /// Then the combined criteria are evaluated correctly
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CompositionCanBeChained()
    {
        // (> 2 AND < 5) OR even
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification()
            .And(new IdLessThanFiveSpecification())
            .Or(new IdIsEvenSpecification());

        var result = spec.Apply(Entities(1, 2, 3, 4, 5, 6)).Select(e => e.Id).ToList();

        // (3,4) OR (2,4,6) => 2, 3, 4, 6
        Assert.Equal([2, 3, 4, 6], result);
    }

    /// <summary>
    /// Given two specifications combined with And
    /// When the combined criteria expression is inspected
    /// Then it is a single AndAlso binary expression with one parameter and no Invoke node
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AndCombinedCriteriaIsASingleTranslatableExpression()
    {
        // A hallmark of correct parameter rebinding: the combined body is a BinaryExpression (AndAlso),
        // not an InvocationExpression that most query providers cannot translate.
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().And(new IdLessThanFiveSpecification());

        Expression<Func<TestSpecifiableEntity, bool>> criteria = spec.Criteria;

        Assert.Single(criteria.Parameters);
        Assert.Equal(System.Linq.Expressions.ExpressionType.AndAlso, criteria.Body.NodeType);
        Assert.DoesNotContain(criteria.Body.ToString(), "Invoke");
    }

    /// <summary>
    /// Given a specification over a non-entity read model
    /// When applied to a set of read models
    /// Then only matching read models are returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SpecificationOverNonEntityReadModelFilters()
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

    /// <summary>
    /// Given two specifications over a non-entity read model combined with And
    /// When applied to a set of read models
    /// Then only read models satisfying both criteria are returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CompositionOverNonEntityReadModelFilters()
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

    /// <summary>
    /// Given a specification that overrides only Apply, leaving the default Criteria
    /// When the default Criteria is compiled and evaluated
    /// Then it matches every entity
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void DefaultCriteriaMatchesEverything()
    {
        // GreaterThanTwoSpecification only overrides Apply, so its Criteria is the default (match all).
        ISpecification<TestSpecifiableEntity> spec = new GreaterThanTwoSpecification();
        var result = spec.Criteria.Compile();

        Assert.True(result(new TestSpecifiableEntity(1)));
        Assert.True(result(new TestSpecifiableEntity(99)));
    }

    /// <summary>
    /// Given a specification
    /// When And is called with a null specification
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AndNullSpecificationThrows()
    {
        var spec = new IdGreaterThanTwoSpecification();

        Assert.Throws<ArgumentNullException>(() => spec.And(null!));
    }
}
