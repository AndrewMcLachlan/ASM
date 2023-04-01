using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Domain.Tests;

[Binding]
public class KeyedEntityTestsSteps
{
    private readonly ScenarioResult<bool> _result;
    private TestKeyedEntity _first;
    private TestKeyedEntity _second;

    public KeyedEntityTestsSteps(ScenarioResult<bool> result)
    {
        _result = result;
    }


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
        _result.Value = _first.Equals(_second);
    }

    [When(@"I check equality with ==")]
    public void WhenICheckEqualityWith()
    {
        _result.Value = _first == _second;
    }

    [When(@"I check inequality with !=")]
    public void WhenICheckInequalityWith()
    {
        _result.Value = _first != _second;
    }

    [When(@"I check equality with an equality comparer")]
    public void WhenICheckEqualityWithAnEqualityComparer()
    {
        _result.Value = new KeyedEntityEqualityComparer<TestKeyedEntity, int>().Equals(_first, _second);
    }
}
