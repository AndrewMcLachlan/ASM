using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Testing;

/// <summary>
/// Reusable assertion steps for simple types.
/// </summary>
/// <param name="context">The current scenario context.</param>
[Binding]
public class SimpleAssertionSteps(ScenarioContext context)
{
    /// <summary>
    /// The key used to store the result in the context.
    /// </summary>
    public const string ResultKey = "Result";

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the string value ""([^""]*)"" is returned")]
    [Then(@"the string value '([^']*)' is returned")]
    [Then(@"the string result should be ""([^""]*)""")]
    [Then(@"the string result should be '([^']*)'")]
    public void ThenTheStringValueIsReturned(string? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the GUID value ""([^""]*)"" is returned")]
    [Then(@"the GUID value '([^']*)' is returned")]
    [Then(@"the GUID result should be ""([^""]*)""")]
    [Then(@"the GUID result should be '([^']*)'")]
    public void ThenTheGuidValueIsReturned(Guid? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the integer value (.*) is returned")]
    [Then(@"the integer result should be (.*)")]
    public void ThenTheIntegerValueIsReturned(int? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the date ""([^""]*)"" is returned")]
    [Then(@"the date '([^']*)' is returned")]
    [Then(@"the date result should be ""([^""]*)""")]
    [Then(@"the date result should be '([^']*)'")]
    [Then(@"the DateTime result should be ""([^""]*)""")]
    [Then(@"the DateTime result should be '([^']*)'")]
    public void ThenTheDateIsReturned(DateTime? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the boolean value (.*) is returned")]
    [Then(@"the boolean result should be (.*)")]
    public void ThenTheBooleanValueIsReturned(bool? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the byte value (.*) is returned")]
    [Then(@"the byte result should be (.*)")]
    public void ThenTheByteValueIsReturned(byte? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the ushort value (.*) is returned")]
    [Then(@"the ushort result should be (.*)")]
    public void ThenTheUShortValueIsReturned(ushort? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the uint value (.*) is returned")]
    [Then(@"the uint result should be (.*)")]
    public void ThenTheUIntValueIsReturned(uint? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is equal to the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    [Then(@"the ulong value (.*) is returned")]
    [Then(@"the ulong result should be (.*)")]
    public void ThenTheULongValueIsReturned(ulong? expected) => AssertValue(expected);

    /// <summary>
    /// Asserts whether the result is <see langword="null" />.
    /// </summary>
    [Then(@"the value is null")]
    [Then(@"the result should be null")]
    public void ThenTheValueIsNull()
    {
        var result = context.Get<object?>(ResultKey);
        Assert.Null(result);
    }

    private void AssertValue<T>(T? expected)
    {
        var result = context.Get<T>(ResultKey);

        Assert.Equal(expected, result);
    }
}

