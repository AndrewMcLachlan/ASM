using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Reporting;

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

    [Fact]
    public void BuildReportingEndpoints_ContainsBothGroupNamesAndUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions();

        var result = SecurityReportingHeaderBuilder.BuildReportingEndpoints(ctx, options);

        Assert.Contains(options.IntegrityGroupName, result);
        Assert.Contains(options.CspGroupName, result);
        Assert.Contains("https://example.com/reporting/integrity", result);
        Assert.Contains("https://example.com/reporting/csp", result);
    }

    [Fact]
    public void BuildReportingEndpoints_CustomRoutePrefix_ReflectedInUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };

        var result = SecurityReportingHeaderBuilder.BuildReportingEndpoints(ctx, options);

        Assert.Contains("https://example.com/api/reporting/integrity", result);
        Assert.Contains("https://example.com/api/reporting/csp", result);
    }

    [Fact]
    public void BuildReportingEndpoints_RoutePrefixWithLeadingSlash_WorksCorrectly()
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

    [Fact]
    public void BuildReportTo_ContainsBothGroups_MaxAge_AndUrls()
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

    [Fact]
    public void BuildReportTo_CustomMaxAge_ReflectedInOutput()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { MaxAgeSeconds = 3600 };

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        Assert.Contains("3600", result);
    }

    [Fact]
    public void BuildReportTo_CustomRoutePrefix_ReflectedInUrls()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions { RoutePrefix = "api/reporting" };

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        Assert.Contains("https://example.com/api/reporting/integrity", result);
        Assert.Contains("https://example.com/api/reporting/csp", result);
    }

    [Fact]
    public void BuildReportTo_ContainsGroupKeyword()
    {
        var ctx = BuildContext();
        var options = new SecurityReportingOptions();

        var result = SecurityReportingHeaderBuilder.BuildReportTo(ctx, options);

        // Should be JSON-like objects with "group" key
        Assert.Contains("\"group\"", result);
    }
}
