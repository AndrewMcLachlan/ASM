using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for the <see cref="IApplicationBuilder"/> interface.
/// </summary>
public static class AsmAspNetCoreApplicationBuilderExtensions
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

                var problemDetails = factory.CreateProblemDetails(context);

                // Serialize as object so derived types (e.g. HttpValidationProblemDetails and its
                // errors dictionary) are written in full, not sliced down to the base ProblemDetails.
                await context.Response.WriteAsJsonAsync<object>(problemDetails, options: null, contentType: System.Net.Mime.MediaTypeNames.Application.ProblemJson);
            });
        });

    /// <summary>
    /// Adds <see cref="CanonicalUrlMiddleware"/> to the pipeline, resolving
    /// <see cref="CanonicalUrlOptions"/> from dependency injection.
    /// </summary>
    /// <remarks>
    /// Configure the options at service-registration time with
    /// <see cref="AsmAspNetCoreServiceCollectionExtensions.AddCanonicalUrls"/>. When no options are
    /// registered the middleware runs with the <see cref="CanonicalUrlOptions"/> defaults supplied by
    /// the options infrastructure.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseCanonicalUrls(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<CanonicalUrlMiddleware>();
    }

    /// <summary>
    /// Adds <see cref="CanonicalUrlMiddleware"/> to the pipeline, configuring options inline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Callback to configure canonicalisation options.</param>
    /// <returns>The application builder.</returns>
    [Obsolete("Configure options via AddCanonicalUrls(...) at service-registration time and call the parameterless UseCanonicalUrls(). This overload will be removed in a future major version.")]
    public static IApplicationBuilder UseCanonicalUrls(this IApplicationBuilder app, Action<CanonicalUrlOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (configure is null)
        {
            return app.UseMiddleware<CanonicalUrlMiddleware>();
        }

        var options = new CanonicalUrlOptions();
        configure.Invoke(options);

        return app.UseMiddleware<CanonicalUrlMiddleware>(Options.Create(options));
    }

    /// <summary>
    /// Adds the NetEscapades security-headers middleware to the pipeline using the
    /// <see cref="HeaderPolicyCollection"/> registered via
    /// <see cref="AsmAspNetCoreServiceCollectionExtensions.AddStandardSecurityHeaders"/>, optionally
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
            return app.UseSecurityHeaders(policies);
        }

        return app.UseWhen(
            ctx => !IsExempt(ctx.Request.Path.Value, exemptPathPrefixes),
            branch => branch.UseSecurityHeaders(policies));
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
