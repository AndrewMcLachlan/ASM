using Asm.AspNetCore.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="IServiceCollectionExtensions.AddSecurityReporting"/> —
/// isolated from the existing Reqnroll-based IServiceCollectionExtensions feature tests.
/// </summary>
public class IServiceCollectionExtensionsReportingTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // AddSecurityReporting with no configure → defaults
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddSecurityReporting_NoConfiguration_RegistersDefaultOptions()
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

    [Fact]
    public void AddSecurityReporting_WithConfigure_RegistersCustomisedOptions()
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

    [Fact]
    public void AddSecurityReporting_ReturnsSameSingletonInstance()
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

    [Fact]
    public void AddSecurityReporting_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var returned = services.AddSecurityReporting();

        Assert.Same(services, returned);
    }
}
