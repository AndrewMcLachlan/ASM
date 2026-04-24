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

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithActionConfigure_RegistersIMachineInfoFactory()
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

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithActionConfigure_BindsOptions()
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

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithActionConfigure_ReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var returned = builder.AddFixedMachineInfoFactory(opts => opts.MachineName = "machine");

        Assert.Same(builder, returned);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithConfigSection_RegistersIMachineInfoFactory()
    {
        var (builder, services) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
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

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithConfigSection_BindsOptions()
    {
        var (builder, services) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
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

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_WithConfigSection_ReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["MachineInfo:MachineName"] = "m" })
            .Build();

        var returned = builder.AddFixedMachineInfoFactory(config.GetSection("MachineInfo"));

        Assert.Same(builder, returned);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_NullBuilder_ThrowsArgumentNullException()
    {
        IUmbracoBuilder nullBuilder = null;
        Assert.Throws<ArgumentNullException>(() =>
            nullBuilder.AddFixedMachineInfoFactory(opts => opts.MachineName = "x"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddFixedMachineInfoFactory_NullConfigure_ThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFixedMachineInfoFactory((Action<FixedMachineInfoFactoryOptions>)null));
    }
}
