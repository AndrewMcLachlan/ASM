using Asm.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.AspNetCore;

public class WebApplicationStart
{
    public static int Run(string[] args, string appName, Action<WebApplicationBuilder> addServices, Action<WebApplication> addApp)
    {
        Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

        try
        {
            Log.Information("Starting...");

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ApplicationName = appName, Args = args, });

            builder.Host.UseCustomSerilog();

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
