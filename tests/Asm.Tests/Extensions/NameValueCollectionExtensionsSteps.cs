#nullable enable
using System.Collections.Specialized;

namespace Asm.Tests.Extensions;

[Binding]
public class NameValueCollectionExtensionsSteps(ScenarioContext scenarioContext)
{
    private NameValueCollection? _collection;

    [Given(@"I have a NameValueCollection with key '(.*)' and value '(.*)'")]
    public void GivenIHaveANameValueCollectionWithKeyAndValue(string key, string value)
    {
        _collection = new NameValueCollection { { key, value } };
    }

    [Given(@"I have an empty NameValueCollection")]
    public void GivenIHaveAnEmptyNameValueCollection()
    {
        _collection = [];
    }

    [Given(@"I have a NameValueCollection with key '(.*)' and null value")]
    public void GivenIHaveANameValueCollectionWithKeyAndNullValue(string key)
    {
        _collection = [];
        _collection.Add(key, null);
    }

    [Given(@"I have a null NameValueCollection")]
    public void GivenIHaveANullNameValueCollection()
    {
        _collection = null;
    }

    [When(@"I call GetValue for '(.*)' with default (.*)")]
    public void WhenICallGetValueForWithDefault(string key, int defaultValue)
    {
        var result = _collection!.GetValue(key, defaultValue);
        scenarioContext.AddResult(result);
    }

    [When(@"I call GetValue for '(.*)' with default '(.*)'")]
    public void WhenICallGetValueForWithDefaultString(string key, string defaultValue)
    {
        var result = _collection!.GetValue(key, defaultValue);
        scenarioContext.AddResult(result);
    }

    [When(@"I call GetValue for '(.*)' with default (.*) as boolean")]
    public void WhenICallGetValueForWithDefaultAsBoolean(string key, bool defaultValue)
    {
        var result = _collection!.GetValue(key, defaultValue);
        scenarioContext.AddResult(result);
    }

    [When(@"I call GetValue for '(.*)' with default (.*) as decimal")]
    public void WhenICallGetValueForWithDefaultAsDecimal(string key, decimal defaultValue)
    {
        var result = _collection!.GetValue(key, defaultValue);
        scenarioContext.AddResult(result);
    }

    [When(@"I call GetValue and expect an exception")]
    public void WhenICallGetValueAndExpectAnException()
    {
        scenarioContext.CatchException(() => _collection!.GetValue("any", 0));
    }

    [Then(@"the decimal result should be (.*)")]
    public void ThenTheDecimalResultShouldBe(decimal expected)
    {
        var result = scenarioContext.GetResult<decimal>();
        Assert.Equal(expected, result);
    }
}
