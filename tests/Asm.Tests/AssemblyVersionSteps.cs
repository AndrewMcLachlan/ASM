namespace Asm.Tests;

[Binding]
public class AssemblyVersionSteps(ScenarioContext context)
{
    private Exception? _exception;

    [When(@"I access the AssemblyVersion\.Version property")]
    public void WhenIAccessTheAssemblyVersionVersionProperty()
    {
        try
        {
            _ = System.AssemblyVersion.Version;
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"I access the AssemblyVersion\.FileVersion property")]
    public void WhenIAccessTheAssemblyVersionFileVersionProperty()
    {
        try
        {
            _ = System.AssemblyVersion.FileVersion;
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"I access the AssemblyVersion\.InformationalVersion property")]
    public void WhenIAccessTheAssemblyVersionInformationalVersionProperty()
    {
        try
        {
            _ = System.AssemblyVersion.InformationalVersion;
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the property access should not throw")]
    public void ThenThePropertyAccessShouldNotThrow()
    {
        Assert.Null(_exception);
    }
}
