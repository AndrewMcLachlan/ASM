using Asm.Umbraco.Authentication.EntraId;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class IUmbracoBuilderExtensionsTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static (IUmbracoBuilder Builder, IServiceCollection Services) CreateBuilder()
    {
        var services = new ServiceCollection();
        services.AddOptions();
        var builderMock = new Mock<IUmbracoBuilder>();
        builderMock.Setup(b => b.Services).Returns(services);
        return (builderMock.Object, services);
    }

    // -----------------------------------------------------------------------
    // AddEntraIdAuthentication(Action<EntraIdOptions>)
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithAction_RegistersEntraIdLoginOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(o =>
        {
            o.TenantId = "t";
            o.ClientId = "c";
            o.ClientSecret = "s";
        });

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<BackOfficeExternalLoginProviderOptions>) &&
            sd.ImplementationType == typeof(EntraIdLoginOptions));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithAction_RegistersEntraIdBackOfficeOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(o =>
        {
            o.TenantId = "t";
            o.ClientId = "c";
            o.ClientSecret = "s";
        });

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<MicrosoftAccountOptions>) &&
            sd.ImplementationType == typeof(EntraIdBackOfficeOptions));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithAction_RegistersPackageManifestReader()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(o =>
        {
            o.TenantId = "t";
            o.ClientId = "c";
            o.ClientSecret = "s";
        });

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IPackageManifestReader) &&
            sd.ImplementationType == typeof(EntraIdManifestReader));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithAction_BindsEntraIdOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(o =>
        {
            o.TenantId = "test-tenant";
            o.ClientId = "test-client";
            o.ClientSecret = "test-secret";
        });

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<EntraIdOptions>>();

        Assert.Equal("test-tenant", options.Value.TenantId);
        Assert.Equal("test-client", options.Value.ClientId);
        Assert.Equal("test-secret", options.Value.ClientSecret);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithAction_ReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var returned = builder.AddEntraIdAuthentication(o =>
        {
            o.TenantId = "t";
            o.ClientId = "c";
            o.ClientSecret = "s";
        });

        Assert.Same(builder, returned);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithNullBuilder_ThrowsArgumentNullException()
    {
        IUmbracoBuilder nullBuilder = null;

        Assert.Throws<ArgumentNullException>(() =>
            nullBuilder.AddEntraIdAuthentication(o =>
            {
                o.TenantId = "t";
                o.ClientId = "c";
                o.ClientSecret = "s";
            }));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithNullAction_ThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.AddEntraIdAuthentication((Action<EntraIdOptions>)null));
    }

    // -----------------------------------------------------------------------
    // AddEntraIdAuthentication(IConfigurationSection)
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_RegistersEntraIdLoginOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<BackOfficeExternalLoginProviderOptions>) &&
            sd.ImplementationType == typeof(EntraIdLoginOptions));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_RegistersEntraIdBackOfficeOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<MicrosoftAccountOptions>) &&
            sd.ImplementationType == typeof(EntraIdBackOfficeOptions));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_RegistersPackageManifestReader()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IPackageManifestReader) &&
            sd.ImplementationType == typeof(EntraIdManifestReader));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_BindsEntraIdOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection(
            tenantId: "section-tenant",
            clientId: "section-client",
            clientSecret: "section-secret"));

        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<EntraIdOptions>>();

        Assert.Equal("section-tenant", options.Value.TenantId);
        Assert.Equal("section-client", options.Value.ClientId);
        Assert.Equal("section-secret", options.Value.ClientSecret);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_ReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var returned = builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Same(builder, returned);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithNullConfigSection_ThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.AddEntraIdAuthentication((IConfigurationSection)null));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthentication_WithConfigSection_NullBuilder_ThrowsArgumentNullException()
    {
        IUmbracoBuilder nullBuilder = null;

        Assert.Throws<ArgumentNullException>(() =>
            nullBuilder.AddEntraIdAuthentication(BuildConfigSection()));
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static IConfigurationSection BuildConfigSection(
        string tenantId = "default-tenant",
        string clientId = "default-client",
        string clientSecret = "default-secret")
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["EntraId:TenantId"] = tenantId,
                ["EntraId:ClientId"] = clientId,
                ["EntraId:ClientSecret"] = clientSecret
            })
            .Build();

        return config.GetSection("EntraId");
    }
}
