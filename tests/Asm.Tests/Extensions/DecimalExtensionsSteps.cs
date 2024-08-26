namespace Asm.Tests.Extensions;

[Binding]
public class DecimalExtensionsSteps(ScenarioContext context)
{
    private decimal _input;
    private int? _places;

    [Given(@"I have a decimal (.*)")]
    public void GivenIHaveADecimal(decimal value)
    {
        _input = value;
    }

    [Given(@"I want to round to (.*) decimal places")]
    public void GivenIWantToRoundToDecimalPlaces(int value)
    {
        _places = value;
    }

    [When(@"I call ToRoundedCurrencyString")]
    public void WhenICallToRoundedCurrencyString()
    {
        context.AddResult(_places != null ? _input.ToRoundedCurrencyString(_places.Value) : _input.ToRoundedCurrencyString());
    }

    [When(@"I call ToRoundedCurrencyString expecting an exception")]
    public void WhenICallToRoundedCurrencyStringExpectingAnException()
    {
        context.CatchException(() => _input.ToRoundedCurrencyString(_places.Value));
    }

}
