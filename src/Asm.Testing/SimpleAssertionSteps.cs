using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Testing;

[Binding]
public class SimpleAssertionSteps
{
    private readonly ScenarioResult<string> _strResult;
    private readonly ScenarioResult<int> _intResult;
    private readonly ScenarioResult<DateTime> _dateResult;
    private readonly ScenarioResult<bool> _boolResult;

    public SimpleAssertionSteps(ScenarioResult<string> strResult, ScenarioResult<int> intResult, ScenarioResult<DateTime> dateResult, ScenarioResult<bool> boolResult)
    {
        _strResult = strResult;
        _intResult = intResult;
        _dateResult = dateResult;
        _boolResult = boolResult;
    }

    [Then(@"the string value '(.*)' is returned")]
    public void ThenTheStringValueIsReturned(string expected)
    {
        Assert.NotNull(_intResult);
        Assert.Equal(expected, _strResult.Value);
    }

    [Then(@"the integer value (.*) is returned")]
    public void ThenTheIntegerValueIsReturned(int expected)
    {
        Assert.NotNull(_intResult);
        Assert.Equal(expected, _intResult.Value);
    }


    [Then(@"the date '(.*)' is returned")]
    public void ThenTheDateIsReturned(DateTime expected)
    {
        Assert.NotNull(_dateResult);
        Assert.Equal(expected, _dateResult.Value);
    }

    [Then(@"the boolean value (.*) is returned")]
    public void ThenTheBooleanValueIsReturned(bool expected)
    {
        Assert.NotNull(_boolResult);
        Assert.Equal(expected, _boolResult.Value);
    }
}
