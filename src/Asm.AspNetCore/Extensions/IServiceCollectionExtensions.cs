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
    /// <para>Defaults applied:</para>
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
    /// <para>HSTS is intentionally not included — use ASP.NET Core's <c>AddHsts()</c> and <c>UseHsts()</c> instead.</para>
    /// <para><b>Auto-coupling:</b> if <see cref="AddSecurityReporting"/> was called before this
    /// method, <c>Reporting-Endpoints</c> and <c>Report-To</c> header policies are automatically
    /// included in the collection. <see cref="AddSecurityReporting"/> <b>must</b> be called first
    /// for auto-emit to work; calling it after this method has no effect on the header policies.</para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <returns>The registered <see cref="HeaderPolicyCollection"/>, returned so the caller can chain additional policy configuration. Mutations to the returned instance are visible at request time because it is the same singleton registered in <paramref name="services"/>.</returns>
    public static HeaderPolicyCollection AddStandardSecurityHeaders(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var policies = new HeaderPolicyCollection();

        policies.AddCrossOriginOpenerPolicy(b => b.SameOriginAllowPopups());
        policies.AddCrossOriginEmbedderPolicy(b => b.RequireCorp());
        policies.AddCrossOriginResourcePolicy(b => b.SameOrigin());
        policies.AddFrameOptionsSameOrigin();
        policies.AddContentTypeOptionsNoSniff();
        policies.AddReferrerPolicyStrictOriginWhenCrossOrigin();
        policies.AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none");
        policies.RemoveServerHeader();

        // Auto-coupling: if AddSecurityReporting was called first, the SecurityReportingOptions
        // singleton is already registered. Lift it and apply the reporting header policies.
        var reportingDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(SecurityReportingOptions));
        if (reportingDescriptor?.ImplementationInstance is SecurityReportingOptions reportingOptions)
        {
            policies.AddSecurityReportingHeaders(reportingOptions);
        }

        services.AddSingleton(policies);
        return policies;
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
    /// When registered before <see cref="AddStandardSecurityHeaders"/>, the resulting
    /// <see cref="HeaderPolicyCollection"/> automatically includes <c>Reporting-Endpoints</c>
    /// and <c>Report-To</c> header policies via
    /// <see cref="HeaderPolicyCollectionReportingExtensions.AddSecurityReportingHeaders"/>.
    /// </summary>
    /// <remarks>
    /// Call this method <b>before</b> <see cref="AddStandardSecurityHeaders"/> for the
    /// auto-coupling to take effect. Registering after has no effect on the header policies.
    /// </remarks>
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
        // AddStandardSecurityHeaders can detect its presence via FirstOrDefault —
        // IOptions<T> is always resolvable and wouldn't signal "reporting not configured".
        services.AddSingleton(options);
        return services;
    }
}
