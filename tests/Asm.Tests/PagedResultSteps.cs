namespace Asm.Tests;

[Binding]
public class PagedResultSteps
{
    private List<int> _items;
    private PagedResult<int> _result;

    [Given(@"I have a list of items \[(.*)\]")]
    public void GivenIHaveAListOfItems(string values)
    {
        _items = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
    }

    [Given(@"I have an empty list of items")]
    public void GivenIHaveAnEmptyListOfItems()
    {
        _items = [];
    }

    [When(@"I create a PagedResult with total (.*)")]
    public void WhenICreateAPagedResultWithTotal(int total)
    {
        _result = new PagedResult<int> { Results = _items, Total = total };
    }

    [Then(@"the PagedResult should have (.*) results")]
    public void ThenThePagedResultShouldHaveResults(int count)
    {
        Assert.Equal(count, _result.Results.Count());
    }

    [Then(@"the PagedResult total should be (.*)")]
    public void ThenThePagedResultTotalShouldBe(int total)
    {
        Assert.Equal(total, _result.Total);
    }
}
