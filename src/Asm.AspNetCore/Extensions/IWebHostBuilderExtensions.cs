using Asm.Serilog;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Asm.AspNetCore.Extensions;

public static class IWebHostBuilderExtensions
{
    public static IWebHostBuilder UseCustomSerilog(this IWebHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) => LoggingConfigurator.ConfigureLogging(configuration, context.Configuration, context.HostingEnvironment));
    }
}
