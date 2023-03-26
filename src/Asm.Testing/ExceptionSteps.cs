using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Testing;

[Binding]
public class ExceptionSteps
{
    private ScenarioResult<Exception> _result;

    public ExceptionSteps(ScenarioResult<Exception> result)
    {
        _result = result;
    }

    [Then(@"an exception of type '(.*)' is thrown")]
    public void ThenAnExceptionOfTypeIsThrown(string exceptionType)
    {
        Type? expected = Type.GetType(exceptionType, true);

        Assert.NotNull(expected);

        var actual = _result.Result;

        Assert.NotNull(actual);
        Assert.IsType(expected, actual);
    }

    [Then(@"the exception message is '(.*)'")]
    public void ThenTheExceptionMessageIs(string message)
    {
        Assert.NotNull(_result.Result);
        Assert.Equal(message.DecodeWhitespace(), _result.Result!.Message);
    }

    [Then(@"the exception parameter name is '(.*)'")]
    public void ThenTheExceptionParameterNameIs(string parameterName)
    {
        var exception = _result.Result;

        Assert.NotNull(exception);
        Assert.IsAssignableFrom(typeof(ArgumentException), exception);
        Assert.Equal(parameterName, ((ArgumentException)exception!).ParamName);
    }
}
