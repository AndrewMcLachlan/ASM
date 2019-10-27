
using Asm.Security;
using Asm.Web.Mvc.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Web.Mvc
{
    public static class MvcBuilderExtensions
    {
        public static void AddPrincipalProvider(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<IPrincipalProvider, HttpContextPrincipalProvider>();
        }
    }
}
