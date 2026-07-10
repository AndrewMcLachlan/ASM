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
            .WriteTo.Console();

        var seqHost = Environment.GetEnvironmentVariable("Seq__Host") ?? Environment.GetEnvironmentVariable("Seq:Host");

        if (!String.IsNullOrEmpty(seqHost))
        {
            configuration.WriteTo.Seq(seqHost, apiKey: Environment.GetEnvironmentVariable("Seq__APIKey") ?? Environment.GetEnvironmentVariable("Seq:APIKey"));
        }
        else
        {
            Console.WriteLine("Seq:Host not defined, seq logging disabled.");
        }

        if (Env == "Development")
        {
            // Trace goes through OutputDebugString even with no listener attached, so it is
            // Development-only rather than a per-event cost in production.
            configuration.WriteTo.Trace();
            configuration.WriteTo.File(Path.Combine("logs", "Log.log"), rollingInterval: RollingInterval.Day);
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
    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration loggerConfiguration, IConfiguration configuration, IHostEnvironment hostEnvironment) =>
        ConfigureLogging(loggerConfiguration, configuration, hostEnvironment, hostEnvironment?.ApplicationName!);

    /// <summary>
    /// Configures Serilog logging with the specified configuration.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog configuration.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="appName">The friendly application name used for log enrichment.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="loggerConfiguration"/>, <paramref name="configuration"/>, or <paramref name="hostEnvironment"/> is <c>null</c>.</exception>
    /// <returns>The configured Serilog logger configuration.</returns>
    public static LoggerConfiguration ConfigureLogging(LoggerConfiguration loggerConfiguration, IConfiguration configuration, IHostEnvironment hostEnvironment, string appName)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", appName)
            .Enrich.WithProperty("Env", hostEnvironment.EnvironmentName)
            .WriteTo.Console();

        IConfigurationSection logLevelConfig = configuration.GetSection("Logging").GetSection("LogLevel");

        foreach (IConfigurationSection child in logLevelConfig.GetChildren())
        {
            LogEventLevel? level = MapLogLevel(child.Value);

            if (level == null) continue;

            if (child.Key == "Default")
            {
                loggerConfiguration.MinimumLevel.Is(level.Value);
                continue;
            }

            loggerConfiguration.MinimumLevel.Override(child.Key, level.Value);
        }

        var seqHost = configuration.GetValue<string>("Seq:Host");

        if (!String.IsNullOrEmpty(seqHost))
        {
            loggerConfiguration.WriteTo.Seq(seqHost, apiKey: configuration.GetValue<string>("Seq:APIKey"));
        }
        else
        {
            Console.WriteLine("Seq:Host not defined, seq logging disabled.");
        }

        if (hostEnvironment.IsDevelopment())
        {
            // Trace goes through OutputDebugString even with no listener attached, so it is
            // Development-only rather than a per-event cost in production.
            loggerConfiguration.WriteTo.Trace();
            loggerConfiguration.WriteTo.File(Path.Combine("logs", "Log.log"), rollingInterval: RollingInterval.Day);
        }

        return loggerConfiguration;
    }

    /// <summary>
    /// Maps a level name from the Microsoft <c>Logging:LogLevel</c> convention to a Serilog level.
    /// Serilog's own level names are also accepted. <c>None</c> maps to a level above
    /// <see cref="LogEventLevel.Fatal"/>, silencing the source; unrecognised values are ignored.
    /// </summary>
    private static LogEventLevel? MapLogLevel(string? value) => value switch
    {
        null or "" => null,
        "Trace" => LogEventLevel.Verbose,
        "Critical" => LogEventLevel.Fatal,
        "None" => (LogEventLevel)(1 + (int)LogEventLevel.Fatal),
        _ => Enum.TryParse(value, ignoreCase: true, out LogEventLevel level) ? level : null,
    };
}
