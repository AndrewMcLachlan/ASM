using System.Globalization;

namespace Asm.Tests.Extensions;

[Binding]
public class DateTimeExtensionsSteps(ScenarioContext context)
{
    public class ScenarioInput
    {
        public DateTime Date { get; set; }
        public DateTime OtherDate { get; set; }
        public IFormatProvider FormatProvider { get; set; }
    }

    private ScenarioInput _input = new();

    [Given(@"I have a date '(.*)'")]
    public void GivenIHaveADate(DateTime date)
    {
        _input.Date = date;
    }

    [Given(@"my locale is '(.*)'")]
    public void GivenMyLocaleIs(string locale)
    {
        _input.FormatProvider = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
    }

    [Given(@"I have another date '([^']*)'")]
    public void GivenIHaveAnotherDate(DateTime date)
    {
        _input.OtherDate = date;
    }


    [When(@"I call FirstDayOfWeek")]
    public void WhenICallFirstDayOfWeek()
    {
        if (_input.FormatProvider != null)
        {
            context.AddResult(_input.Date.FirstDayOfWeek(_input.FormatProvider));
        }
        else
        {
            context.AddResult(_input.Date.FirstDayOfWeek());
        }
    }

    [When(@"I call LastDayOfWeek")]
    public void WhenICallLastDayOfWeek()
    {
        if (_input.FormatProvider != null)
        {
            context.AddResult(_input.Date.LastDayOfWeek(_input.FormatProvider));
        }
        else
        {
            context.AddResult(_input.Date.LastDayOfWeek());
        }
    }

    [When(@"I call DifferenceInMonths")]
    public void WhenICallDifferenceInMonths()
    {
        context.AddResult(_input.Date.DifferenceInMonths(_input.OtherDate));
    }

    [Given(@"I have a datetime '([^']*)'")]
    public void GivenIHaveADatetime(DateTime datetime)
    {
        _input.Date = datetime;
    }

    [When(@"I call ToDateOnly")]
    public void WhenICallToDateOnly()
    {
        context.AddResult(_input.Date.ToDateOnly());
    }

    [When(@"I call ToTimeOnly")]
    public void WhenICallToTimeOnly()
    {
        context.AddResult(_input.Date.ToTimeOnly());
    }

    [When(@"I access the DateOnly extension property")]
    public void WhenIAccessTheDateOnlyExtensionProperty()
    {
        context.AddResult(_input.Date.DateOnly);
    }

    [When(@"I access the TimeOnly extension property")]
    public void WhenIAccessTheTimeOnlyExtensionProperty()
    {
        context.AddResult(_input.Date.TimeOnly);
    }

    [Then(@"the DateOnly result should be '([^']*)'")]
    public void ThenTheDateOnlyResultShouldBe(DateOnly expected)
    {
        var result = context.Get<DateOnly>("Result");
        Assert.Equal(expected, result);
    }

    [Then(@"the TimeOnly result should be '([^']*)'")]
    public void ThenTheTimeOnlyResultShouldBe(TimeOnly expected)
    {
        var result = context.Get<TimeOnly>("Result");
        Assert.Equal(expected, result);
    }
}
