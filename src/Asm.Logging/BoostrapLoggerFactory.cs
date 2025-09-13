using Microsoft.Extensions.Logging;

namespace Asm.Logging;

/// <summary>
/// Creates a bootstrap logger for early application startup logging.
/// </summary>
public static class BootstrapLoggerFactory
{
    /// <summary>
    /// Creates a bootstrap logger with console and optional Seq logging.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <returns>A configured ILogger instance.</returns>
    public static ILogger Create(string appName)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "UNKNOWN";

            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);

            if (env == "Development")
            {
                builder.AddDebug();
            }

            // Add Seq logging if configured
            var seqHost = Environment.GetEnvironmentVariable("Seq:Host");
            if (!String.IsNullOrEmpty(seqHost))
            {
                var seqApiKey = Environment.GetEnvironmentVariable("Seq:APIKey");
                builder.AddSeq(seqHost, seqApiKey);
            }
            else
            {
                Console.WriteLine("Seq:Host not defined, seq logging disabled.");
            }

            builder.AddFilter((category, level) =>
            {
                // You can add custom filtering logic here if needed
                return level >= LogLevel.Information;
            });
        });

        var bootstrapLogger = loggerFactory.CreateLogger($"Bootstrap.{appName}");

        using var appScope = bootstrapLogger.BeginScope(new Dictionary<string, object>
        {
            ["App"] = appName,
            ["Env"] = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "UNKNOWN"
        });

        return bootstrapLogger;
    }
}
