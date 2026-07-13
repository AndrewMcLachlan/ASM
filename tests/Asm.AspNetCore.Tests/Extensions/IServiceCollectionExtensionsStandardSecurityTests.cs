using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreServiceCollectionExtensions.AddStandardSecurityHeaders"/>.
/// </summary>
[Trait("Category", "Unit")]
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

        // The collection is keyed by header name; verify the expected headers are present.
        // CSP is intentionally NOT a default — consumers must opt in via the returned collection.
        Assert.False(policies.ContainsKey("Content-Security-Policy"), "CSP policy should NOT be registered by default");
        Assert.True(policies.ContainsKey("Cross-Origin-Opener-Policy"), "COOP policy should be registered");
        Assert.True(policies.ContainsKey("Cross-Origin-Embedder-Policy"), "COEP policy should be registered");
        Assert.True(policies.ContainsKey("Cross-Origin-Resource-Policy"), "CORP policy should be registered");
        Assert.True(policies.ContainsKey("X-Frame-Options"), "X-Frame-Options policy should be registered");
        Assert.True(policies.ContainsKey("X-Content-Type-Options"), "X-Content-Type-Options policy should be registered");
        Assert.True(policies.ContainsKey("Referrer-Policy"), "Referrer-Policy policy should be registered");
        Assert.True(policies.ContainsKey("X-Permitted-Cross-Domain-Policies"), "X-Permitted-Cross-Domain-Policies policy should be registered");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Return value — IServiceCollection for chaining
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var returned = services.AddStandardSecurityHeaders();

        Assert.Same(services, returned);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // configure callback — additional/overriding policies are composed in
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_ConfigureCallback_ContributesAdditionalPolicies()
    {
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders(policies => policies.AddCustomHeader("X-Custom-Test", "custom-value"));

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.True(resolved.ContainsKey("X-Custom-Test"), "The configure callback should contribute additional policies to the resolved collection.");
        // Standard defaults are still present alongside the custom policy.
        Assert.True(resolved.ContainsKey("X-Frame-Options"), "Standard defaults should remain when a configure callback is supplied.");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Order-independent reporting coupling: AddSecurityReporting BEFORE
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_AfterAddSecurityReporting_ReportingPoliciesIncluded()
    {
        var services = new ServiceCollection();
        services.AddSecurityReporting();
        services.AddStandardSecurityHeaders();

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.True(collection.ContainsKey("Reporting-Endpoints"),
            "Reporting-Endpoints policy should be coupled when AddSecurityReporting is called first");
        Assert.True(collection.ContainsKey("Report-To"),
            "Report-To policy should be coupled when AddSecurityReporting is called first");
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
    // Order-independent reporting coupling: AddSecurityReporting AFTER
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_BeforeAddSecurityReporting_ReportingPoliciesIncluded()
    {
        // Order-independence (item 2): AddSecurityReporting composes the reporting policies via an
        // IPostConfigureOptions, so calling it AFTER AddStandardSecurityHeaders still works.
        var services = new ServiceCollection();
        services.AddStandardSecurityHeaders();           // called first
        services.AddSecurityReporting();                 // still composes via IPostConfigureOptions

        var provider = services.BuildServiceProvider();
        var collection = provider.GetRequiredService<HeaderPolicyCollection>();

        Assert.True(collection.ContainsKey("Reporting-Endpoints"),
            "Reporting-Endpoints should be present regardless of registration order");
        Assert.True(collection.ContainsKey("Report-To"),
            "Report-To should be present regardless of registration order");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Order-independence: both registration orders produce the same composed result
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddStandardSecurityHeaders_ReportingCoupling_IsOrderIndependent()
    {
        var reportingFirst = new ServiceCollection();
        reportingFirst.AddSecurityReporting();
        reportingFirst.AddStandardSecurityHeaders();

        var headersFirst = new ServiceCollection();
        headersFirst.AddStandardSecurityHeaders();
        headersFirst.AddSecurityReporting();

        var a = reportingFirst.BuildServiceProvider().GetRequiredService<HeaderPolicyCollection>();
        var b = headersFirst.BuildServiceProvider().GetRequiredService<HeaderPolicyCollection>();

        // Same set of header keys regardless of registration order.
        Assert.Equal(a.Keys.OrderBy(k => k), b.Keys.OrderBy(k => k));
    }
}
