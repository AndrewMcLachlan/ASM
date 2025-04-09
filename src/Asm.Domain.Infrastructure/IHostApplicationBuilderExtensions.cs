using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace Asm.AspNetCore.Extensions;

/// <summary>
/// Extensions to the <see cref="IHostApplicationBuilder"/> interface.
/// </summary>
public static class IHostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the OpenTelemetry for Entity Framework and SQL Client instrumentation to the application builder.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IHostApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static IHostApplicationBuilder AddEntityFrameworkOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(options =>
            {
                options.AddSqlClientInstrumentation();
                options.AddEntityFrameworkCoreInstrumentation();
            });

        return builder;
    }
}
