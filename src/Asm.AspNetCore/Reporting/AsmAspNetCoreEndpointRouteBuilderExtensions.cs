using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Asm.AspNetCore.Reporting;

namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Endpoint-routing extensions for mapping the security-reporting endpoints.
/// </summary>
public static class AsmAspNetCoreEndpointRouteBuilderExtensions
{
    /// <summary>Logger category used for integrity-report messages.</summary>
    public const string IntegrityLoggerCategory = "Asm.AspNetCore.Reporting.Integrity";

    /// <summary>Logger category used for CSP report messages.</summary>
    public const string CspLoggerCategory = "Asm.AspNetCore.Reporting.Csp";

    /// <summary>
    /// Maps the security-reporting endpoints.
    /// Requires <see cref="AsmAspNetCoreServiceCollectionExtensions.AddSecurityReporting"/>
    /// to have been called.
    /// </summary>
    /// <param name="endpoints">The endpoint-route builder.</param>
    /// <returns>The endpoint-convention builder for further configuration.</returns>
    public static IEndpointConventionBuilder MapSecurityReporting(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var options = endpoints.ServiceProvider.GetRequiredService<SecurityReportingOptions>();
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var integrityLogger = loggerFactory.CreateLogger(IntegrityLoggerCategory);
        var cspLogger = loggerFactory.CreateLogger(CspLoggerCategory);

        var group = endpoints.MapGroup(options.RoutePrefix.TrimStart('/').TrimEnd('/'));

        // Browsers post CSP/integrity reports without credentials; a fallback authorization
        // policy would otherwise 401 every report and silently lose it.
        group.AllowAnonymous();

        group.MapPost(options.IntegrityRoute.TrimStart('/'),
            (Func<HttpContext, Task<IResult>>)(async ctx => await HandleReportAsync(ctx, integrityLogger, "Integrity Report", options.MaxBodyBytes)));

        group.MapPost(options.CspRoute.TrimStart('/'),
            (Func<HttpContext, Task<IResult>>)(async ctx => await HandleReportAsync(ctx, cspLogger, "CSP Report", options.MaxBodyBytes)));

        return group;
    }

    private static async Task<IResult> HandleReportAsync(
        HttpContext ctx,
        ILogger logger,
        string reportLabel,
        int maxBodyBytes)
    {
        if (ctx.Request.ContentLength is long declaredLength && declaredLength > maxBodyBytes)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        var (body, truncated) = await ReadBoundedAsync(ctx.Request.Body, maxBodyBytes, ctx.RequestAborted);

        if (truncated)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        var contentType = SanitiseForLog(ctx.Request.ContentType);
        var safeBody = SanitiseForLog(body);
        logger.LogWarning("{Label} ({ContentType}): {Report}", reportLabel, contentType, safeBody);
        return Results.NoContent();
    }

    private static async Task<(string Body, bool Truncated)> ReadBoundedAsync(Stream stream, int maxBytes, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        var buffer = new byte[8192];
        var total = 0;
        int read;
        while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > maxBytes)
            {
                return (string.Empty, true);
            }
            await ms.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
        return (Encoding.UTF8.GetString(ms.ToArray()), false);
    }

    private static string SanitiseForLog(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            // Keep printable and tab (tab is common in JSON whitespace); drop other control chars.
            if (c == '\t' || !char.IsControl(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
