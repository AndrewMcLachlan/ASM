using System;
using Asm.Extensions;
using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Tests
{
    [Binding]
    public class DecimalExtensionsSteps
    {
        private readonly ScenarioResult<string> _result;
        private readonly ScenarioResult<Exception> _exception;

        private decimal _input;
        private int? _places;

        public DecimalExtensionsSteps(ScenarioResult<string> result, ScenarioResult<Exception> exception)
        {
            _result = result;
            _exception = exception;
        }

        [Given(@"I have a decimal (.*)")]
        public void GivenIHaveADecimal(decimal value)
        {
            _input = value;
        }

        [Given(@"I want to round to (.*) decimal places")]
        public void GivenIWantToRoundToDecimalPlaces(int value)
        {
            _places = value;
        }

        [When(@"I call ToRoundedCurrencyString")]
        public void WhenICallToRoundedCurrencyString()
        {
            _result.Result = _places != null ? _input.ToRoundedCurrencyString(_places.Value) : _input.ToRoundedCurrencyString();
        }

        [When(@"I call ToRoundedCurrencyString expecting an exception")]
        public void WhenICallToRoundedCurrencyStringExpectingAnException()
        {
            SpecFlowHelper.CatchException(() => _input.ToRoundedCurrencyString(_places.Value), _exception);
        }

    }
}
