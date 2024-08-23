namespace Asm.Testing;

public static class SpecFlowHelper
{
    public static string? DecodeWhitespace(this string str)
    {
        if (str == null) return null;

        return str.Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t");
    }

    public static void CatchException(this ScenarioContext context, Action action)
    {
        var exception = Record.Exception(action);

        context.AddException(exception);

    }

    public static async Task CatchExceptionAsync(this ScenarioContext context, Func<Task> testCode)
    {
        var exception = await Record.ExceptionAsync(testCode);

        context.AddException(exception);

    }
}
