namespace Asm.AspNetCore.Tests.Integration;

[Binding]
public class IntegrationTestHooks(IntegrationTestContext context)
{
    [AfterScenario("Integration")]
    public void AfterIntegrationScenario()
    {
        context.Dispose();
    }
}
