namespace Asm.Reqnroll;

/// <summary>
/// Common steps for handling exceptions.
/// </summary>
/// <param name="context">Scenario context</param>
[Binding]
public class ExceptionSteps(ScenarioContext context)
{
    /// <summary>
    /// The key used to store the exception in the context.
    /// </summary>
    public const string ExceptionKey = nameof(ExceptionKey);

    /// <summary>
    /// Asserts an exception of the given type has been thrown.
    /// </summary>
    /// <param name="exceptionType">The fully qualified type name.</param>
    [Then(@"an exception of type '([^']*)' should be thrown")]
    [Then(@"an exception of type '([^']*)' is thrown")]
    [Then(@"an exception of type ""([^""]*)"" should be thrown")]
    [Then(@"an exception of type ""([^""]*)"" is thrown")]
    public void ThenAnExceptionOfTypeIsThrown(string exceptionType)
    {
        // Type.GetType only probes corlib and this assembly unless the name is assembly-qualified.
        // Fall back to scanning loaded assemblies so library types (e.g. Asm.NotFoundException)
        // resolve from their simple full name.
        Type? expected = Type.GetType(exceptionType, throwOnError: false)
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(exceptionType, throwOnError: false))
                .FirstOrDefault(t => t is not null);

        Assert.True(expected is not null, $"Could not resolve exception type '{exceptionType}'.");

        var actual = context.GetException();

        Assert.NotNull(actual);
        Assert.IsType(expected, actual);
    }

    /// <summary>
    /// Asserts the exception message is as expected.
    /// </summary>
    /// <param name="message">The message.</param>
    [Then(@"the exception message is '(.*)'")]
    [Then(@"the exception message should be '(.*)'")]
    [Then(@"the exception message is ""([^""]*)""")]
    [Then(@"the exception message should be ""([^""]*)""")]
    public void ThenTheExceptionMessageIs(string message)
    {
        var ex = context.GetException();
        Assert.NotNull(ex);
        Assert.Equal(message.DecodeWhitespace(), ex.Message);
    }

    /// <summary>
    /// Asserts the argument exception parameter name is as expected.
    /// </summary>
    /// <remarks>
    /// The exception thrown must have been of type <see cref="ArgumentException"/>.
    /// </remarks>
    /// <param name="parameterName">The name of the parameter.</param>
    [Then(@"the exception parameter name is '(.*)'")]
    [Then(@"the exception parameter name should be '(.*)'")]
    [Then(@"the exception parameter name is ""([^""]*)""")]
    [Then(@"the exception parameter name should be ""([^""]*)""")]
    public void ThenTheExceptionParameterNameIs(string parameterName)
    {
        var ex = context.GetException();

        Assert.NotNull(ex);
#pragma warning disable xUnit2007 // I want to check for derived types.
        Assert.IsAssignableFrom(typeof(ArgumentException), ex);
#pragma warning restore xUnit2007 // Do not use typeof expression to check the type
        Assert.Equal(parameterName, ((ArgumentException)ex!).ParamName);
    }

    /// <summary>
    /// Asserts no exception has been thrown.
    /// </summary>
    [Then(@"no exception is thrown")]
    public void ThenNoExceptionIsThrown()
    {
        var actual = context.GetException();

        Assert.Null(actual);
    }
}
