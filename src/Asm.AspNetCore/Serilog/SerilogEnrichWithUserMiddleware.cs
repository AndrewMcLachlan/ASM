using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Asm.AspNetCore.Middleware
{
    /// <summary>
    /// Middleware that enriches the Serilog log context with the current user.
    /// </summary>
    /// <param name="next">The next middleware in the chain.</param>
    public class SerilogEnrichWithUserMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <remarks>
        /// I'm not sure why it's necessary to pass the IHttpContextAccessor to the middleware. It seems like it should be able to access the current context directly.
        /// </remarks>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="httpContextAccessor">An instance of <see cref="IHttpContextAccessor"/>.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHttpContextAccessor httpContextAccessor)
        {
            string name = httpContextAccessor.HttpContext.GetUserName();

            using (LogContext.PushProperty("User", name))
            {
                await _next.Invoke(context);
            }
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions for the <see cref="IApplicationBuilder"/> interface.
    /// </summary>
    public static class SerilogEnrichmentMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="Asm.AspNetCore.Middleware.SerilogEnrichWithUserMiddleware"/> to the middleware pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance that this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance for chaining.</returns>
        public static IApplicationBuilder UseSerilogEnrichWithUser(this IApplicationBuilder app) =>
            app.UseMiddleware<Asm.AspNetCore.Middleware.SerilogEnrichWithUserMiddleware>();
    }
}
