using System.Globalization;
using System.Text;

namespace Asm.Tests.Extensions;

[Binding]
public class StringBuilderExtensionsSteps
{
    private StringBuilder _stringBuilder;

    [Given(@"I have a StringBuilder with content '(.*)'")]
    public void GivenIHaveAStringBuilderWithContent(string content)
    {
        _stringBuilder = new StringBuilder(content);
    }

    [Given(@"I have an empty StringBuilder")]
    public void GivenIHaveAnEmptyStringBuilder()
    {
        _stringBuilder = new StringBuilder();
    }

    [When(@"I call AppendFormatLine with format '(.*)' and arg '(.*)'")]
    public void WhenICallAppendFormatLineWithFormatAndArg(string format, string arg)
    {
        _stringBuilder.AppendFormatLine(format, arg);
    }

    [When(@"I call AppendFormatLine with InvariantCulture and format '(.*)' and arg (.*)")]
    public void WhenICallAppendFormatLineWithInvariantCultureAndFormatAndArg(string format, double arg)
    {
        _stringBuilder.AppendFormatLine(CultureInfo.InvariantCulture, format, arg);
    }

    [When(@"I call AppendFormatLine with format '(.*)' and args \[(.*)\]")]
    public void WhenICallAppendFormatLineWithFormatAndArgs(string format, string args)
    {
        var argArray = args.Split(',').Select(a => (object)Int32.Parse(a.Trim())).ToArray();
        _stringBuilder.AppendFormatLine(format, argArray);
    }

    [Then(@"the StringBuilder should contain '(.*)' followed by a newline")]
    public void ThenTheStringBuilderShouldContainFollowedByANewline(string expected)
    {
        var result = _stringBuilder.ToString();
        Assert.Equal(expected + Environment.NewLine, result);
    }
}
