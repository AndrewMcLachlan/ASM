using Asm.Web.Serilog;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Asm.Web.Extensions;

public static class IWebHostBuilderExtensions
{
     public static IWebHostBuilder UseCustomSerilog(this IWebHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) => LoggingConfigurator.ConfigureLogging(configuration, context));
    }
}
