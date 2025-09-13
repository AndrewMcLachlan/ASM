using Asm.Drawing;

namespace Asm.Tests;

[Binding]
[Scope(Feature = "HexColour")]
public class HexColourSteps(ScenarioContext context)
{
    private string _hexColour = null;
    private uint _uintColour;
    private HexColour _result;

    [Given("I have a string {string}")]
    public void GivenIHaveAString(string hexColour)
    {
        _hexColour = hexColour;
    }
    
    [Given("I have a uint {int}")]
    public void GivenIHaveAUint(int uintColour)
    {
        _uintColour = (uint)uintColour;
    }

    [When("I create a new HexColour from the uint")]
    public void WhenICreateANewHexColourFromTheUint()
    {
        context.CatchException(() => _result = new HexColour(_uintColour));
    }

    [When("I create a new HexColour from the string")]
    public void WhenICreateANewHexColourFromTheString()
    {
        context.CatchException(() => _result = new HexColour(_hexColour));
    }

    [When("I parse a new HexColour from the string")]
    public void WhenIParseANewHexColourFromTheString()
    {
        context.CatchException(() => _result = HexColour.Parse(_hexColour));
    }

    [When("I try parse a new HexColour from the string")]
    public void WhenITryParseANewHexColourFromTheString()
    {
        context.AddResult(HexColour.TryParse(_hexColour, out _result));
    }


    [Then("the result should be a HexColour with value {string}")]
    public void ThenTheResultShouldBeAHexColourWithValue(string expected)
    {
        Assert.Equal(expected, _result.HexString);
    }
}
