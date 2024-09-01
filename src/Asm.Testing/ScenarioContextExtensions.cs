using TechTalk.SpecFlow;

namespace Asm.Testing;

/// <summary>
/// Extensions for the <see cref="ScenarioContext"/> class.
/// </summary>
public static class ScenarioContextExtensions
{
    /// <summary>
    /// Adds a common result to the context, to be consumed by the <see cref="SimpleAssertionSteps" />.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <param name="result">The result to add.</param>
    public static void AddResult<T>(this ScenarioContext context, T? result)
        => context.Add(SimpleAssertionSteps.ResultKey, result);

    /// <summary>
    /// Gets the current result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    public static T GetResult<T>(this ScenarioContext context)
        => context.Get<T>(SimpleAssertionSteps.ResultKey);

    /// <summary>
    /// Adds a common result to the context, to be consumed by the <see cref="SimpleAssertionSteps" />.
    /// </summary>
    /// <typeparam name="T">The type of the exception.</typeparam>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <param name="exception">The exception to add.</param>
    public static void AddException<T>(this ScenarioContext context, T? exception) where T : Exception
        => context.Add(ExceptionSteps.ExceptionKey, exception);

    /// <summary>
    /// Gets the current exception.
    /// </summary>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <returns>The current exception or <c>null</c>.</returns>
    public static Exception? GetException(this ScenarioContext context)
    {
        context.TryGetValue(ExceptionSteps.ExceptionKey, out Exception? value);
        return value;
    }

    /// <summary>
    /// Gets the current exception.
    /// </summary>
    /// <typeparam name="T">The type of the exception.</typeparam>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <returns>The current exception or <c>null</c>.</returns>
    public static T? GetException<T>(this ScenarioContext context) where T : Exception
    {
        context.TryGetValue(ExceptionSteps.ExceptionKey, out T? value);
        return value;
    }

    /// <summary>
    /// Executes the action and catches any exceptions, adding them to the context.
    /// </summary>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <param name="testCode">The action to perform.</param>
    public static void CatchException(this ScenarioContext context, Action testCode)
    {
        var exception = Record.Exception(testCode);

        context.AddException(exception);
    }

    /// <summary>
    /// Executes the action and catches any exceptions, adding them to the context.
    /// </summary>
    /// <param name="context">The <see cref="ScenarioContext"/> object that this method extends.</param>
    /// <param name="testCode">The function to run.</param>
    /// <returns></returns>
    public static async Task CatchExceptionAsync(this ScenarioContext context, Func<Task> testCode)
    {
        var exception = await Record.ExceptionAsync(testCode);

        context.AddException(exception);
    }
}
