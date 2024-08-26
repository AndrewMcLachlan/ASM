using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Asm.Serilog;

/// <summary>
/// Helper class to configure Serilog logging.
/// </summary>
public static class LoggingConfigurator
{
    /// <summary>
    /// Configures Serilog logging with the specified configuration.
    /// </summary>
    /// <param name="configuration">The Serilog configuration.</param>
    /// <param name="appName">The application name.</param>
    /// <returns>The configured Serilog logger configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <c>null</c>.</exception>
    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration configuration, string appName)
    {
        string? Env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "UNKNOWN";

        ArgumentNullException.ThrowIfNull(configuration);

        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", appName)
            .Enrich.WithProperty("Env", Env)
            .MinimumLevel.Information()
            .MinimumLevel.Is(LogEventLevel.Information)
            .WriteTo.Trace()
            .WriteTo.Console()
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

    /// <summary>
    /// Configures Serilog logging with the specified configuration.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog configuration.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="loggerConfiguration"/>, <paramref name="configuration"/>, or <paramref name="hostEnvironment"/> is <c>null</c>.</exception>
    /// <returns>The configured Serilog logger configuration.</returns>
    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration loggerConfiguration, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", hostEnvironment.ApplicationName)
            .Enrich.WithProperty("Env", hostEnvironment.EnvironmentName)
            .WriteTo.Trace()
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);


        IConfigurationSection? logLevelConfig = configuration.GetSection("Logging").GetSection("LogLevel");

        if (logLevelConfig != null)
        {
            foreach (IConfigurationSection child in logLevelConfig.GetChildren())
            {
                if (child.Key == "Default")
                {
                    loggerConfiguration.MinimumLevel.Is(logLevelConfig.GetValue<LogEventLevel>(child.Key));
                    continue;
                }

                loggerConfiguration.MinimumLevel.Override(child.Key, logLevelConfig.GetValue<LogEventLevel>(child.Key));
            }
        }

        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("Seq:Host")))
        {
            loggerConfiguration.WriteTo.Seq(Environment.GetEnvironmentVariable("Seq:Host")!, apiKey: Environment.GetEnvironmentVariable("Seq:APIKey"));
        }

        if (hostEnvironment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.File(@"logs\Log.log", rollingInterval: RollingInterval.Day);
        }

        return loggerConfiguration;
    }
}
