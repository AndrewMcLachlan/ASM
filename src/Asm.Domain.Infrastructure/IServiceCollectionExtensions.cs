using Asm.Domain;

namespace Microsoft.Extensions.DependencyInjection;

public static class AsmDomainInfrastructureIServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services) where T : class, IUnitOfWork
    {
        return services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<T>());
    }

}
