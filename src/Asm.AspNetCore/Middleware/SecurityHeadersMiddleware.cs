using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Middleware;

/// <summary>
/// Adds a configurable set of security response headers to HTML responses and, optionally,
/// strips server-fingerprint headers from all responses.
/// </summary>
/// <remarks>
/// If <see cref="SecurityReportingOptions"/> is registered in DI (via
/// <c>AddSecurityReporting()</c>), the middleware also emits <c>Reporting-Endpoints</c> and
/// <c>Report-To</c> headers referencing the configured reporting routes.
/// </remarks>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersOptions _options;
    private readonly SecurityReportingOptions? _reportingOptions;

    /// <summary>
    /// Creates a new <see cref="SecurityHeadersMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="options">The security-headers options.</param>
    /// <param name="reportingOptions">Optional reporting options. When present (i.e.
    /// <c>AddSecurityReporting()</c> was called), the middleware emits
    /// <c>Reporting-Endpoints</c> and <c>Report-To</c> headers.</param>
    public SecurityHeadersMiddleware(
        RequestDelegate next,
        IOptions<SecurityHeadersOptions> options,
        SecurityReportingOptions? reportingOptions = null)
    {
        _next = next;
        _options = options.Value;
        _reportingOptions = reportingOptions;
    }

    /// <summary>
    /// Processes the request and attaches security headers to the response.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        if (IsExempt(context.Request.Path.Value))
        {
            await _next(context);
            return;
        }

        context.Response.OnStarting(() =>
        {
            if (_options.RemoveServerHeaders)
            {
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");
                context.Response.Headers.Remove("Server");
            }

            var contentType = context.Response.ContentType;
            if (contentType == null || !contentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            foreach (var (name, value) in _options.Headers)
            {
                context.Response.Headers.Append(name, value);
            }

            if (_options.DynamicHeaders is { } dynamic)
            {
                foreach (var (name, value) in dynamic(context))
                {
                    context.Response.Headers.Append(name, value);
                }
            }

            if (_reportingOptions is { } reporting)
            {
                context.Response.Headers.Append(
                    "Reporting-Endpoints",
                    SecurityReportingHeaderBuilder.BuildReportingEndpoints(context, reporting));
                context.Response.Headers.Append(
                    "Report-To",
                    SecurityReportingHeaderBuilder.BuildReportTo(context, reporting));
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }

    private bool IsExempt(string? path)
    {
        if (string.IsNullOrEmpty(path) || _options.ExemptPathPrefixes.Count == 0)
        {
            return false;
        }

        foreach (var prefix in _options.ExemptPathPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
