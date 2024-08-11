using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for the <see cref="IApplicationBuilder"/> interface.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a standard exception handler to the middleware pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that calls can be chained.</returns>
    public static IApplicationBuilder UseStandardExceptionHandler(this IApplicationBuilder builder) =>
        builder.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var factory = context.RequestServices.GetRequiredService<Mvc.Infrastructure.ProblemDetailsFactory>();
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(factory.CreateProblemDetails(context));
            });
        });
}
