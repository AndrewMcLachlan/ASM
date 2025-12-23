using System.Collections;

namespace Asm.Tests.Extensions;

[Binding]
public class IEnumeratorExtensionsSteps(ScenarioContext scenarioContext)
{
    private ArrayList _arrayList = null!;

    [Given(@"I have an ArrayList with values (.*), (.*), (.*)")]
    public void GivenIHaveAnArrayListWithValues(int v1, int v2, int v3)
    {
        _arrayList = [v1, v2, v3];
    }

    [Given(@"I have an empty ArrayList")]
    public void GivenIHaveAnEmptyArrayList()
    {
        _arrayList = [];
    }

    [Given(@"I have an ArrayList with strings '(.*)', '(.*)', '(.*)'")]
    public void GivenIHaveAnArrayListWithStrings(string s1, string s2, string s3)
    {
        _arrayList = [s1, s2, s3];
    }

    [When(@"I call GetEnumerator to convert to IEnumerator of int")]
    public void WhenICallGetEnumeratorToConvertToIEnumeratorOfInt()
    {
        var enumerator = _arrayList.GetEnumerator();
        var genericEnumerator = enumerator.GetEnumerator<int>();
        var results = new List<int>();
        while (genericEnumerator.MoveNext())
        {
            results.Add(genericEnumerator.Current);
        }
        scenarioContext.AddResult(results);
    }

    [When(@"I call GetEnumerator to convert to IEnumerator of string")]
    public void WhenICallGetEnumeratorToConvertToIEnumeratorOfString()
    {
        var enumerator = _arrayList.GetEnumerator();
        var genericEnumerator = enumerator.GetEnumerator<string>();
        var results = new List<string>();
        while (genericEnumerator.MoveNext())
        {
            results.Add(genericEnumerator.Current);
        }
        scenarioContext.AddResult(results);
    }

    [Then(@"I should be able to enumerate and get values (.*), (.*), (.*)")]
    public void ThenIShouldBeAbleToEnumerateAndGetValues(int v1, int v2, int v3)
    {
        var results = scenarioContext.GetResult<List<int>>();
        Assert.Equal(3, results.Count);
        Assert.Equal(v1, results[0]);
        Assert.Equal(v2, results[1]);
        Assert.Equal(v3, results[2]);
    }

    [Then(@"the enumeration should yield no results")]
    public void ThenTheEnumerationShouldYieldNoResults()
    {
        var results = scenarioContext.GetResult<List<int>>();
        Assert.Empty(results);
    }

    [Then(@"I should be able to enumerate and get strings '(.*)', '(.*)', '(.*)'")]
    public void ThenIShouldBeAbleToEnumerateAndGetStrings(string s1, string s2, string s3)
    {
        var results = scenarioContext.GetResult<List<string>>();
        Assert.Equal(3, results.Count);
        Assert.Equal(s1, results[0]);
        Assert.Equal(s2, results[1]);
        Assert.Equal(s3, results[2]);
    }
}
