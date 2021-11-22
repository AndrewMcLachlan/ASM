using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Asm.Web.Serilog;

public static class LoggingConfigurator
{
    private static readonly string? Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration configuration, string appName)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", appName)
            .Enrich.WithProperty("Env", Env)
            .MinimumLevel.Information()
            .MinimumLevel.Is(LogEventLevel.Information)
            .WriteTo.Trace()
            .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);

        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("Seq:Host")))
        {
            configuration.WriteTo.Seq(Environment.GetEnvironmentVariable("Seq:Host")!, apiKey: Environment.GetEnvironmentVariable("Seq:APIKey"));
        }

        if (Env == "Development")
        {
            configuration.WriteTo.File(@"logs\Log.log", rollingInterval: RollingInterval.Day);
        }

        return configuration;
    }

    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration configuration, WebHostBuilderContext context)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (context == null) throw new ArgumentNullException(nameof(context));

        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", context.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Env", context.HostingEnvironment.EnvironmentName)
            .MinimumLevel.Information()
            .MinimumLevel.Is(LogEventLevel.Information)
            .WriteTo.Trace()
            .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);


        IConfigurationSection logLevelConfig = context.Configuration.GetSection("Logging").GetSection("LogLevel");
        configuration
            .MinimumLevel.Override("System", logLevelConfig.GetValue<LogEventLevel>("System"))
            .MinimumLevel.Override("Microsoft", logLevelConfig.GetValue<LogEventLevel>("Microsoft"))
            .WriteTo.Seq(context.Configuration["Seq:Host"], apiKey: context.Configuration["Seq:APIKey"]);

        if (context.HostingEnvironment.EnvironmentName == "Development")
        {
            configuration.WriteTo.File(@"logs\Log.log", rollingInterval: RollingInterval.Day);
        }

        return configuration;
    }
}
