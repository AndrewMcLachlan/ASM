using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Asm.AspNetCore.Reporting;

namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Endpoint-routing extensions for mapping the security-reporting endpoints.
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Logger category used for integrity-report messages.
    /// </summary>
    public const string IntegrityLoggerCategory = "Asm.AspNetCore.Reporting.Integrity";

    /// <summary>
    /// Logger category used for CSP report messages.
    /// </summary>
    public const string CspLoggerCategory = "Asm.AspNetCore.Reporting.Csp";

    /// <summary>
    /// Maps the security-reporting endpoints.
    /// Requires <see cref="IServiceCollectionExtensions.AddSecurityReporting"/>
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

        group.MapPost(options.IntegrityRoute.TrimStart('/'), async (HttpContext ctx) =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            var body = await reader.ReadToEndAsync();
            integrityLogger.LogWarning("Integrity Report ({ContentType}): {Report}", ctx.Request.ContentType, body);
            return Results.NoContent();
        });

        group.MapPost(options.CspRoute.TrimStart('/'), async (HttpContext ctx) =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            var body = await reader.ReadToEndAsync();
            cspLogger.LogWarning("CSP Report ({ContentType}): {Report}", ctx.Request.ContentType, body);
            return Results.NoContent();
        });

        return group;
    }
}
