using System.Text.Json;
using Asm.Drawing;

namespace Asm.Tests;

[Binding]
[Scope(Feature = "HexColour")]
public class HexColourSteps(ScenarioContext context)
{
    private string _hexColour = null;
    private uint _uintColour;
    private HexColour _result;
    private HexColour _result2;
    private string _jsonResult;
    private string _jsonInput;
    private object _conversionResult;
    private string _conversionResultString;
    private bool _conversionResultBool;

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

    [Given("I have a HexColour with value {string}")]
    public void GivenIHaveAHexColourWithValue(string hexValue)
    {
        _result = new HexColour(hexValue);
    }

    [Given("I have another HexColour with value {string}")]
    public void GivenIHaveAnotherHexColourWithValue(string hexValue)
    {
        context.CatchException(() => _result2 = HexColour.Parse(hexValue));
    }

    [Given("I have a JSON string {string}")]
    public void GivenIHaveAJsonString(string json)
    {
        _jsonInput = json;
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

    [When("I serialize the HexColour to JSON")]
    public void WhenISerializeTheHexColourToJson()
    {
        _jsonResult = JsonSerializer.Serialize(_result);
    }

    [When("I deserialize the JSON to HexColour")]
    public void WhenIDeserializeTheJsonToHexColour()
    {
        _result = JsonSerializer.Deserialize<HexColour>(_jsonInput);
    }

    [When("I get the HexString")]
    public void WhenIGetTheHexString()
    {
        _conversionResultString = _result.HexString;
    }

    [When("I convert to string")]
    public void WhenIConvertToString()
    {
        context.CatchException(() => _conversionResultString = _result.ToString());
    }

    [When("I convert to UInt32 implicitly")]
    public void WhenIConvertToUInt32Implicitly()
    {
        context.CatchException(() =>
        {
            uint value = (uint)_result;
            _conversionResult = value;
        });
    }

    [When("I convert to HexColour from UInt32 explicitly")]
    public void WhenIConvertToHexColourFromUInt32Explicitly()
    {
        context.CatchException(() =>
        {
            HexColour hc = (HexColour)_uintColour;
            _conversionResult = hc;
        });
    }

    [When("I check equality with another HexColour")]
    public void WhenICheckEqualityWithAnotherHexColour()
    {
        _conversionResultBool = _result == _result2;
    }

    [When("I check inequality with another HexColour")]
    public void WhenICheckInequalityWithAnotherHexColour()
    {
        _conversionResultBool = _result != _result2;
    }

    [When("I check equality with object")]
    public void WhenICheckEqualityWithObject()
    {
        _conversionResultBool = _result.Equals((object)_result2);
    }

    [When("I check equality with null object")]
    public void WhenICheckEqualityWithNullObject()
    {
        _conversionResultBool = _result.Equals((object)null);
    }

    [When("I check equality with another HexColour value")]
    public void WhenICheckEqualityWithAnotherHexColourValue()
    {
        _conversionResultBool = _result.Equals(_result2);
    }

    [When("I get the hash code")]
    public void WhenIGetTheHashCode()
    {
        _conversionResult = _result.GetHashCode();
    }

    [When("I convert HexColour to string implicitly")]
    public void WhenIConvertHexColourToStringImplicitly()
    {
        string value = (string)_result;
        _conversionResultString = value;
    }

    [When("I get the Value property")]
    public void WhenIGetTheValueProperty()
    {
        _conversionResult = _result.Value;
    }

    [Then("the result should be a HexColour with value {string}")]
    public void ThenTheResultShouldBeAHexColourWithValue(string expected)
    {
        Assert.Equal(expected, _result.HexString);
    }

    [Then("the JSON should be {string}")]
    public void ThenTheJsonShouldBe(string expected)
    {
        Assert.Equal(expected, _jsonResult);
    }

    [Then("the equality check should be (.*)")]
    public void ThenTheEqualityCheckShouldBe(bool expected)
    {
        Assert.Equal(expected, _conversionResultBool);
    }

    [Then("the result should not be null")]
    public void ThenTheResultShouldNotBeNull()
    {
        Assert.NotNull(_conversionResult);
    }

    [Then("the string result should not be null")]
    public void ThenTheStringResultShouldNotBeNull()
    {
        Assert.NotNull(_conversionResultString);
    }

    [Then("the result should be a valid hash code")]
    public void ThenTheResultShouldBeAValidHashCode()
    {
        Assert.IsType<int>(_conversionResult);
    }

    [Then("the string representation should contain hex digits")]
    public void ThenTheStringRepresentationShouldContainHexDigits()
    {
        Assert.Matches(@"[0-9a-fA-F]+", _conversionResultString);
    }

    [Then("the boolean value true is returned")]
    public void ThenTheBooleanValueTrueIsReturned()
    {
        Assert.True(context.GetResult<bool>());
    }

    [Then("the boolean value false is returned")]
    public void ThenTheBooleanValueFalseIsReturned()
    {
        Assert.False(context.GetResult<bool>());
    }
}
