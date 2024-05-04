using Asm.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.AspNetCore;

/// <summary>
/// Bootstraps and runs a web application.
/// </summary>
/// <typeparam name="T">The type of the startup class.</typeparam>
public static class WebStart<T> where T : class
{
    /// <summary>
    /// Builds and runs the web application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The name of the application.</param>
    /// <returns>A return code.</returns>
    public static int Run(string[] args, string appName)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            CreateHostBuilder(args, appName).Build().Run();
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

    /// <summary>
    /// Creates a host builder.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The name of the application.</param>
    /// <returns>The <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args, string appName) =>
        Host.CreateDefaultBuilder(args)
            .UseCustomSerilog()
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseSetting(WebHostDefaults.ApplicationKey, appName)
                .UseStartup<T>());

}
