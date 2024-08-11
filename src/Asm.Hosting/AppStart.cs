using Asm.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.Hosting;

/// <summary>
/// Start a hosted process.
/// </summary>
public static class AppStart
{
    /// <summary>
    /// Run the hosted process synchronously.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A return code.</returns>
    public static int Run(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            CreateHostBuilder(args, appName, configureServices).Build().Run();
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
    /// Run the hosted process synchronously.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A return code.</returns>
    public static async ValueTask<int> RunAsync(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            await CreateHostBuilder(args, appName, configureServices).Build().RunAsync();
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
    /// Creates a host builder to allow customization of the host.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>The created host.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, appBuilder) =>
        {
            context.HostingEnvironment.ApplicationName = appName;
        })
        .UseCustomSerilog()
        .ConfigureServices(configureServices);

}
