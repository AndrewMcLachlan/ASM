using System.Linq.Expressions;

namespace Asm.Testing.Steps;

[Binding]
public class IQueryableExtensionsSteps
{
    private class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj) =>
            obj is Item item && item.Id == Id && item.Name == Name;
    }

    private IQueryable<Item> _dataSource;
    private IQueryable<Item> _result;

    [Given(@"I have a data source with the following items")]
    public void GivenIHaveADataSourceWithTheFollowingItems(Table table)
    {
        var items = table.Rows.Select(row => new Item
        {
            Id = Int32.Parse(row["Id"]),
            Name = row["Name"]
        }).AsQueryable();

        _dataSource = items;
    }

    [When(@"I retrieve page (.*) with a page size of (.*)")]
    public void WhenIRetrievePageWithAPageSizeOf(int pageNumber, int pageSize)
    {
        _result = _dataSource.Page(pageSize, pageNumber);
    }

    [Then(@"the result should contain the following items")]
    public void ThenTheResultShouldContainTheFollowingItems(Table table)
    {
        var expectedItems = table.Rows.Select(row => new Item
        {
            Id = Int32.Parse(row["Id"]),
            Name = row["Name"]
        });

        Assert.Collection(_result, expectedItems.Select<Item, Action<Item>>(ei => (ai) => Assert.Equal(ei, ai)).ToArray());
    }

    [When(@"I filter the data with predicates ""(.*)"" or ""(.*)""")]
    public void WhenIFilterTheDataWithPredicates(string predicate1, string predicate2)
    {
        var predicates = new List<Expression<Func<Item, bool>>>
        {
            item => item.Id == Int32.Parse(predicate1.Split(' ', StringSplitOptions.None)[2]),
            item => item.Name == predicate2.Split(' ', StringSplitOptions.None)[2].Trim('\'')
        };

        _result = _dataSource.WhereAny(predicates);
    }
}
