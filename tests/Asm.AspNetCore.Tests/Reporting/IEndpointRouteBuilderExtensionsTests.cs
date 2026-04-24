using System.Net;
using System.Net.Http;
using System.Text;
using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asm.AspNetCore.Tests.Reporting;

public class IEndpointRouteBuilderExtensionsTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Logger capture helper
    // ──────────────────────────────────────────────────────────────────────────

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public readonly List<(string Category, LogLevel Level, string Message)> Entries = [];

        public ILogger CreateLogger(string categoryName) =>
            new CapturingLogger(categoryName, Entries);

        public void Dispose() { }

        private sealed class CapturingLogger(
            string category,
            List<(string, LogLevel, string)> entries) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                entries.Add((category, logLevel, formatter(state, exception)));
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Build helper
    // ──────────────────────────────────────────────────────────────────────────

    private static async Task<(IHost host, HttpClient client, CapturingLoggerProvider logProvider)>
        BuildHostAsync(Action<SecurityReportingOptions>? configureReporting = null)
    {
        var logProvider = new CapturingLoggerProvider();

        var host = await new HostBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(logProvider);
                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddSecurityReporting(configureReporting);
                });
                webHost.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapSecurityReporting();
                        endpoints.MapGet("/", () => "ok");
                    });
                });
            })
            .StartAsync();

        return (host, host.GetTestClient(), logProvider);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // POST to integrity endpoint returns 204
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostIntegrity_Returns204()
    {
        var (host, client, _) = await BuildHostAsync();
        using (host)
        {
            var content = new StringContent("{\"blocked-uri\":\"https://evil.com\"}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/reporting/integrity", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // POST to csp endpoint returns 204
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_Returns204()
    {
        var (host, client, _) = await BuildHostAsync();
        using (host)
        {
            var content = new StringContent("{\"csp-report\":{\"violated-directive\":\"script-src\"}}", Encoding.UTF8, "application/csp-report");
            var response = await client.PostAsync("/reporting/csp", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Body is logged at Warning under documented category names
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostIntegrity_LogsAtWarning_UnderIntegrityCategory()
    {
        var (host, client, logProvider) = await BuildHostAsync();
        using (host)
        {
            var reportBody = "{\"integrity-report\":\"test\"}";
            var content = new StringContent(reportBody, Encoding.UTF8, "application/json");
            await client.PostAsync("/reporting/integrity", content);

            var matchingEntries = logProvider.Entries
                .Where(e => e.Category == IEndpointRouteBuilderExtensions.IntegrityLoggerCategory &&
                            e.Level == LogLevel.Warning)
                .ToList();

            Assert.True(matchingEntries.Count > 0,
                $"Expected a Warning log entry under category '{IEndpointRouteBuilderExtensions.IntegrityLoggerCategory}'");
            Assert.Contains(reportBody, matchingEntries[0].Message);
        }
    }

    [Fact]
    public async Task PostCsp_LogsAtWarning_UnderCspCategory()
    {
        var (host, client, logProvider) = await BuildHostAsync();
        using (host)
        {
            var reportBody = "{\"csp-report\":{\"violated-directive\":\"script-src\"}}";
            var content = new StringContent(reportBody, Encoding.UTF8, "application/csp-report");
            await client.PostAsync("/reporting/csp", content);

            var matchingEntries = logProvider.Entries
                .Where(e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory &&
                            e.Level == LogLevel.Warning)
                .ToList();

            Assert.True(matchingEntries.Count > 0,
                $"Expected a Warning log entry under category '{IEndpointRouteBuilderExtensions.CspLoggerCategory}'");
            Assert.Contains(reportBody, matchingEntries[0].Message);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Custom RoutePrefix shifts endpoints accordingly
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CustomRoutePrefix_ShiftsEndpoints()
    {
        var (host, client, _) = await BuildHostAsync(opts =>
            opts.RoutePrefix = "api/reporting");
        using (host)
        {
            // Default endpoint should not be present
            var defaultResponse = await client.PostAsync("/reporting/integrity",
                new StringContent("test", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, defaultResponse.StatusCode);

            // Custom prefix endpoint should respond
            var customResponse = await client.PostAsync("/api/reporting/integrity",
                new StringContent("test", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NoContent, customResponse.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // MapSecurityReporting without AddSecurityReporting → InvalidOperationException
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task MapSecurityReporting_WithoutAddSecurityReporting_ThrowsInvalidOperationException()
    {
        // Build host WITHOUT calling AddSecurityReporting
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddRouting();
                        // deliberately omit AddSecurityReporting
                    });
                    webHost.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            // This should throw because SecurityReportingOptions is not registered
                            endpoints.MapSecurityReporting();
                        });
                    });
                })
                .StartAsync();

            // We may need to make a request to trigger the endpoint resolution
            var client = host.GetTestClient();
            await client.PostAsync("/reporting/integrity",
                new StringContent("test", Encoding.UTF8, "application/json"));

            host.Dispose();
        });

        Assert.NotNull(ex);
    }
}
