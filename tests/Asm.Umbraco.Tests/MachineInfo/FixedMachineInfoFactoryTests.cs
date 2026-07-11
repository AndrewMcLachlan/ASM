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

    /// <summary>
    /// Given the configured environment variable is set
    /// When GetMachineIdentifier is called
    /// Then the environment variable value is returned in preference to the fallback machine name
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifierReturnsEnvVarValueWhenEnvVarSet()
    {
        Environment.SetEnvironmentVariable(TestEnvVar, "env-machine-name");

        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "fallback-name" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        Assert.Equal("env-machine-name", factory.GetMachineIdentifier());
    }

    /// <summary>
    /// Given the configured environment variable is not set
    /// When GetMachineIdentifier is called
    /// Then the fallback machine name from options is returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifierReturnsFallbackMachineNameWhenEnvVarNotSet()
    {
        Environment.SetEnvironmentVariable(TestEnvVar, null);

        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "fallback-name" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        Assert.Equal("fallback-name", factory.GetMachineIdentifier());
    }

    /// <summary>
    /// Given options specify a custom environment variable name that is set
    /// When GetMachineIdentifier is called
    /// Then the value of the custom environment variable is returned
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetMachineIdentifierReturnsCustomEnvVarValueWhenCustomEnvVarSet()
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

    /// <summary>
    /// Given a factory built with a discriminator and machine name options
    /// When GetLocalIdentity is called
    /// Then the returned identity contains the discriminator, process id prefix, domain id prefix and a GUID segment
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetLocalIdentityContainsDiscriminatorProcessIdDomainIdAndGuid()
    {
        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "test-machine" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        var identity = factory.GetLocalIdentity();

        Assert.Contains("test-app", identity);                  // discriminator
        Assert.Contains("[P", identity);                        // process id prefix
        Assert.Contains("/D", identity);                        // domain id prefix
        Assert.Matches(@"[0-9A-F]{32}", identity);              // GUID segment (N format, uppercased)
    }

    /// <summary>
    /// Given a single factory instance
    /// When GetLocalIdentity is called more than once
    /// Then the same identity value is returned on every call
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GetLocalIdentityReturnsSameValueOnMultipleCalls()
    {
        var options = Options.Create(new FixedMachineInfoFactoryOptions { MachineName = "test-machine" });
        var factory = new FixedMachineInfoFactory(_discriminatorMock.Object, options);

        var first = factory.GetLocalIdentity();
        var second = factory.GetLocalIdentity();

        Assert.Equal(first, second);
    }
}
