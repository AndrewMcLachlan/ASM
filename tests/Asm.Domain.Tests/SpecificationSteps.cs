namespace Asm.Domain.Tests;

[Binding]
public class SpecificationSteps
{
    private IQueryable<TestSpecifiableEntity> _query;
    private IQueryable<TestSpecifiableEntity> _result;

    [Given(@"I have a collection of test entities with IDs \[(.*)\]")]
    public void GivenIHaveACollectionOfTestEntitiesWithIds(string ids)
    {
        var entities = ids.Split(',').Select(id => new TestSpecifiableEntity(Int32.Parse(id.Trim()))).ToList();
        _query = entities.AsQueryable();
    }

    [When(@"I apply a specification that filters IDs greater than (.*)")]
    public void WhenIApplyASpecificationThatFiltersIdsGreaterThan(int threshold)
    {
        var spec = new GreaterThanTwoSpecification();
        _result = _query.Specify(spec);
    }

    [When(@"I apply a null specification")]
    public void WhenIApplyANullSpecification()
    {
        _result = _query.Specify((ISpecification<TestSpecifiableEntity>)null);
    }

    [When(@"I apply the GreaterThanTwoSpecification using type parameter")]
    public void WhenIApplyTheGreaterThanTwoSpecificationUsingTypeParameter()
    {
        _result = _query.Specify<TestSpecifiableEntity, GreaterThanTwoSpecification>();
    }

    [When(@"I apply the greater-than-two AND less-than-five specification")]
    public void WhenIApplyTheGreaterThanTwoAndLessThanFiveSpecification()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().And(new IdLessThanFiveSpecification());
        _result = _query.Specify(spec);
    }

    [When(@"I apply the greater-than-two OR even specification")]
    public void WhenIApplyTheGreaterThanTwoOrEvenSpecification()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Or(new IdIsEvenSpecification());
        _result = _query.Specify(spec);
    }

    [When(@"I apply the NOT greater-than-two specification")]
    public void WhenIApplyTheNotGreaterThanTwoSpecification()
    {
        ISpecification<TestSpecifiableEntity> spec = new IdGreaterThanTwoSpecification().Not();
        _result = _query.Specify(spec);
    }

    [Then(@"the result should contain entities with IDs \[(.*)\]")]
    public void ThenTheResultShouldContainEntitiesWithIds(string expectedIds)
    {
        var expected = expectedIds.Split(',').Select(id => Int32.Parse(id.Trim())).ToList();
        var actual = _result.Select(e => e.Id).ToList();
        Assert.Equal(expected, actual);
    }
}
