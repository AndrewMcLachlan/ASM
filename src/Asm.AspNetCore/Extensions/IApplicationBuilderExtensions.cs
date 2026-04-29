using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for the <see cref="IApplicationBuilder"/> interface.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a standard exception handler to the middleware pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that calls can be chained.</returns>
    public static IApplicationBuilder UseStandardExceptionHandler(this IApplicationBuilder builder) =>
        builder.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var factory = context.RequestServices.GetRequiredService<Mvc.Infrastructure.ProblemDetailsFactory>();
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(factory.CreateProblemDetails(context));
            });
        });

    /// <summary>
    /// Adds <see cref="CanonicalUrlMiddleware"/> to the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Optional callback to configure canonicalisation options.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseCanonicalUrls(this IApplicationBuilder app, Action<CanonicalUrlOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new CanonicalUrlOptions();
        configure?.Invoke(options);

        return app.UseMiddleware<CanonicalUrlMiddleware>(Options.Create(options));
    }

    /// <summary>
    /// Adds the NetEscapades security-headers middleware to the pipeline using the
    /// <see cref="HeaderPolicyCollection"/> registered via
    /// <see cref="IServiceCollectionExtensions.AddStandardSecurityHeaders"/>, optionally
    /// skipping the supplied request path prefixes.
    /// </summary>
    /// <remarks>
    /// Path-prefix matching is case-insensitive. A request whose path starts with any of the
    /// supplied prefixes bypasses security-header processing entirely.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <param name="exemptPathPrefixes">Path prefixes to exempt from security-header processing.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseStandardSecurityHeaders(this IApplicationBuilder app, params string[] exemptPathPrefixes)
    {
        ArgumentNullException.ThrowIfNull(app);

        var policies = app.ApplicationServices.GetRequiredService<HeaderPolicyCollection>();

        if (exemptPathPrefixes.Length == 0)
        {
            return SecurityHeadersMiddlewareExtensions.UseSecurityHeaders(app, policies);
        }

        return app.UseWhen(
            ctx => !IsExempt(ctx.Request.Path.Value, exemptPathPrefixes),
            branch => SecurityHeadersMiddlewareExtensions.UseSecurityHeaders(branch, policies));
    }

    private static bool IsExempt(string? path, string[] prefixes)
    {
        if (String.IsNullOrEmpty(path)) return false;
        foreach (var prefix in prefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}
