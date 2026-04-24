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
    /// Adds a hardcoded default security-header set to the pipeline.
    /// </summary>
    /// <remarks>
    /// Adds:
    /// - Referrer-Policy: no-referrer
    /// - Cross-Origin-Opener-Policy: same-origin-allow-popups
    /// - Cross-Origin-Embedder-Policy: require-corp
    /// - Cross-Origin-Resource-Policy: same-origin
    /// - X-Content-Type-Options: nosniff
    /// - X-Frame-Options: SAMEORIGIN
    /// - X-Permitted-Cross-Domain-Policies: none
    /// </remarks>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that calls can be chained.</returns>
    [Obsolete("Use UseSecurityHeaders(Action<SecurityHeadersOptions>) instead, which provides "
            + "configurable defaults and integrates with AddSecurityReporting(). "
            + "Pass _ => {} to get the new defaults without overrides.")]
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder) =>
        builder.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
            context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
            context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

            await next();
        });

    /// <summary>
    /// Adds <see cref="SecurityHeadersMiddleware"/> to the pipeline with the supplied options.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Callback to configure the security headers.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Action<SecurityHeadersOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SecurityHeadersOptions();
        configure(options);

        return app.UseMiddleware<SecurityHeadersMiddleware>(Options.Create(options));
    }

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
}
