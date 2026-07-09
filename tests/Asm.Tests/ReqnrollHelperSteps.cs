namespace Asm.Tests;

/// <summary>
/// Exercises the shared Asm.Reqnroll helpers (exception-type resolution and repeated
/// <c>CatchException</c> calls) that the Phase 1h fixes address.
/// </summary>
[Binding]
public class ReqnrollHelperSteps(ScenarioContext context)
{
    [When(@"I catch an Asm NotFoundException")]
    public void WhenICatchAnAsmNotFoundException()
        => context.CatchException(() => throw new NotFoundException("missing"));

    [When(@"I catch an exception then catch another")]
    public void WhenICatchAnExceptionThenCatchAnother()
    {
        context.CatchException(() => throw new ArgumentException("first"));
        context.CatchException(() => throw new InvalidOperationException("second"));
    }
}
