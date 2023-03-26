using Xunit;

namespace Asm.Testing;

public static class SpecFlowHelper
{
    public static string? DecodeWhitespace(this string str)
    {
        if (str == null) return null;

        return str.Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t");
    }

    public static void CatchException(Action action, ScenarioResult<Exception> scenarioData)
    {
        var exception = Record.Exception(action);

        Assert.NotNull(scenarioData);

        scenarioData.Result = exception;

    }
}
