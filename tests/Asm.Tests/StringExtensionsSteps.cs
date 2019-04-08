using System;
using Asm.Extensions;
using Asm.Testing;
using TechTalk.SpecFlow;

namespace Asm.Tests
{
    [Binding]
    public class StringExtensionsSteps
    {
        private readonly ScenarioResult<string> _result;
        private string _input;
        private string _separator;

        public StringExtensionsSteps(ScenarioResult<string> result)
        {
            _result = result;
        }

        [Given(@"I have a string '(.*)'")]
        public void GivenIHaveAString(string input)
        {
            _input = input;
        }

        [Given(@"I have a separator '(.*)'")]
        public void GivenIHaveASeparator(string separator)
        {
            _separator = separator;
        }

        [When(@"I Append '(.*)' to the string")]
        public void WhenIAppendToTheString(string append)
        {
            _result.Result = _input.Append(append, _separator);
        }
    }
}
