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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action.
    /// Then EntraIdLoginOptions is registered as IConfigureOptions of BackOfficeExternalLoginProviderOptions.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithActionRegistersEntraIdLoginOptions()
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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action.
    /// Then EntraIdBackOfficeOptions is registered as IConfigureOptions of MicrosoftAccountOptions.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithActionRegistersEntraIdBackOfficeOptions()
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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action.
    /// Then EntraIdManifestReader is registered as an IPackageManifestReader.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithActionRegistersPackageManifestReader()
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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action setting tenant, client and secret.
    /// Then the resolved EntraIdOptions carry those bound values.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithActionBindsEntraIdOptions()
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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action.
    /// Then the same builder instance is returned.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithActionReturnsBuilder()
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

    /// <summary>
    /// Given a null Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configure action.
    /// Then an ArgumentNullException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithNullBuilderThrowsArgumentNullException()
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

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a null configure action.
    /// Then an ArgumentNullException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithNullActionThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.AddEntraIdAuthentication((Action<EntraIdOptions>)null));
    }

    // -----------------------------------------------------------------------
    // AddEntraIdAuthentication(IConfigurationSection)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Given an Umbraco builder and a configuration section.
    /// When AddEntraIdAuthentication is called with that section.
    /// Then EntraIdLoginOptions is registered as IConfigureOptions of BackOfficeExternalLoginProviderOptions.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionRegistersEntraIdLoginOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<BackOfficeExternalLoginProviderOptions>) &&
            sd.ImplementationType == typeof(EntraIdLoginOptions));
    }

    /// <summary>
    /// Given an Umbraco builder and a configuration section.
    /// When AddEntraIdAuthentication is called with that section.
    /// Then EntraIdBackOfficeOptions is registered as IConfigureOptions of MicrosoftAccountOptions.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionRegistersEntraIdBackOfficeOptions()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IConfigureOptions<MicrosoftAccountOptions>) &&
            sd.ImplementationType == typeof(EntraIdBackOfficeOptions));
    }

    /// <summary>
    /// Given an Umbraco builder and a configuration section.
    /// When AddEntraIdAuthentication is called with that section.
    /// Then EntraIdManifestReader is registered as an IPackageManifestReader.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionRegistersPackageManifestReader()
    {
        var (builder, services) = CreateBuilder();

        builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Contains(services, sd =>
            sd.ServiceType == typeof(IPackageManifestReader) &&
            sd.ImplementationType == typeof(EntraIdManifestReader));
    }

    /// <summary>
    /// Given an Umbraco builder and a configuration section with tenant, client and secret values.
    /// When AddEntraIdAuthentication is called with that section.
    /// Then the resolved EntraIdOptions carry those bound values.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionBindsEntraIdOptions()
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

    /// <summary>
    /// Given an Umbraco builder and a configuration section.
    /// When AddEntraIdAuthentication is called with that section.
    /// Then the same builder instance is returned.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionReturnsBuilder()
    {
        var (builder, _) = CreateBuilder();

        var returned = builder.AddEntraIdAuthentication(BuildConfigSection());

        Assert.Same(builder, returned);
    }

    /// <summary>
    /// Given an Umbraco builder.
    /// When AddEntraIdAuthentication is called with a null configuration section.
    /// Then an ArgumentNullException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithNullConfigSectionThrowsArgumentNullException()
    {
        var (builder, _) = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.AddEntraIdAuthentication((IConfigurationSection)null));
    }

    /// <summary>
    /// Given a null Umbraco builder.
    /// When AddEntraIdAuthentication is called with a configuration section.
    /// Then an ArgumentNullException is thrown.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddEntraIdAuthenticationWithConfigSectionNullBuilderThrowsArgumentNullException()
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
