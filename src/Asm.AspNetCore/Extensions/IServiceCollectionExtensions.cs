using Asm.Security;
using Asm.AspNetCore.Security;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddProblemDetailsFactory(this IServiceCollection services) =>
        services.AddTransient<ProblemDetailsFactory, Asm.AspNetCore.ProblemDetailsFactory>();

    public static IServiceCollection AddPrincipalProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services.AddScoped<IPrincipalProvider, HttpContextPrincipalProvider>();
    }
}
