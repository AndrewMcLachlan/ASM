using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Tests.Common
{
    public static class SpecFlowHelper
    {
        public const string NullString = "<NULL>";

        public static string SpecFlowProcess(this string str)
        {
            if (str == null) return null;
            if (str == NullString) return null;

            return str.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t");
        }

        public static void CatchException(Action action)
        {
            var exception = Record.Exception(action);

            Assert.NotNull(ScenarioContext.Current);

            ScenarioContext.Current.Add(CommonSteps.ExceptionKey, exception);
        }
    }
}
