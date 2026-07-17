using Asm.Umbraco.MachineInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Factories;

namespace Asm.Umbraco.Tests.MachineInfo;

public class IUmbracoBuilderExtensionsTests
{
    private static (IUmbracoBuilder Builder, IServiceCollection Services) CreateBuilder()
    {
        var services = new ServiceCollection();
        var builderMock = new Mock<IUmbracoBuilder>();
        builderMock.Setup(b => b.Services).Returns(services);
        return (builderMock.Object, services);
    }

    /// <summary>
    /// Given an Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration action
    /// Then a service descriptor mapping IMachineInfoFactory to FixedMachineInfoFactory is registered
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithActionConfigureRegistersMachineInfoFactory()
    {
        var (builder, services) = CreateBuilder();

        builder.AddFixedMachineInfoFactory(opts =>
        {
            opts.MachineName = "configured-machine";
        });

        // Verify that a service descriptor for IMachineInfoFactory → FixedMachineInfoFactory was registered.
        // We check the descriptor rather than resolving the service to avoid needing IApplicationDiscriminator.
        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IMachineInfoFactory) &&
            sd.ImplementationType == typeof(FixedMachineInfoFactory));
    }

    /// <summary>
    /// Given an Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration action that sets options
    /// Then the resolved FixedMachineInfoFactoryOptions reflect the configured values
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithActionConfigureBindsOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddFixedMachineInfoFactory(opts =>
        {
            opts.MachineName = "configured-machine";
            opts.EnvironmentVariableName = "CUSTOM_VAR";
        });

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<FixedMachineInfoFactoryOptions>>();
        Assert.Equal("configured-machine", options.Value.MachineName);
        Assert.Equal("CUSTOM_VAR", options.Value.EnvironmentVariableName);
    }

    /// <summary>
    /// Given an Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration action
    /// Then the same builder instance is returned to allow fluent chaining
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithActionConfigureReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var returned = builder.AddFixedMachineInfoFactory(opts => opts.MachineName = "machine");

        Assert.Same(builder, returned);
    }

    /// <summary>
    /// Given an Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration section
    /// Then a service descriptor mapping IMachineInfoFactory to FixedMachineInfoFactory is registered
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithConfigSectionRegistersMachineInfoFactory()
    {
        var (builder, services) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MachineInfo:MachineName"] = "section-machine",
                ["MachineInfo:EnvironmentVariableName"] = "SECTION_VAR"
            })
            .Build();

        builder.AddFixedMachineInfoFactory(config.GetSection("MachineInfo"));

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IMachineInfoFactory) &&
            sd.ImplementationType == typeof(FixedMachineInfoFactory));
    }

    /// <summary>
    /// Given an Umbraco builder and a configuration section with machine info values
    /// When AddFixedMachineInfoFactory is called with that section
    /// Then the resolved FixedMachineInfoFactoryOptions are bound from the section values
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithConfigSectionBindsOptions()
    {
        var (builder, services) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MachineInfo:MachineName"] = "section-machine",
                ["MachineInfo:EnvironmentVariableName"] = "SECTION_VAR"
            })
            .Build();

        builder.AddFixedMachineInfoFactory(config.GetSection("MachineInfo"));

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<FixedMachineInfoFactoryOptions>>();
        Assert.Equal("section-machine", options.Value.MachineName);
        Assert.Equal("SECTION_VAR", options.Value.EnvironmentVariableName);
    }

    /// <summary>
    /// Given an Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration section
    /// Then the same builder instance is returned to allow fluent chaining
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithConfigSectionReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["MachineInfo:MachineName"] = "m" })
            .Build();

        var returned = builder.AddFixedMachineInfoFactory(config.GetSection("MachineInfo"));

        Assert.Same(builder, returned);
    }

    /// <summary>
    /// Given a null Umbraco builder
    /// When AddFixedMachineInfoFactory is called with a configuration action
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithNullBuilderThrowsArgumentNullException()
    {
        IUmbracoBuilder nullBuilder = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullBuilder.AddFixedMachineInfoFactory(opts => opts.MachineName = "x"));
    }

    /// <summary>
    /// Given a valid Umbraco builder but a null configuration action
    /// When AddFixedMachineInfoFactory is called
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactoryWithNullConfigureThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFixedMachineInfoFactory((Action<FixedMachineInfoFactoryOptions>)null!));
    }
}
