using Asm.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddStandardExceptionHandler(this IApplicationBuilder builder) =>
            builder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(factory.CreateProblemDetails(context));
                });
            });
    }
}
