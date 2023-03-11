using Asm.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.Web
{
    public static class WebJobStart<T> where T : class
    {
        public static int Run(string[] args, string appName, Action<HostBuilderContext, IServiceCollection> configureServices)
        {
            Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

            try
            {
                Log.Information("Starting...");
                CreateHostBuilder(args, appName, configureServices).UseConsoleLifetime().Build().Run();
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
            .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .UseCustomSerilog()
            .ConfigureServices(configureServices);
    }
}
