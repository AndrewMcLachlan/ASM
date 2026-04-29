using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="IServiceCollectionExtensions.AddStandardSecurityHeaders"/>.
/// </summary>
public class IServiceCollectionExtensionsStandardSecurityTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Null guard
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() => services.AddStandardSecurityHeaders());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Default registration
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_NoExtend_RegistersHeaderPolicyCollectionSingleton()
    {
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders();

        var provider = services.BuildServiceProvider();
        var policies = provider.GetService<HeaderPolicyCollection>();

        Assert.NotNull(policies);
    }

    [Fact]
    public void AddStandardSecurityHeaders_NoExtend_CollectionContainsDefaultPolicies()
    {
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders();

        var provider = services.BuildServiceProvider();
        var policies = provider.GetRequiredService<HeaderPolicyCollection>();

        // The collection is keyed by header name; verify the expected headers are present
        Assert.True(policies.ContainsKey("Content-Security-Policy"), "CSP policy should be registered");
        Assert.True(policies.ContainsKey("Cross-Origin-Opener-Policy"), "COOP policy should be registered");
        Assert.True(policies.ContainsKey("Cross-Origin-Embedder-Policy"), "COEP policy should be registered");
        Assert.True(policies.ContainsKey("Cross-Origin-Resource-Policy"), "CORP policy should be registered");
        Assert.True(policies.ContainsKey("X-Frame-Options"), "X-Frame-Options policy should be registered");
        Assert.True(policies.ContainsKey("X-Content-Type-Options"), "X-Content-Type-Options policy should be registered");
        Assert.True(policies.ContainsKey("Referrer-Policy"), "Referrer-Policy policy should be registered");
        Assert.True(policies.ContainsKey("X-Permitted-Cross-Domain-Policies"), "X-Permitted-Cross-Domain-Policies policy should be registered");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // extend callback
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_WithExtend_CustomHeaderInCollection()
    {
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders(policies => policies.AddCustomHeader("X-Custom-Test", "custom-value"));

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.True(collection.ContainsKey("X-Custom-Test"), "Custom header should be present after extend callback");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Auto-coupling: AddSecurityReporting BEFORE AddStandardSecurityHeaders
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_AfterAddSecurityReporting_ReportingPoliciesIncluded()
    {
        var services = new ServiceCollection();
        services.AddSecurityReporting();                 // must come first
        services.AddStandardSecurityHeaders();

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.True(collection.ContainsKey("Reporting-Endpoints"),
            "Reporting-Endpoints policy should be auto-coupled when AddSecurityReporting is called first");
        Assert.True(collection.ContainsKey("Report-To"),
            "Report-To policy should be auto-coupled when AddSecurityReporting is called first");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Auto-coupling: AddSecurityReporting NOT called → reporting policies absent
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_WithoutAddSecurityReporting_ReportingPoliciesAbsent()
    {
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders();           // no AddSecurityReporting

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.False(collection.ContainsKey("Reporting-Endpoints"),
            "Reporting-Endpoints should not be present when AddSecurityReporting was not called");
        Assert.False(collection.ContainsKey("Report-To"),
            "Report-To should not be present when AddSecurityReporting was not called");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Auto-coupling: AddSecurityReporting AFTER AddStandardSecurityHeaders → NOT coupled
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_BeforeAddSecurityReporting_ReportingPoliciesNotPresent()
    {
        // Documents the ordering requirement: AddSecurityReporting must come BEFORE
        // AddStandardSecurityHeaders for auto-coupling to take effect.
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders();           // called first
        services.AddSecurityReporting();                 // too late — no effect on the already-built collection

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.False(collection.ContainsKey("Reporting-Endpoints"),
            "Reporting-Endpoints should be absent when AddSecurityReporting is called AFTER AddStandardSecurityHeaders");
        Assert.False(collection.ContainsKey("Report-To"),
            "Report-To should be absent when AddSecurityReporting is called AFTER AddStandardSecurityHeaders");
    }
}
