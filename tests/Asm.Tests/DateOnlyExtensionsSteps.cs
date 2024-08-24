namespace Asm.Tests;

[Binding]
public class DateOnlyExtensionsSteps(ScenarioContext context)
{
    private DateOnly _date;
    private DateOnly _otherDate;

    [When(@"I get today's date")]
    public void WhenIGetTodaySDate()
    {
        _date = DateOnlyExtensions.Today();
    }

    [Then(@"the result should be today's date")]
    public void ThenTheResultShouldBeTodaySDate()
    {
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), _date);
    }

    [Given(@"I have a date ""(.*)""")]
    public void GivenIHaveADate(string date)
    {
        _date = DateOnly.Parse(date);
    }

    [Given(@"I have another date ""(.*)""")]
    public void GivenIHaveAnotherDate(string otherDate)
    {
        _otherDate = DateOnly.Parse(otherDate);
    }

    [When(@"I convert the date to DateTime at the start of the day")]
    public void WhenIConvertTheDateToDateTimeAtTheStartOfTheDay()
    {
        context.AddResult(_date.ToStartOfDay());
    }

    [When(@"I convert the date to DateTime at the end of the day")]
    public void WhenIConvertTheDateToDateTimeAtTheEndOfTheDay()
    {
        context.AddResult(_date.ToEndOfDay());
    }

    [When(@"I calculate the difference in months between the dates")]
    public void WhenICalculateTheDifferenceInMonthsBetweenTheDates()
    {
        context.AddResult(_date.DifferenceInMonths(_otherDate));
    }
}
