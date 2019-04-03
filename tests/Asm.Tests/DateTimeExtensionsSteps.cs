using System;
using System.Globalization;
using Asm.Extensions;
using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Tests
{
    [Binding]
    public class DateTimeExtensionsSteps
    {
        public class ScenarioInput
        {
            public DateTime Date { get; set; }
            public IFormatProvider FormatProvider { get; set; }
        }

        private ScenarioInput _input;
        private ScenarioResult<DateTime> _result;

        public DateTimeExtensionsSteps(ScenarioInput input, ScenarioResult<DateTime> result)
        {
            _input = input;
            _result = result;
        }

        [Given(@"I have date '(.*)'")]
        public void GivenIHaveDate(DateTime date)
        {
            _input.Date = date;
        }

        [Given(@"my locale is '(.*)'")]
        public void GivenMyLocaleIs(string locale)
        {
            _input.FormatProvider = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
        }


        [When(@"I call FirstDayOfWeek")]
        public void WhenICallFirstDayOfWeek()
        {
            if (_input.FormatProvider != null)
            {
                _result.Result = _input.Date.FirstDayOfWeek(_input.FormatProvider);
            }
            else
            {
                _result.Result = _input.Date.FirstDayOfWeek();
            }
        }

        [When(@"I call LastDayOfWeek")]
        public void WhenICallLastDayOfWeek()
        {
            if (_input.FormatProvider != null)
            {
                _result.Result = _input.Date.LastDayOfWeek(_input.FormatProvider);
            }
            else
            {
                _result.Result = _input.Date.LastDayOfWeek();
            }
        }

    }
}
