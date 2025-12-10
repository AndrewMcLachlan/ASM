namespace Asm.Tests;

[Binding]
[Scope(Feature = "String Extensions")]
public class StringExtensionsSteps(ScenarioContext context)
{
    private string _input;
    private string _separator;
    private string _result;
    private int? _fromStart;
    private int? _fromEnd;

    [Given(@"I have a string '(.*)'")]
    public void GivenIHaveAString(string input)
    {
        _input = input;
    }

    [Given(@"I have a separator '(.*)'")]
    public void GivenIHaveASeparator(string separator)
    {
        _separator = separator;
    }

    [When(@"I Append '(.*)' to the string")]
    public void WhenIAppendToTheString(string append)
    {
        context.AddResult(_input.Append(append, _separator));
    }

    [When(@"I Prepend '(.*)' to the string")]
    public void WhenIPrependToTheString(string append)
    {
        context.AddResult(_input.Prepend(append, _separator));
    }

    [When(@"I Squish the string by (.*) characters")]
    public void WhenISquishTheStringByCharacters(int chars)
    {
        context.AddResult(_input.Squish(chars, chars));
    }

    [When(@"I Squish the string with fromStart (.*) and fromEnd (.*)")]
    public void WhenISquishTheStringWithFromStartAndFromEnd(int fromStart, int fromEnd)
    {
        context.CatchException(() => context.AddResult(_input.Squish(fromStart, fromEnd)));
    }

    [When(@"I convert the string to machine format")]
    public void WhenIConvertTheStringToMachineFormat()
    {
        context.AddResult(_input.ToMachine());
    }

    [When(@"I convert the string to title case")]
    public void WhenIConvertTheStringToTitleCase()
    {
        context.AddResult(_input.ToTitleCase());
    }

    [When(@"I Squish the string by (.*) characters from start only")]
    public void WhenISquishTheStringByCharactersFromStartOnly(int chars)
    {
        context.AddResult(_input.Squish(chars, null));
    }

    [When(@"I Squish the string by (.*) characters from end only")]
    public void WhenISquishTheStringByCharactersFromEndOnly(int chars)
    {
        context.AddResult(_input.Squish(null, chars));
    }

    [When(@"I Squish the string with both parameters null")]
    public void WhenISquishTheStringWithBothParametersNull()
    {
        context.AddResult(_input.Squish(null, null));
    }

    [When(@"I Squish the string with zero from start and (.*) from end")]
    public void WhenISquishTheStringWithZeroFromStartAndFromEnd(int fromEnd)
    {
        context.AddResult(_input.Squish(0, fromEnd));
    }

    [When(@"I Squish the string with (.*) from start and zero from end")]
    public void WhenISquishTheStringWithFromStartAndZeroFromEnd(int fromStart)
    {
        context.AddResult(_input.Squish(fromStart, 0));
    }

    [When(@"I Squish an empty string by (.*) characters")]
    public void WhenISquishAnEmptyStringByCharacters(int chars)
    {
        _input = string.Empty;
        context.AddResult(_input.Squish(chars, chars));
    }

    [When(@"I Squish a single character string")]
    public void WhenISquishASingleCharacterString()
    {
        context.AddResult(_input.Squish(1, 1));
    }

    [When(@"I Squish with values exceeding string length")]
    public void WhenISquishWithValuesExceedingStringLength()
    {
        context.AddResult(_input.Squish(_input.Length + 10, _input.Length + 10));
    }

    [Then(@"the result should not contain the removed characters")]
    public void ThenTheResultShouldNotContainTheRemovedCharacters()
    {
        var result = context.GetResult<string>();
        Assert.NotNull(result);
        Assert.NotEqual(_input, result);
    }

    [Then(@"the result should be empty or shorter")]
    public void ThenTheResultShouldBeEmptyOrShorter()
    {
        var result = context.GetResult<string>();
        Assert.True(string.IsNullOrEmpty(result) || result.Length <= _input.Length);
    }

    [Then(@"the result should equal the original string")]
    public void ThenTheResultShouldEqualTheOriginalString()
    {
        var result = context.GetResult<string>();
        Assert.Equal(_input, result);
    }

    [Then(@"the result should have length of exactly (.*)")]
    public void ThenTheResultShouldHaveLengthOfExactly(int expectedLength)
    {
        var result = context.GetResult<string>();
        Assert.Equal(expectedLength, result?.Length ?? 0);
    }
}
