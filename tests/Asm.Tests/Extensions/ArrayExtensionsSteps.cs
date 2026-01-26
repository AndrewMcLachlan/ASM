namespace Asm.Tests.Extensions;

[Binding]
public class ArrayExtensionsSteps(ScenarioContext context)
{
    private int[] _array;
    private int[] _originalValues;

    [Given(@"I have an array with values \[(.*)\]")]
    public void GivenIHaveAnArrayWithValues(string values)
    {
        _array = [.. values.Split(',').Select(v => Int32.Parse(v.Trim()))];
        _originalValues = [.. _array];
    }

    [Given(@"I have a null array")]
    public void GivenIHaveANullArray()
    {
        _array = null;
    }

    [Given(@"I have an empty array")]
    public void GivenIHaveAnEmptyArray()
    {
        _array = [];
    }

    [When(@"I call Shuffle on the array")]
    public void WhenICallShuffleOnTheArray()
    {
        context.CatchException(() => _array.Shuffle());
    }

    [When(@"I call IsNullOrEmpty on the array")]
    public void WhenICallIsNullOrEmptyOnTheArray()
    {
        context.AddResult(_array.IsNullOrEmpty());
    }

    [When(@"I call Empty on the array")]
    public void WhenICallEmptyOnTheArray()
    {
        context.AddResult(_array.Empty());
    }

    [Then(@"the array should contain all original elements")]
    public void ThenTheArrayShouldContainAllOriginalElements()
    {
        var sorted = _array.OrderBy(x => x).ToArray();
        var originalSorted = _originalValues.OrderBy(x => x).ToArray();
        Assert.Equal(originalSorted, sorted);
    }

    [Then(@"the array should have (.*) elements")]
    public void ThenTheArrayShouldHaveElements(int count)
    {
        Assert.Equal(count, _array.Length);
    }
}
