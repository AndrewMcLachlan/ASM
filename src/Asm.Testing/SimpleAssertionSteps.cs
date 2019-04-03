using System;
using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Testing
{
    [Binding]
    public class SimpleAssertionSteps
    {
        private readonly ScenarioResult<string> _strResult;
        private readonly ScenarioResult<int> _intResult;
        private readonly ScenarioResult<DateTime> _dateResult;

        public SimpleAssertionSteps(ScenarioResult<string> strResult, ScenarioResult<int> intResult, ScenarioResult<DateTime> dateResult)
        {
            _strResult = strResult;
            _intResult = intResult;
            _dateResult = dateResult;
        }

        [Then(@"the string value '(.*)' is returned")]
        public void ThenTheStringValueIsReturned(string expected)
        {
            Assert.NotNull(_intResult);
            Assert.Equal(expected, _strResult.Result);
        }

        [Then(@"the integer value (.*) is returned")]
        public void ThenTheIntegerValueIsReturned(int expected)
        {
            Assert.NotNull(_intResult);
            Assert.Equal(expected, _intResult.Result);
        }


        [Then(@"the a date '(.*)' will be returned")]
        public void ThenTheADateWillBeReturned(DateTime expected)
        {
            Assert.NotNull(_dateResult);
            Assert.Equal(expected, _dateResult.Result);
        }
    }
}
