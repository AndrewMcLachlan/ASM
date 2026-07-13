using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Reporting;

[Trait("Category", "Unit")]

public class SecurityReportingHeaderBuilderTests
{
    // Build a minimal HttpContext with a known scheme and host
    private static HttpContext BuildContext(string scheme = "https", string host = "example.com")
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Scheme = scheme;
        ctx.Request.Host = new HostString(host);
        return ctx;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // BuildReportingEndpoints
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a context and default options
    /// When BuildReportingEndpoints is called
    /// Then the result contains both group names and their endpoint URLs
    /// </summary>
    [Fact]
    public void BuildReportingEndpointsContainsBothGroupNamesAndUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions();

        var result = SecurityReportingHeaderBuilder.BuildReportingEndpoints(ctx, options);

        Assert.Contains(options.IntegrityGroupName, result);
        Assert.Contains(options.CspGroupName, result);
        Assert.Contains("https://example.com/reporting/integrity", result);
        Assert.Contains("https://example.com/reporting/csp", result);
    }

    /// <summary>
    /// Given options with a custom route prefix
    /// When BuildReportingEndpoints is called
    /// Then the custom prefix is reflected in the endpoint URLs
    /// </summary>
    [Fact]
    public void BuildReportingEndpointsCustomRoutePrefixReflectedInUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };

        var result = SecurityReportingHeaderBuilder.BuildReportingEndpoints(ctx, options);

        Assert.Contains("https://example.com/api/reporting/integrity", result);
        Assert.Contains("https://example.com/api/reporting/csp", result);
    }

    /// <summary>
    /// Given options with a route prefix containing leading and trailing slashes
    /// When BuildReportingEndpoints is called
    /// Then the slashes are handled gracefully and the URLs are well-formed
    /// </summary>
    [Fact]
    public void BuildReportingEndpointsRoutePrefixWithLeadingSlashWorksCorrectly()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { RoutePrefix = "/reporting/" };

        var result = SecurityReportingHeaderBuilder.BuildReportingEndpoints(ctx, options);

        // Leading/trailing slashes in RoutePrefix should be handled gracefully
        Assert.Contains("https://example.com/reporting/integrity", result);
        Assert.Contains("https://example.com/reporting/csp", result);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // BuildReportTo
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a context and default options
    /// When BuildReportTo is called
    /// Then the result contains both group names, the max-age and the endpoint URLs
    /// </summary>
    [Fact]
    public void BuildReportToContainsBothGroupsMaxAgeAndUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions();

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        Assert.Contains(options.IntegrityGroupName, result);
        Assert.Contains(options.CspGroupName, result);
        Assert.Contains(options.MaxAgeSeconds.ToString(), result);
        Assert.Contains("https://example.com/reporting/integrity", result);
        Assert.Contains("https://example.com/reporting/csp", result);
    }

    /// <summary>
    /// Given options with a custom max-age
    /// When BuildReportTo is called
    /// Then the custom max-age is reflected in the output
    /// </summary>
    [Fact]
    public void BuildReportToCustomMaxAgeReflectedInOutput()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { MaxAgeSeconds = 3600 };

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        Assert.Contains("3600", result);
    }

    /// <summary>
    /// Given options with a custom route prefix
    /// When BuildReportTo is called
    /// Then the custom prefix is reflected in the endpoint URLs
    /// </summary>
    [Fact]
    public void BuildReportToCustomRoutePrefixReflectedInUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        Assert.Contains("https://example.com/api/reporting/integrity", result);
        Assert.Contains("https://example.com/api/reporting/csp", result);
    }

    /// <summary>
    /// Given a context and default options
    /// When BuildReportTo is called
    /// Then the output is JSON-like and contains the "group" key
    /// </summary>
    [Fact]
    public void BuildReportToContainsGroupKeyword()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions();

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        // Should be JSON-like objects with "group" key
        Assert.Contains("\"group\"", result);
    }
}
