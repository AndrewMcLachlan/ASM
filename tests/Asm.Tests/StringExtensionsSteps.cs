namespace Asm.Tests;

[Binding]
public class StringExtensionsSteps(ScenarioContext context)
{
    private string _input;
    private string _separator;

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

}
