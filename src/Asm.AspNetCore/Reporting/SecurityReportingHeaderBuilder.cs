using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Builds the <c>Reporting-Endpoints</c> and <c>Report-To</c> header values for the
/// configured security-reporting endpoints.
/// </summary>
public static class SecurityReportingHeaderBuilder
{
    /// <summary>
    /// Builds the <c>Reporting-Endpoints</c> header value.
    /// </summary>
    public static string BuildReportingEndpoints(HttpContext ctx, SecurityReportingOptions options)
    {
        var integrityUrl = BuildEndpointUrl(ctx, options.RoutePrefix, options.IntegrityRoute);
        var cspUrl = BuildEndpointUrl(ctx, options.RoutePrefix, options.CspRoute);
        return $"{options.IntegrityGroupName}=\"{integrityUrl}\", {options.CspGroupName}=\"{cspUrl}\"";
    }

    /// <summary>
    /// Builds the <c>Report-To</c> header value (legacy multi-group JSON form).
    /// </summary>
    public static string BuildReportTo(HttpContext ctx, SecurityReportingOptions options)
    {
        var integrityUrl = BuildEndpointUrl(ctx, options.RoutePrefix, options.IntegrityRoute);
        var cspUrl = BuildEndpointUrl(ctx, options.RoutePrefix, options.CspRoute);
        return $"{{\"group\":\"{options.IntegrityGroupName}\",\"max_age\":{options.MaxAgeSeconds},\"endpoints\":[{{\"url\":\"{integrityUrl}\"}}]}}, "
             + $"{{\"group\":\"{options.CspGroupName}\",\"max_age\":{options.MaxAgeSeconds},\"endpoints\":[{{\"url\":\"{cspUrl}\"}}]}}";
    }

    private static string BuildEndpointUrl(HttpContext ctx, string routePrefix, string route) =>
        $"{ctx.Request.Scheme}://{ctx.Request.Host}/{routePrefix.TrimStart('/').TrimEnd('/')}/{route.TrimStart('/')}";
}
