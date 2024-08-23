namespace Asm.Domain.Tests;

[Binding]
public class KeyedEntityTestsSteps(ScenarioContext context)
{
    private TestKeyedEntity _first;
    private TestKeyedEntity _second;

    [Given(@"I have a keyed entity with ID (.*)")]
    public void GivenIHaveAKeyedEntityWithID(int? id)
    {
        _first = id == null ? null : new(id.Value);
    }

    [Given(@"I have a second  keyed entity with ID (.*)")]
    public void GivenIHaveASecondKeyedEntityWithID(int? id)
    {
        _second = id == null ? null : new(id.Value);
    }

    [When(@"I call first\.Equals\(second\)")]
    public void WhenICallFirst_EqualsSecond()
    {
        context.AddResult(_first.Equals(_second));
    }

    [When(@"I check equality with ==")]
    public void WhenICheckEqualityWith()
    {
        context.AddResult(_first == _second);
    }

    [When(@"I check inequality with !=")]
    public void WhenICheckInequalityWith()
    {
        context.AddResult(_first != _second);
    }

    [When(@"I check equality with an equality comparer")]
    public void WhenICheckEqualityWithAnEqualityComparer()
    {
        context.AddResult(new KeyedEntityEqualityComparer<TestKeyedEntity, int>().Equals(_first, _second));
    }
}
