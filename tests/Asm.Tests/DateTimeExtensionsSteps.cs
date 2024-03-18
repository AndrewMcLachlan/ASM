using System;
using System.Globalization;
using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Tests;

[Binding]
public class DateTimeExtensionsSteps
{
    public class ScenarioInput
    {
        public DateTime Date { get; set; }
        public DateTime OtherDate { get; set; }
        public IFormatProvider FormatProvider { get; set; }
    }

    private ScenarioInput _input;
    private ScenarioResult<DateTime> _result;
    private ScenarioResult<int> _intResult;

    public DateTimeExtensionsSteps(ScenarioInput input, ScenarioResult<DateTime> result, ScenarioResult<int> intResult)
    {
        _input = input;
        _result = result;
        _intResult = intResult;
    }

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
            _result.Value = _input.Date.FirstDayOfWeek(_input.FormatProvider);
        }
        else
        {
            _result.Value = _input.Date.FirstDayOfWeek();
        }
    }

    [When(@"I call LastDayOfWeek")]
    public void WhenICallLastDayOfWeek()
    {
        if (_input.FormatProvider != null)
        {
            _result.Value = _input.Date.LastDayOfWeek(_input.FormatProvider);
        }
        else
        {
            _result.Value = _input.Date.LastDayOfWeek();
        }
    }

    [When(@"I call DifferenceInMonths")]
    public void WhenICallDifferenceInMonths()
    {
        _intResult.Value = _input.Date.DifferenceInMonths(_input.OtherDate);
    }

}
