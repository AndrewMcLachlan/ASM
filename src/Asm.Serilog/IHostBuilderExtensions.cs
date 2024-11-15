using Asm.Serilog;
using Serilog;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extensions to the <see cref="IHostBuilder"/> interface.
/// </summary>
public static class IHostBuilderExtensions
{
    /// <summary>
    /// Use the ASM default Serilog configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="IHostBuilder"/> instance so that calls can be chained.</returns>
    public static IHostBuilder UseCustomSerilog(this IHostBuilder builder) =>
        builder.UseSerilog((context, configuration) => LoggingConfigurator.ConfigureLogging(configuration, context.Configuration, context.HostingEnvironment));
}
