namespace Asm.Tests.Extensions;

[Binding]
public class BooleanExtensionsSteps(ScenarioContext context)
{
    private bool _input;

    [Given(@"I have boolean that is '(.*)'")]
    public void GivenIHaveBooleanThatIs(bool value)
    {
        _input = value;
    }

    [When(@"I call ToNumeric")]
    public void WhenICallToNumeric()
    {
        context.AddResult(_input.ToNumeric());
    }

    [When(@"I call ToNumericString")]
    public void WhenICallToNumericString()
    {
        context.AddResult(_input.ToNumericString());
    }
}
