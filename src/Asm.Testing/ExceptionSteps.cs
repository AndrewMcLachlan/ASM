using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Testing;

[Binding]
public class ExceptionSteps(ScenarioResult<Exception> result)
{
    [Then(@"an exception of type '(.*)' is thrown")]
    public void ThenAnExceptionOfTypeIsThrown(string exceptionType)
    {
        Type? expected = Type.GetType(exceptionType, true);

        Assert.NotNull(expected);

        var actual = result.Value;

        Assert.NotNull(actual);
        Assert.IsType(expected, actual);
    }

    [Then(@"the exception message is '(.*)'")]
    public void ThenTheExceptionMessageIs(string message)
    {
        Assert.NotNull(result.Value);
        Assert.Equal(message.DecodeWhitespace(), result.Value!.Message);
    }

    [Then(@"the exception parameter name is '(.*)'")]
    public void ThenTheExceptionParameterNameIs(string parameterName)
    {
        var exception = result.Value;

        Assert.NotNull(exception);
        Assert.IsAssignableFrom(typeof(ArgumentException), exception);
        Assert.Equal(parameterName, ((ArgumentException)exception!).ParamName);
    }
}
