using Asm.AspNetCore.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreServiceCollectionExtensions.AddSecurityReporting"/> —
/// isolated from the existing Reqnroll-based IServiceCollectionExtensions feature tests.
/// </summary>
[Trait("Category", "Unit")]
public class IServiceCollectionExtensionsReportingTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // AddSecurityReporting with no configure → defaults
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting is called without configuration
    /// When the SecurityReportingOptions are resolved
    /// Then the default route, group name and max-age values are registered
    /// </summary>
    [Fact]
    public void AddSecurityReportingNoConfigurationRegistersDefaultOptions()
    {
        var services = new ServiceCollection();
        services.AddSecurityReporting();

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<SecurityReportingOptions>();

        Assert.NotNull(options);
        Assert.Equal("reporting", options.RoutePrefix);
        Assert.Equal("integrity", options.IntegrityRoute);
        Assert.Equal("csp", options.CspRoute);
        Assert.Equal("integrity-endpoint", options.IntegrityGroupName);
        Assert.Equal("csp-endpoint", options.CspGroupName);
        Assert.Equal(86400, options.MaxAgeSeconds);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AddSecurityReporting with configure → custom options reflected
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting is called with a configure callback
    /// When the SecurityReportingOptions are resolved
    /// Then the customised option values are reflected
    /// </summary>
    [Fact]
    public void AddSecurityReportingWithConfigureRegistersCustomisedOptions()
    {
        var services = new ServiceCollection();
        services.AddSecurityReporting(opts => opts.RoutePrefix = "x");

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<SecurityReportingOptions>();

        Assert.Equal("x", options.RoutePrefix);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Singleton — same instance returned on multiple resolutions
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting has registered the options
    /// When SecurityReportingOptions is resolved more than once
    /// Then the same singleton instance is returned each time
    /// </summary>
    [Fact]
    public void AddSecurityReportingReturnsSameSingletonInstance()
    {
        var services = new ServiceCollection();
        services.AddSecurityReporting();

        var provider = services.BuildServiceProvider();
        var first = provider.GetRequiredService<SecurityReportingOptions>();
        var second = provider.GetRequiredService<SecurityReportingOptions>();

        Assert.Same(first, second);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Method returns the same IServiceCollection for chaining
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a service collection
    /// When AddSecurityReporting is called
    /// Then the same service collection instance is returned for chaining
    /// </summary>
    [Fact]
    public void AddSecurityReportingReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var returned = services.AddSecurityReporting();

        Assert.Same(services, returned);
    }
}
