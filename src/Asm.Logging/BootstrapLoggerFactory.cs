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
    /// <remarks>
    /// The returned logger owns its underlying logging providers. Dispose it once the
    /// application's full logging pipeline has taken over (or at process exit) to flush
    /// any buffered log events, such as those queued for Seq.
    /// </remarks>
    /// <param name="appName">The name of the application.</param>
    /// <returns>A configured logger that should be disposed to flush and release its providers.</returns>
    public static BootstrapLogger Create(string appName)
    {
        string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "UNKNOWN";

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);

            if (env == "Development")
            {
                builder.AddDebug();
            }

            // Add Seq logging if configured
            var seqHost = Environment.GetEnvironmentVariable("Seq__Host") ?? Environment.GetEnvironmentVariable("Seq:Host");
            if (!String.IsNullOrEmpty(seqHost))
            {
                var seqApiKey = Environment.GetEnvironmentVariable("Seq__APIKey") ?? Environment.GetEnvironmentVariable("Seq:APIKey");
                builder.AddSeq(seqHost, seqApiKey);
            }
            else
            {
                Console.WriteLine("Seq:Host not defined, seq logging disabled.");
            }
        });

        ILogger logger = loggerFactory.CreateLogger($"Bootstrap.{appName}");

        IDisposable? appScope = logger.BeginScope(new Dictionary<string, object>
        {
            ["App"] = appName,
            ["Env"] = env,
        });

        return new BootstrapLogger(logger, loggerFactory, appScope);
    }
}
