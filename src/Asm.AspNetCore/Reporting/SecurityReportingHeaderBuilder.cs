using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Reporting;

/// <summary>
/// Builds the <c>Reporting-Endpoints</c> and <c>Report-To</c> header values for the
/// configured security-reporting endpoints.
/// </summary>
public static class SecurityReportingHeaderBuilder
{
    // Only the scheme and host vary per request; the endpoint paths depend solely on the
    // (startup-configured) options. Memoise the trimmed paths per options instance so the
    // trimming doesn't run on every HTML response. ConditionalWeakTable keeps this bounded —
    // weak keys, and in practice a single options instance for the app's lifetime.
    private static readonly ConditionalWeakTable<SecurityReportingOptions, EndpointPaths> PathCache = new();

    private sealed record EndpointPaths(string IntegrityPath, string CspPath);

    /// <summary>
    /// Builds the <c>Reporting-Endpoints</c> header value.
    /// </summary>
    public static string BuildReportingEndpoints(HttpContext ctx, SecurityReportingOptions options)
    {
        var paths = GetPaths(options);
        var origin = GetOrigin(ctx);
        return $"{options.IntegrityGroupName}=\"{origin}{paths.IntegrityPath}\", {options.CspGroupName}=\"{origin}{paths.CspPath}\"";
    }

    /// <summary>
    /// Builds the <c>Report-To</c> header value (legacy multi-group JSON form).
    /// </summary>
    public static string BuildReportTo(HttpContext ctx, SecurityReportingOptions options)
    {
        var paths = GetPaths(options);
        var origin = GetOrigin(ctx);
        return $"{{\"group\":\"{options.IntegrityGroupName}\",\"max_age\":{options.MaxAgeSeconds},\"endpoints\":[{{\"url\":\"{origin}{paths.IntegrityPath}\"}}]}}, "
             + $"{{\"group\":\"{options.CspGroupName}\",\"max_age\":{options.MaxAgeSeconds},\"endpoints\":[{{\"url\":\"{origin}{paths.CspPath}\"}}]}}";
    }

    private static EndpointPaths GetPaths(SecurityReportingOptions options) =>
        PathCache.GetValue(options, static o =>
        {
            var prefix = o.RoutePrefix.TrimStart('/').TrimEnd('/');
            return new EndpointPaths($"/{prefix}/{o.IntegrityRoute.TrimStart('/')}", $"/{prefix}/{o.CspRoute.TrimStart('/')}");
        });

    private static string GetOrigin(HttpContext ctx) => $"{ctx.Request.Scheme}://{ctx.Request.Host}";
}
