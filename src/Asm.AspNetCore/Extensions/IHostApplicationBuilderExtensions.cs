using Asm.AspNetCore.OpenTelemetry;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Asm.AspNetCore.Extensions;

/// <summary>
/// Extensions to the <see cref="IHostApplicationBuilder"/> interface.
/// </summary>
public static class IHostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the standard OpenTelemetry configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IHostApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static IHostApplicationBuilder AddStandardOpenTelemetry(this IHostApplicationBuilder builder)
    {
        bool hasAppInsights = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") != null;

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder.AddService(builder.Environment.ApplicationName, serviceInstanceId: builder.Environment.EnvironmentName);
            })
            .WithMetrics(options =>
            {
                options.AddAspNetCoreInstrumentation();
                options.AddHttpClientInstrumentation();
            })
            .WithTracing(options =>
            {
                options.AddAspNetCoreInstrumentation();
                options.AddHttpClientInstrumentation();
                options.AddProcessor<HttpContextTraceProcessor>();
                options.AddOtlpExporter();
            })
            .WithLogging(options => options.AddProcessor<HttpContextLogProcessor>());

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.AddOtlpExporter();
            options.IncludeScopes = true;

            if (builder.Environment.IsDevelopment())
            {
                options.AddConsoleExporter();
            }
        });

        if (hasAppInsights)
        {
            builder.Services.AddOpenTelemetry().UseAzureMonitor();
        }

        return builder;
    }
}
