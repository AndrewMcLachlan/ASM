namespace Asm.Tests.Extensions;

[Binding]
public class ICollectionExtensionsSteps(ScenarioContext context)
{
    private ICollection<int> _collection;
    private IEnumerable<int> _itemsToAdd;

    [Given(@"I have a collection with values \[(.*)\]")]
    public void GivenIHaveACollectionWithValues(string values)
    {
        _collection = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
    }

    [Given(@"I have an empty collection")]
    public void GivenIHaveAnEmptyCollection()
    {
        _collection = [];
    }

    [Given(@"I have a null collection")]
    public void GivenIHaveANullCollection()
    {
        _collection = null;
    }

    [Given(@"I have a readonly collection with values \[(.*)\]")]
    public void GivenIHaveAReadonlyCollectionWithValues(string values)
    {
        List<int> items = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
        _collection = items.AsReadOnly();
    }

    [Given(@"I have items to add \[(.*)\]")]
    public void GivenIHaveItemsToAdd(string values)
    {
        _itemsToAdd = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
    }

    [Given(@"I have empty items to add")]
    public void GivenIHaveEmptyItemsToAdd()
    {
        _itemsToAdd = [];
    }

    [Given(@"I have null items to add")]
    public void GivenIHaveNullItemsToAdd()
    {
        _itemsToAdd = null;
    }

    [When(@"I call AddRange on the collection")]
    public void WhenICallAddRangeOnTheCollection()
    {
        try
        {
            _collection.AddRange(_itemsToAdd!);
        }
        catch (Exception ex)
        {
            context.AddException(ex);
        }
    }

    [Then(@"the collection should contain \[(.*)\]")]
    public void ThenTheCollectionShouldContain(string expected)
    {
        var expectedValues = expected.Split(',').Select(v => Int32.Parse(v.Trim()));
        Assert.Equal(expectedValues, [.. _collection!]);
    }
}
