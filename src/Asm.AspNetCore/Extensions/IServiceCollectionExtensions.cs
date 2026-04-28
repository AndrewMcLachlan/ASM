using Asm.AspNetCore.Reporting;
using Asm.AspNetCore.Security;
using Asm.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="HeaderPolicyCollection"/> singleton populated with the
    /// standard Asm security-header defaults, ready to be applied by
    /// <see cref="IApplicationBuilderExtensions.UseStandardSecurityHeaders"/>.
    /// </summary>
    /// <remarks>
    /// Defaults applied:
    /// <list type="bullet">
    ///   <item><description><c>Content-Security-Policy</c>: <c>default-src 'self'</c></description></item>
    ///   <item><description><c>Cross-Origin-Opener-Policy</c>: <c>same-origin-allow-popups</c></description></item>
    ///   <item><description><c>Cross-Origin-Embedder-Policy</c>: <c>require-corp</c></description></item>
    ///   <item><description><c>Cross-Origin-Resource-Policy</c>: <c>same-origin</c></description></item>
    ///   <item><description><c>X-Frame-Options</c>: <c>SAMEORIGIN</c></description></item>
    ///   <item><description><c>X-Content-Type-Options</c>: <c>nosniff</c></description></item>
    ///   <item><description><c>Referrer-Policy</c>: <c>strict-origin-when-cross-origin</c></description></item>
    ///   <item><description><c>X-Permitted-Cross-Domain-Policies</c>: <c>none</c></description></item>
    ///   <item><description>Server-fingerprint header removal</description></item>
    /// </list>
    /// HSTS is intentionally not included — use ASP.NET Core's <c>UseHsts()</c> instead.
    /// Use <paramref name="extend"/> to override any default or add additional headers.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="extend">Optional callback to override defaults or add additional policies.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddStandardSecurityHeaders(this IServiceCollection services, Action<HeaderPolicyCollection>? extend = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var policies = new HeaderPolicyCollection();

        policies.AddContentSecurityPolicy(csp => csp.AddDefaultSrc().Self());
        policies.AddCrossOriginOpenerPolicy(b => b.SameOriginAllowPopups());
        policies.AddCrossOriginEmbedderPolicy(b => b.RequireCorp());
        policies.AddCrossOriginResourcePolicy(b => b.SameOrigin());
        policies.AddFrameOptionsSameOrigin();
        policies.AddContentTypeOptionsNoSniff();
        policies.AddReferrerPolicyStrictOriginWhenCrossOrigin();
        policies.AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none");
        policies.RemoveServerHeader();

        extend?.Invoke(policies);

        services.AddSingleton(policies);
        return services;
    }

    /// <summary>
    /// Adds the custom <see cref="ProblemDetailsFactory"/> to the services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddProblemDetailsFactory(this IServiceCollection services) =>
        services.AddTransient<ProblemDetailsFactory, Asm.AspNetCore.ProblemDetailsFactory>();

    /// <summary>
    /// Adds the <see cref="HttpContextPrincipalProvider"/> to the services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddPrincipalProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services.AddScoped<IPrincipalProvider, HttpContextPrincipalProvider>();
    }

    /// <summary>
    /// Registers <see cref="SecurityReportingOptions"/> in the service collection.
    /// When registered, <see cref="Asm.AspNetCore.Middleware.SecurityHeadersMiddleware"/> automatically
    /// emits <c>Reporting-Endpoints</c> and <c>Report-To</c> headers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to customise options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSecurityReporting(
        this IServiceCollection services,
        Action<SecurityReportingOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new SecurityReportingOptions();
        configure?.Invoke(options);

        // Registered as a singleton (not via services.Configure<T>) so that
        // SecurityHeadersMiddleware can detect its absence — IOptions<T> is always
        // resolvable and wouldn't signal "reporting not configured".
        services.AddSingleton(options);
        return services;
    }
}
