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
}
