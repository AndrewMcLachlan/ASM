namespace Asm.Testing;

[Binding]
public class ExceptionSteps(ScenarioContext context)
{
    public const string ExceptionKey = nameof(ExceptionKey);

    [Then(@"an exception of type '([^']*)' should be thrown")]
    [Then(@"an exception of type '([^']*)' is thrown")]
    [Then(@"an exception of type ""([^""]*)"" should be thrown")]
    [Then(@"an exception of type ""([^""]*)"" is thrown")]
    public void ThenAnExceptionOfTypeIsThrown(string exceptionType)
    {
        Type? expected = Type.GetType(exceptionType, true);

        Assert.NotNull(expected);

        var actual = context.Get<Exception>(ExceptionKey);

        Assert.NotNull(actual);
        Assert.IsType(expected, actual);
    }

    [Then(@"the exception message is '(.*)'")]
    [Then(@"the exception message should be '(.*)'")]
    [Then(@"the exception message is ""([^""]*)""")]
    [Then(@"the exception message should be ""([^""]*)""")]
    public void ThenTheExceptionMessageIs(string message)
    {
        var ex = context.Get<Exception>(ExceptionKey);
        Assert.NotNull(ex);
        Assert.Equal(message.DecodeWhitespace(), ex.Message);
    }

    [Then(@"the exception parameter name is '(.*)'")]
    [Then(@"the exception parameter name should be '(.*)'")]
    [Then(@"the exception parameter name is ""([^""]*)""")]
    [Then(@"the exception parameter name should be ""([^""]*)""")]
    public void ThenTheExceptionParameterNameIs(string parameterName)
    {
        var ex = context.Get<Exception>(ExceptionKey);

        Assert.NotNull(ex);
#pragma warning disable xUnit2007 // I want to check for derived types.
        Assert.IsAssignableFrom(typeof(ArgumentException), ex);
#pragma warning restore xUnit2007 // Do not use typeof expression to check the type
        Assert.Equal(parameterName, ((ArgumentException)ex!).ParamName);
    }
}
