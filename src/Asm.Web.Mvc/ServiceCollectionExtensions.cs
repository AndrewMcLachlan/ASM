
using Asm.Security;
using Asm.Web.Mvc.Security;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPrincipalProvider(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPrincipalProvider, HttpContextPrincipalProvider>();
        }
    }
}
