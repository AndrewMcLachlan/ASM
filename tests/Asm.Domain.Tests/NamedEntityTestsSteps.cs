namespace Asm.Domain.Tests;

[Binding]
public class NamedEntityTestsSteps(ScenarioContext context)
{
    private TestNamedEntity _first;
    private TestNamedEntity _second;

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
        context.AddResult(_first.CompareTo(_second));
    }
}
