using Asm.Umbraco.MachineInfo;

namespace Asm.Umbraco.Tests.MachineInfo;

public class FixedMachineInfoFactoryOptionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void EnvironmentVariableName_DefaultsToMachineName()
    {
        var options = new FixedMachineInfoFactoryOptions { MachineName = "test-machine" };
        Assert.Equal("MACHINE_NAME", options.EnvironmentVariableName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MachineName_RoundTrip()
    {
        var options = new FixedMachineInfoFactoryOptions { MachineName = "my-server" };
        Assert.Equal("my-server", options.MachineName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void EnvironmentVariableName_CanBeOverridden()
    {
        var options = new FixedMachineInfoFactoryOptions
        {
            MachineName = "test-machine",
            EnvironmentVariableName = "CUSTOM_MACHINE_VAR"
        };
        Assert.Equal("CUSTOM_MACHINE_VAR", options.EnvironmentVariableName);
    }
}
