using Asm.Umbraco.MachineInfo;

namespace Asm.Umbraco.Tests.MachineInfo;

public class FixedMachineInfoFactoryOptionsTests
{
    /// <summary>
    /// Given options created without specifying an environment variable name
    /// When EnvironmentVariableName is read
    /// Then it defaults to "MACHINE_NAME"
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EnvironmentVariableNameDefaultsToMachineName()
    {
        var options = new FixedMachineInfoFactoryOptions { MachineName = "test-machine" };
        Assert.Equal("MACHINE_NAME", options.EnvironmentVariableName);
    }

    /// <summary>
    /// Given options with MachineName set to a value
    /// When MachineName is read back
    /// Then it returns the value that was assigned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void MachineNameRoundTrips()
    {
        var options = new FixedMachineInfoFactoryOptions { MachineName = "my-server" };
        Assert.Equal("my-server", options.MachineName);
    }

    /// <summary>
    /// Given options that explicitly set EnvironmentVariableName
    /// When EnvironmentVariableName is read back
    /// Then it returns the overridden value rather than the default
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EnvironmentVariableNameCanBeOverridden()
    {
        var options = new FixedMachineInfoFactoryOptions
        {
            MachineName = "test-machine",
            EnvironmentVariableName = "CUSTOM_MACHINE_VAR"
        };
        Assert.Equal("CUSTOM_MACHINE_VAR", options.EnvironmentVariableName);
    }
}
