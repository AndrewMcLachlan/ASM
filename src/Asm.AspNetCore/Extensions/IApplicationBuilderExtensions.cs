using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
    /// Adds security headers to the middleware pipeline.
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
}
