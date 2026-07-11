using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Asm.AspNetCore.Extensions;
using Asm.AspNetCore.HealthChecks;
using Asm.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Asm.AspNetCore;

/// <summary>
/// Bootstraps and runs a web application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class WebApplicationStart
{
    /// <summary>
    /// Builds and runs the web application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The friendly name of the application, used for logging and telemetry.</param>
    /// <param name="addServices">A method to add service mappings.</param>
    /// <param name="addApp">Set up custom middleware.</param>
    /// <param name="addHealthChecks">A method to add health checks.</param>
    /// <returns>An integer return code.</returns>
    public static int Run(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp, Action<IHealthChecksBuilder, WebApplicationBuilder> addHealthChecks) =>
        RunCore(args, appName, addServices, addApp, addHealthChecks, Assembly.GetCallingAssembly(), application =>
        {
            application.Run();
            return Task.CompletedTask;
        }).AsTask().GetAwaiter().GetResult();

    /// <summary>
    /// Builds and runs the web application asynchronously.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The friendly name of the application, used for logging and telemetry.</param>
    /// <param name="addServices">A method to add service mappings.</param>
    /// <param name="addApp">Set up custom middleware.</param>
    /// <param name="addHealthChecks">A method to add health checks.</param>
    /// <returns>An integer return code.</returns>
    public static async ValueTask<int> RunAsync(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp, Action<IHealthChecksBuilder, WebApplicationBuilder> addHealthChecks) =>
        await RunCore(args, appName, addServices, addApp, addHealthChecks, Assembly.GetCallingAssembly(), application => application.RunAsync());

    private static async ValueTask<int> RunCore(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp, Action<IHealthChecksBuilder, WebApplicationBuilder> addHealthChecks, Assembly callingAssembly, Func<WebApplication, Task> run)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information($"{appName} Starting...");

            var application = BuildApplication(args, appName, addServices, addApp, addHealthChecks, callingAssembly);

            await run(application);

            Log.Information($"{appName} Stopping...");

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal host exception");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static WebApplication BuildApplication(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp, Action<IHealthChecksBuilder, WebApplicationBuilder> addHealthChecks, Assembly callingAssembly)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets(callingAssembly, true);
            builder.Logging.AddDebug();
            builder.Logging.AddConsole();
        }

        builder.Host.UseCustomSerilog(appName);
        builder.AddStandardOpenTelemetry(appName);

        addServices(builder);

        addHealthChecks(builder.Services.AddHealthChecks(), builder);

        var application = builder.Build();

        addApp(application);

        application.MapHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = p => p.Tags.IsNullOrEmpty() || p.Tags.Contains("health"),
            ResponseWriter = ResponseWriter.WriteResponse,
        });

        return application;
    }
}
