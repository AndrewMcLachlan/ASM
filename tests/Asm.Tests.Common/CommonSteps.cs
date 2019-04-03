using System;
using TechTalk.SpecFlow;
using Xunit;
using static Asm.Tests.Common.SpecFlowHelper;

namespace Asm.Tests.Common
{
    [Binding]
    public class CommonSteps
    {
        public const string ExceptionKey = "ExpectedException";

        [Then(@"an exception of type '(.*)' is thrown")]
        public void ThenAnExceptionofTypeIsThrown(string exceptionType)
        {
            Type expected = Type.GetType(exceptionType, true);

            var actual = ScenarioContext.Current.Get<Exception>(ExceptionKey);

            Assert.NotNull(actual);
            Assert.IsType(expected, actual);
        }

        [Then(@"the exception message is '(.*)'")]
        public void ThenTheExceptionMessageIs(string message)
        {
            var exception = ScenarioContext.Current.Get<Exception>(ExceptionKey);

            Assert.NotNull(exception);
            Assert.Equal(message.SpecFlowProcess(), exception.Message);
        }

        [Then(@"the exception parameter name is '(.*)'")]
        public void ThenTheExceptionParameterNAmeIs(string parameterName)
        {
            var exception = ScenarioContext.Current.Get<ArgumentException>(ExceptionKey);

            Assert.NotNull(exception);
            Assert.Equal(parameterName, exception.ParamName);
        }
    }
}
