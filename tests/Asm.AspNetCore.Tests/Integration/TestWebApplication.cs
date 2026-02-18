using System.Net.Http;
using Asm.AspNetCore.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Integration;

public class TestWebApplication : IDisposable
{
    private readonly IHost _host;
    private readonly HttpClient _client;

    public TestWebApplication()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.UseEnvironment("Development");
                webBuilder.ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddProblemDetailsFactory();
                    services.AddHealthChecks()
                        .AddCheck("test-check", () => HealthCheckResult.Healthy("Test check passed"));
                });
                webBuilder.Configure(app =>
                {
                    app.UseSecurityHeaders();
                    app.UseStandardExceptionHandler();
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", () => "Hello World");
                        endpoints.MapGet("/error", () =>
                        {
                            throw new ApplicationException("Test exception");
                        });
                        endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                        {
                            ResponseWriter = ResponseWriter.WriteResponse,
                        });
                    });
                });
            });

        _host = builder.Build();
        _host.Start();

        _client = _host.GetTestClient();
    }

    public HttpClient CreateClient() => _client;

    public void Dispose()
    {
        _client.Dispose();
        _host.Dispose();
        GC.SuppressFinalize(this);
    }
}
