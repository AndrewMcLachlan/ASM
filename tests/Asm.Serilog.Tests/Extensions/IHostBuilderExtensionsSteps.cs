#nullable enable
using Microsoft.Extensions.Hosting;

namespace Asm.Serilog.Tests.Extensions;

[Binding]
public class IHostBuilderExtensionsSteps
{
    private IHostBuilder _builder = null!;
    private IHostBuilder _result = null!;

    [Given(@"I have a HostBuilder")]
    public void GivenIHaveAHostBuilder()
    {
        _builder = Host.CreateDefaultBuilder();
    }

    [When(@"I call UseCustomSerilog")]
    public void WhenICallUseCustomSerilog()
    {
        _result = _builder.UseCustomSerilog();
    }

    [Then(@"the host builder should be returned")]
    public void ThenTheHostBuilderShouldBeReturned()
    {
        Assert.NotNull(_result);
        Assert.Same(_builder, _result);
    }
}
