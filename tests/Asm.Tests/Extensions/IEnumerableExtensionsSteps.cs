using Asm.Testing;

namespace Asm.Tests.Extensions;

[Binding]
public class IEnumerableExtensionsSteps(ScenarioContext context)
{
    private IEnumerable<int> _enumerable;
    private IEnumerable<int> _result;
    private bool _boolResult;

    [Given(@"I have an enumerable with values \[(.*)\]")]
    public void GivenIHaveAnEnumerableWithValues(string values)
    {
        if (String.IsNullOrWhiteSpace(values))
        {
            _enumerable = [];
        }
        else
        {
            _enumerable = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
        }
    }

    [Given(@"I have a null enumerable")]
    public void GivenIHaveANullEnumerable()
    {
        _enumerable = null;
    }

    [Given(@"I have an empty enumerable")]
    public void GivenIHaveAnEmptyEnumerable()
    {
        _enumerable = [];
    }

    [When(@"I call Page with page size (.*) and page number (.*)")]
    public void WhenICallPageWithPageSizeAndPageNumber(int pageSize, int pageNumber)
    {
        _result = _enumerable!.Page(pageSize, pageNumber);
    }

    [When(@"I call Shuffle on the enumerable")]
    public void WhenICallShuffleOnTheEnumerable()
    {
        _result = IEnumerableExtensions.Shuffle(_enumerable!);
    }

    [When(@"I call IsNullOrEmpty on the enumerable")]
    public void WhenICallIsNullOrEmptyOnTheEnumerable()
    {
        _boolResult = _enumerable.IsNullOrEmpty();
        context.AddResult(_boolResult);
    }

    [When(@"I call Empty on the enumerable")]
    public void WhenICallEmptyOnTheEnumerable()
    {
        _boolResult = _enumerable!.Empty();
        context.AddResult(_boolResult);
    }

    [Then(@"the result should be \[(.*)\]")]
    public void ThenTheResultShouldBe(string expected)
    {
        if (String.IsNullOrWhiteSpace(expected))
        {
            Assert.Empty(_result!);
        }
        else
        {
            var expectedValues = expected.Split(',').Select(v => Int32.Parse(v.Trim())).ToList();
            Assert.Equal(expectedValues, [.. _result!]);
        }
    }

    [Then(@"the result should be empty")]
    public void ThenTheResultShouldBeEmpty()
    {
        Assert.Empty(_result!);
    }

    [Then(@"the result should contain all original elements")]
    public void ThenTheResultShouldContainAllOriginalElements()
    {
        var original = _enumerable!.OrderBy(x => x).ToList();
        var shuffled = _result!.OrderBy(x => x).ToList();
        Assert.Equal(original, shuffled);
    }

    [Then(@"the result should have (.*) elements")]
    public void ThenTheResultShouldHaveElements(int count)
    {
        Assert.Equal(count, _result!.Count());
    }
}
