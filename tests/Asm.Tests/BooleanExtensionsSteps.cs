using System;
using Asm.Extensions;
using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Tests
{
    [Binding]
    public class BooleanExtensionsSteps
    {
        private readonly ScenarioInput<bool> _input;
        private readonly ScenarioResult<int> _intResult;
        private readonly ScenarioResult<string> _strResult;

        public BooleanExtensionsSteps(ScenarioInput<bool> input, ScenarioResult<int> intResult, ScenarioResult<string> strResult)
        {
            _input = input;
            _intResult = intResult;
            _strResult = strResult;
        }

        [Given(@"I have boolean that is '(.*)'")]
        public void GivenIHaveBooleanThatIs(bool value)
        {
            _input.Input = value;
        }

        [When(@"I call ToNumeric")]
        public void WhenICallToNumeric()
        {
            _intResult.Result = _input.Input.ToNumeric();
        }

        [When(@"I call ToNumericString")]
        public void WhenICallToNumericString()
        {
            _strResult.Result = _input.Input.ToNumericString();
        }
    }
}
