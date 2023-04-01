using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Domain.Tests;

[Binding]
public class NamedEntityTestsSteps
{
    private readonly ScenarioResult<int> _result;
    private TestNamedEntity _first;
    private TestNamedEntity _second;

    public NamedEntityTestsSteps(ScenarioResult<int> result)
    {
        _result = result;
    }

    [Given(@"I have a named entity with name '(.*)'")]
    public void GivenIHaveANamedEntityWithName(string name)
    {
        _first = new(1) { Name = name };
    }

    [Given(@"I have a second  named entity with name '(.*)'")]
    public void GivenIHaveASecondNamedEntityWithName(string name)
    {
        _second = new(2) { Name = name };
    }

    [When(@"I call first\.CompareTo\(second\)")]
    public void WhenICallFirst_CompareToSecond()
    {
        _result.Value = _first.CompareTo(_second);
    }
}
