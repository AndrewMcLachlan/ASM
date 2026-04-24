using Asm.AspNetCore.Reporting;
using Asm.AspNetCore.Security;
using Asm.Security;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class IServiceCollectionExtensions
{
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
