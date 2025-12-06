using Asm.Testing;

namespace Asm.Tests.Extensions;

[Binding]
public class IListExtensionsSteps(ScenarioContext context)
{
    private IList<int> _list;
    private IList<int> _originalValues;
    private bool _boolResult;

    [Given(@"I have a list with values \[(.*)\]")]
    public void GivenIHaveAListWithValues(string values)
    {
        _list = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
        _originalValues = [.. _list];
    }

    [Given(@"I have a null list")]
    public void GivenIHaveANullList()
    {
        _list = null;
    }

    [Given(@"I have an empty list")]
    public void GivenIHaveAnEmptyList()
    {
        _list = [];
    }

    [Given(@"I have a readonly list with values \[(.*)\]")]
    public void GivenIHaveAReadonlyListWithValues(string values)
    {
        var items = values.Split(',').Select(v => Int32.Parse(v.Trim())).ToList();
        _list = items.AsReadOnly();
    }

    [When(@"I call Shuffle on the list")]
    public void WhenICallShuffleOnTheList()
    {
        try
        {
            _list!.Shuffle();
        }
        catch (Exception ex)
        {
            context.AddException(ex);
        }
    }

    [When(@"I call IsNullOrEmpty on the list")]
    public void WhenICallIsNullOrEmptyOnTheList()
    {
        _boolResult = _list.IsNullOrEmpty();
        context.AddResult(_boolResult);
    }

    [When(@"I call Empty on the list")]
    public void WhenICallEmptyOnTheList()
    {
        _boolResult = _list!.Empty();
        context.AddResult(_boolResult);
    }

    [Then(@"the list should contain all original elements")]
    public void ThenTheListShouldContainAllOriginalElements()
    {
        var sorted = _list!.OrderBy(x => x).ToList();
        var originalSorted = _originalValues!.OrderBy(x => x).ToList();
        Assert.Equal(originalSorted, sorted);
    }

    [Then(@"the list should have (.*) elements")]
    public void ThenTheListShouldHaveElements(int count)
    {
        Assert.Equal(count, _list!.Count);
    }
}
