namespace Asm.Cqrs.AspNetCore.Tests;

[Binding]
public class CommandBindingSteps
{
    [Then(@"the CommandBinding enum value '(.*)' should equal (.*)")]
    public void ThenTheCommandBindingEnumValueShouldEqual(string name, int expectedValue)
    {
        var actualValue = (int)Enum.Parse<CommandBinding>(name);
        Assert.Equal(expectedValue, actualValue);
    }

    [Then(@"the CommandBinding enum should have (.*) values")]
    public void ThenTheCommandBindingEnumShouldHaveValues(int expectedCount)
    {
        var values = Enum.GetValues<CommandBinding>();
        Assert.Equal(expectedCount, values.Length);
    }
}
