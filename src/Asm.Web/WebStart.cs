﻿using Asm.Web.Extensions;
using Asm.Web.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.Web
{
    public static class WebStart<T> where T : class
    {
        public static int Run(string[] args, string appName)
        {
            Log.Logger = LoggingConfigurator.ConfigureLogging(new LoggerConfiguration(), appName).CreateBootstrapLogger();

            try
            {
                Log.Information("Starting...");
                CreateHostBuilder(args, appName).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args, string appName) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseSetting(WebHostDefaults.ApplicationKey, appName)
                    .UseCustomSerilog()
                    .UseStartup<T>());

    }
}