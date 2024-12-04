using Asm.AspNetCore.Extensions;
using Asm.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.AspNetCore;

/// <summary>
/// Bootstraps and runs a web application.
/// </summary>
public class WebApplicationStart
{
    /// <summary>
    /// Builds and runs the web application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The name of the application.</param>
    /// <param name="addServices">A method to add service mappings.</param>
    /// <param name="addApp">Set up custom middleware.</param>
    /// <returns>An integer return code.</returns>
    public static int Run(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information($"{appName} Starting...");

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ApplicationName = appName, Args = args, });

            builder.Host.UseCustomSerilog();
            builder.AddStandardOpenTelemetry();

            addServices(builder);

            var application = builder.Build();

            addApp(application);

            application.Run();

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
}
