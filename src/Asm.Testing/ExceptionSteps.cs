using System;
using TechTalk.SpecFlow;
using Xunit;
using static Asm.Testing.SpecFlowHelper;

namespace Asm.Testing
{
    [Binding]
    public class ExceptionSteps
    {
        private ScenarioData<Exception> _scenarioData;

        public ExceptionSteps(ScenarioData<Exception> scenarioData)
        {
            _scenarioData = scenarioData;
        }

        [Then(@"an exception of type '(.*)' is thrown")]
        public void ThenAnExceptionofTypeIsThrown(string exceptionType)
        {
            Type expected = Type.GetType(exceptionType, true);

            var actual = _scenarioData.Result;

            Assert.NotNull(actual);
            Assert.IsType(expected, actual);
        }

        [Then(@"the exception message is '(.*)'")]
        public void ThenTheExceptionMessageIs(string message)
        {
            Assert.NotNull(_scenarioData.Result);
            Assert.Equal(message.SpecFlowProcess(), _scenarioData.Result.Message);
        }

        [Then(@"the exception parameter name is '(.*)'")]
        public void ThenTheExceptionParameterNAmeIs(string parameterName)
        {
            var exception = _scenarioData.Result;

            Assert.NotNull(exception);
            Assert.IsAssignableFrom(typeof(ArgumentException), exception);
            Assert.Equal(parameterName, ((ArgumentException)exception).ParamName);
        }
    }
}
