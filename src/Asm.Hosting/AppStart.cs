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
    public static int Run(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices) =>
        RunCore(args, appName, configureServices, host =>
        {
            host.Run();
            return Task.CompletedTask;
        }).AsTask().GetAwaiter().GetResult();

    /// <summary>
    /// Run the hosted process asynchronously.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <param name="appName">The application name.</param>
    /// <param name="configureServices">A method to configure services.</param>
    /// <returns>A return code.</returns>
    public static async ValueTask<int> RunAsync(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices) =>
        await RunCore(args, appName, configureServices, host => host.RunAsync());

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

    private static async ValueTask<int> RunCore(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices, Func<IHost, Task> run)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            await run(CreateHostBuilder(args, appName, configureServices).Build());
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
