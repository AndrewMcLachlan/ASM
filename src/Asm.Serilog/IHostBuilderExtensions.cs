using Asm.Serilog;
using Serilog;


namespace Microsoft.Extensions.Hosting;

public static class IHostBuilderExtensions
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) => LoggingConfigurator.ConfigureLogging(configuration, context.Configuration, context.HostingEnvironment));
    }
}
