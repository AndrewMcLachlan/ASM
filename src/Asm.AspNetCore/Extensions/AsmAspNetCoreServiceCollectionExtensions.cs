using System.Diagnostics;
using Asm.AspNetCore;
using Asm.AspNetCore.Middleware;
using Asm.AspNetCore.Reporting;
using Asm.AspNetCore.Security;
using Asm.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class AsmAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="HeaderPolicyCollection"/> singleton populated with the
    /// standard Asm security-header defaults, ready to be applied by
    /// <see cref="AsmAspNetCoreApplicationBuilderExtensions.UseStandardSecurityHeaders"/>.
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
    /// <para><b>Reporting coupling (order-independent):</b> if <see cref="AddSecurityReporting"/> is
    /// called — <em>before or after</em> this method — the <c>Reporting-Endpoints</c> and
    /// <c>Report-To</c> header policies are automatically composed into the collection via an
    /// <see cref="Options.IPostConfigureOptions{TOptions}"/>. Registration order no longer matters.</para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">
    /// Optional callback to add or override policies on the standard <see cref="HeaderPolicyCollection"/>
    /// (for example, to add a Content-Security-Policy). Runs after the standard defaults and before the
    /// reporting policies are composed.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddStandardSecurityHeaders(this IServiceCollection services, Action<HeaderPolicyCollection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<HeaderPolicyCollection>()
            .Configure(static policies =>
            {
                policies.AddCrossOriginOpenerPolicy(b => b.SameOriginAllowPopups());
                policies.AddCrossOriginEmbedderPolicy(b => b.RequireCorp());
                policies.AddCrossOriginResourcePolicy(b => b.SameOrigin());
                policies.AddFrameOptionsSameOrigin();
                policies.AddContentTypeOptionsNoSniff();
                policies.AddReferrerPolicyStrictOriginWhenCrossOrigin();
                policies.AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none");
                policies.RemoveServerHeader();
            });

        if (configure is not null)
        {
            services.AddOptions<HeaderPolicyCollection>().Configure(configure);
        }

        // Bridge the configured options instance to a directly-resolvable singleton so that
        // UseStandardSecurityHeaders (and any consumer resolving HeaderPolicyCollection) sees the
        // fully-composed collection, including any reporting policies contributed via
        // IPostConfigureOptions regardless of registration order.
        services.TryAddSingleton(static sp => sp.GetRequiredService<Options.IOptions<HeaderPolicyCollection>>().Value);

        return services;
    }

    /// <summary>
    /// Registers the Asm problem-details pipeline: the <see cref="AsmExceptionHandler"/> plus
    /// <c>AddProblemDetails()</c> with a customisation that adds a <c>traceId</c> extension and,
    /// outside production, the full error detail for otherwise-undescribed failures.
    /// </summary>
    /// <remarks>
    /// Wire the handler into the pipeline with <c>app.UseExceptionHandler()</c> (or the equivalent
    /// <see cref="AsmAspNetCoreApplicationBuilderExtensions.UseStandardExceptionHandler"/>). To map
    /// further exception types, register additional
    /// <see cref="Microsoft.AspNetCore.Diagnostics.IExceptionHandler"/> implementations — they compose
    /// per container, so there is no shared static handler state.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddAsmExceptionHandler(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = static context =>
            {
                var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                if (traceId is not null)
                {
                    context.ProblemDetails.Extensions["traceId"] = traceId;
                }

                // Outside production, surface the full error for failures that carry no detail
                // (for example the framework's default 500 for an exception this library does not map).
                if (context.ProblemDetails.Detail is null && context.Exception is not null)
                {
                    var environment = context.HttpContext.RequestServices.GetService<IHostEnvironment>();
                    if (environment is null || !environment.IsProduction())
                    {
                        context.ProblemDetails.Detail = context.Exception.ToString();
                    }
                }
            });

        services.AddExceptionHandler<AsmExceptionHandler>();

        return services;
    }

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
    /// Registers <see cref="SecurityReportingOptions"/> in the service collection and couples the
    /// <c>Reporting-Endpoints</c> and <c>Report-To</c> header policies into the standard
    /// <see cref="HeaderPolicyCollection"/> via an
    /// <see cref="Options.IPostConfigureOptions{TOptions}"/>.
    /// </summary>
    /// <remarks>
    /// The coupling with <see cref="AddStandardSecurityHeaders"/> is <b>order-independent</b>: this
    /// method may be called before or after <c>AddStandardSecurityHeaders</c> and the reporting
    /// header policies will still be composed into the collection, because they are contributed by an
    /// <see cref="Options.IPostConfigureOptions{TOptions}"/> that runs after every <c>Configure</c>
    /// callback.
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

        // Registered as a singleton so that MapSecurityReporting can require its presence
        // (GetRequiredService throws when reporting was not configured).
        services.AddSingleton(options);

        // Contribute the reporting header policies via IPostConfigureOptions so the coupling with
        // AddStandardSecurityHeaders is order-independent. The presence of this registration is the
        // signal that reporting is enabled — nothing is contributed unless this method was called.
        services.AddSingleton<Options.IPostConfigureOptions<HeaderPolicyCollection>, SecurityReportingHeaderPostConfigure>();

        return services;
    }

    /// <summary>
    /// Registers and configures <see cref="CanonicalUrlOptions"/> using the options pattern, for use
    /// by <see cref="AsmAspNetCoreApplicationBuilderExtensions.UseCanonicalUrls(IApplicationBuilder)"/>.
    /// </summary>
    /// <remarks>
    /// Call this at service-registration time; <c>UseCanonicalUrls()</c> then resolves the configured
    /// <see cref="Options.IOptions{TOptions}"/> from DI at request time.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to configure canonicalisation options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddCanonicalUrls(this IServiceCollection services, Action<CanonicalUrlOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = services.AddOptions<CanonicalUrlOptions>();
        if (configure is not null)
        {
            builder.Configure(configure);
        }

        return services;
    }
}
