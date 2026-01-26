namespace Asm.Tests.Extensions;

[Binding]
public class TimeOnlyExtensionsSteps(ScenarioContext context)
{
    [When(@"I get TimeOnly Now")]
    public void WhenIGetTimeOnlyNow()
    {
        context.AddResult(TimeOnly.Now);
    }

    [When(@"I get TimeOnly UtcNow")]
    public void WhenIGetTimeOnlyUtcNow()
    {
        context.AddResult(TimeOnly.UtcNow);
    }

    [Then(@"the result should be approximately the current time")]
    public void ThenTheResultShouldBeApproximatelyTheCurrentTime()
    {
        var result = context.GetResult<TimeOnly>();
        var now = TimeOnly.FromDateTime(DateTime.Now);

        // Allow 1 second tolerance
        var diff = Math.Abs(result.Ticks - now.Ticks);
        Assert.True(diff < TimeSpan.FromSeconds(1).Ticks, $"Expected time to be close to current UTC time, but difference was {diff} seconds");
    }

    [Then(@"the result should be approximately the current UTC time")]
    public void ThenTheResultShouldBeApproximatelyTheCurrentUtcTime()
    {
        var result = context.GetResult<TimeOnly>();
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        
        // Allow 1 second tolerance
        var diff = Math.Abs(result.Ticks - now.Ticks);
        Assert.True(diff < TimeSpan.FromSeconds(1).Ticks, $"Expected time to be close to current UTC time, but difference was {diff} seconds");
    }
}

