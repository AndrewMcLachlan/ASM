using Asm.Umbraco.MachineInfo;
using Microsoft.AspNetCore.DataProtection.Infrastructure;
using Microsoft.Extensions.Options;

namespace Asm.Umbraco.Tests.MachineInfo;

public class FixedMachineInfoFactoryTests : IDisposable
{
    private const string TestEnvVar = "MACHINE_NAME";
    private const string CustomEnvVar = "CUSTOM_MACHINE_VAR";

    private readonly Mock<IApplicationDiscriminator> _discriminatorMock;

    public FixedMachineInfoFactoryTests()
    {
        _discriminatorMock = new Mock<IApplicationDiscriminator>();
        _discriminatorMock.Setup(d => d.Discriminator).Returns("test-app");

        // Clean up env vars before each test
        Environment.SetEnvironmentVariable(TestEnvVar, null);
        Environment.SetEnvironmentVariable(CustomEnvVar, null);
    }

    public void Dispose()
    {
        // Clean up after each test
        Environment.SetEnvironmentVariable(TestEnvVar, null);
        Environment.SetEnvironmentVariable(CustomEnvVar, null);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifier_WhenEnvVarSet_ReturnsEnvVarValue()
    {
        Environment.SetEnvironmentVariable(TestEnvVar, "env-machine-name");

        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "fallback-name" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        Assert.Equal("env-machine-name", factory.GetMachineIdentifier());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifier_WhenEnvVarNotSet_ReturnsFallbackMachineName()
    {
        Environment.SetEnvironmentVariable(TestEnvVar, null);

        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "fallback-name" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        Assert.Equal("fallback-name", factory.GetMachineIdentifier());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifier_WhenCustomEnvVarSet_ReturnsCustomEnvVarValue()
    {
        Environment.SetEnvironmentVariable(CustomEnvVar, "custom-env-machine");

        var options = Options.Create(new FixedMachineInfoFactoryOptions
        {
            MachineName = "fallback-name",
            EnvironmentVariableName = CustomEnvVar
        });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        Assert.Equal("custom-env-machine", factory.GetMachineIdentifier());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetLocalIdentity_ContainsMachineNameDiscriminatorProcessIdDomainIdAndGuid()
    {
        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "test-machine" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        var identity = factory.GetLocalIdentity();

        Assert.Contains("test-app", identity);                  // discriminator
        Assert.Contains("[P", identity);                        // process id prefix
        Assert.Contains("/D", identity);                        // domain id prefix
        Assert.Matches(@"[0-9A-F]{32}", identity);              // GUID segment (N format, uppercased)
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetLocalIdentity_ReturnsSameValueOnMultipleCalls()
    {
        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "test-machine" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        var first = factory.GetLocalIdentity();
        var second = factory.GetLocalIdentity();

        Assert.Equal(first, second);
    }
}
