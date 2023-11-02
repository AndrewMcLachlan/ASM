using Asm.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.AspNetCore;

public static class WebJobStart
{
    public static int Run(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices, Action<IHost> runner)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            var host = CreateHostBuilder(args, appName, configureServices).UseConsoleLifetime().Build();
            runner(host);
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

    public static async Task<int> RunAsync(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices, Func<IHost, Task> runner)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");
            var host = CreateHostBuilder(args, appName, configureServices).UseConsoleLifetime().Build();
            await runner(host);
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

    public static IHostBuilder CreateHostBuilder(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, appBuilder) =>
        {
            context.HostingEnvironment.ApplicationName = appName;
        })
        .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "LocalDev")
        .UseCustomSerilog()
        .ConfigureServices(configureServices);
}
