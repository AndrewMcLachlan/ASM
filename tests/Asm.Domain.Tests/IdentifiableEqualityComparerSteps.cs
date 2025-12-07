using Asm.Testing;

namespace Asm.Domain.Tests;

[Binding]
public class IdentifiableEqualityComparerSteps(ScenarioContext context)
{
    private TestKeyedEntity _first;
    private TestKeyedEntity _second;
    private IIdentifiableEqualityComparer<TestKeyedEntity, int> _comparer = new();
    private int _hashCode;

    [Given(@"I have an identifiable entity with ID (.*)")]
    public void GivenIHaveAnIdentifiableEntityWithId(int? id)
    {
        _first = id == null ? null : new TestKeyedEntity(id.Value);
    }

    [Given(@"I have a second identifiable entity with ID (.*)")]
    public void GivenIHaveASecondIdentifiableEntityWithId(int? id)
    {
        _second = id == null ? null : new TestKeyedEntity(id.Value);
    }

    [When(@"I check equality with IIdentifiableEqualityComparer")]
    public void WhenICheckEqualityWithIIdentifiableEqualityComparer()
    {
        context.AddResult(_comparer.Equals(_first, _second));
    }

    [When(@"I get the hash code using IIdentifiableEqualityComparer")]
    public void WhenIGetTheHashCodeUsingIIdentifiableEqualityComparer()
    {
        _hashCode = _comparer.GetHashCode(_first);
    }

    [When(@"I get the hash code of null using IIdentifiableEqualityComparer")]
    public void WhenIGetTheHashCodeOfNullUsingIIdentifiableEqualityComparer()
    {
        context.CatchException(() => _comparer.GetHashCode(null!));
    }

    [Then(@"the hash code should equal the ID hash code")]
    public void ThenTheHashCodeShouldEqualTheIdHashCode()
    {
        Assert.Equal(_first.Id.GetHashCode(), _hashCode);
    }
}
