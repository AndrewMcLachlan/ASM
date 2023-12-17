using Asm.Serilog;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.AspNetCore;

/// <summary>
/// Start a Web Job process.
/// </summary>
public static class WebJobStart
{
    /// <summary>
    /// Run the Web Job process.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureWebJobs">Configures the web jobs.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A return code.</returns>
    public static int Run(string[] args, string appName, Action<IWebJobsBuilder> configureWebJobs, Action<HostBuilderContext, IServiceCollection> configureServices)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            var host = CreateHostBuilder(args, appName,configureWebJobs, configureServices).UseConsoleLifetime().Build();

            host.Run();
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
    /// Run the Web Job process.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureWebJobs">Configures the web jobs.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A return code.</returns>
    public static async Task<int> RunAsync(string[] args, string appName, Action<IWebJobsBuilder> configureWebJobs, Action<HostBuilderContext, IServiceCollection> configureServices)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            var host = CreateHostBuilder(args, appName, configureWebJobs, configureServices).UseConsoleLifetime().Build();
            await host.RunAsync();
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
    /// Creates teh host builder.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureWebJobs">Configures the web jobs.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A host builder instance.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args, string appName, Action<IWebJobsBuilder> configureWebJobs, Action<HostBuilderContext, IServiceCollection> configureServices) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebJobs(configureWebJobs)
        .ConfigureAppConfiguration((context, appBuilder) =>
        {
            context.HostingEnvironment.ApplicationName = appName;
        })
        .UseEnvironment(Environment.GetEnvironmentVariable(EnvironmentVariables.AspNetCoreEnvironment) ?? Environment.GetEnvironmentVariable(EnvironmentVariables.DotNetEnvironment) ?? "Development")
        .UseCustomSerilog()
        .ConfigureServices(configureServices);
}
