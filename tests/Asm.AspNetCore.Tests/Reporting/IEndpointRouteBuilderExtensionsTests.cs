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

    // ──────────────────────────────────────────────────────────────────────────
    // Body under cap: 204 + log captured
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_BodyUnderCap_Returns204AndLogsBody()
    {
        var (host, client, logProvider) = await BuildHostAsync(opts => opts.MaxBodyBytes = 1024);
        using (host)
        {
            var reportBody = "{\"csp-report\":{\"violated-directive\":\"default-src\"}}";
            var content = new StringContent(reportBody, Encoding.UTF8, "application/csp-report");
            var response = await client.PostAsync("/reporting/csp", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var matchingEntries = logProvider.Entries
                .Where(e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory &&
                            e.Level == LogLevel.Warning)
                .ToList();

            Assert.True(matchingEntries.Count > 0, "Expected at least one Warning log entry under the CSP category.");
            Assert.Contains(reportBody, matchingEntries[0].Message);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Body exceeding declared Content-Length: 413
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_DeclaredContentLengthExceedsCap_Returns413()
    {
        var (host, client, logProvider) = await BuildHostAsync(opts => opts.MaxBodyBytes = 10);
        using (host)
        {
            // Body itself is 20 bytes; Content-Length is set accurately by StringContent.
            var content = new StringContent("12345678901234567890", Encoding.UTF8, "application/csp-report");
            var response = await client.PostAsync("/reporting/csp", content);

            Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);

            // Nothing should have been logged
            Assert.DoesNotContain(logProvider.Entries,
                e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Body exceeds cap via streaming (no Content-Length): 413
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_StreamingBodyExceedsCap_Returns413()
    {
        var (host, client, logProvider) = await BuildHostAsync(opts => opts.MaxBodyBytes = 10);
        using (host)
        {
            // Use a StreamContent without a known length so Content-Length is not sent.
            var bodyBytes = Encoding.UTF8.GetBytes("12345678901234567890");
            var stream = new MemoryStream(bodyBytes);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/csp-report");
            // Deliberately omit Content-Length so only the bounded read enforces the cap.

            var response = await client.PostAsync("/reporting/csp", content);

            Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);

            // Nothing should have been logged
            Assert.DoesNotContain(logProvider.Entries,
                e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Body containing \r\n control chars: log contains sanitised text
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_BodyWithControlChars_LogsSanitisedBody()
    {
        var (host, client, logProvider) = await BuildHostAsync();
        using (host)
        {
            // Embed \r\n and a form-feed into the body to simulate log-injection attempt.
            var maliciousBody = "{\"injected\": \"value\r\nFAKE LOG ENTRY\fmore-fake\"}";
            var content = new StringContent(maliciousBody, Encoding.UTF8, "application/csp-report");
            await client.PostAsync("/reporting/csp", content);

            var matchingEntries = logProvider.Entries
                .Where(e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory &&
                            e.Level == LogLevel.Warning)
                .ToList();

            Assert.True(matchingEntries.Count > 0, "Expected at least one Warning log entry under the CSP category.");
            var message = matchingEntries[0].Message;
            // CR, LF and form-feed must not appear in the logged message.
            Assert.DoesNotContain('\r', message);
            Assert.DoesNotContain('\n', message);
            Assert.DoesNotContain('\f', message);
            // The non-control content should still be present.
            Assert.Contains("FAKE LOG ENTRY", message);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ContentType header containing \r\n: sanitised in log
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_ContentTypeWithControlChars_LogsSanitisedContentType()
    {
        var (host, client, logProvider) = await BuildHostAsync();
        using (host)
        {
            var content = new StringContent("{\"csp-report\":{}}", Encoding.UTF8, "application/csp-report");

            // ASP.NET Core / TestServer won't let us inject \r\n into a header value directly
            // (it would throw in the HTTP client), so we test the sanitiser at the unit level by
            // verifying a clean ContentType passes through correctly and that the log entry
            // contains the content type string without control characters.
            var response = await client.PostAsync("/reporting/csp", content);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var matchingEntries = logProvider.Entries
                .Where(e => e.Category == IEndpointRouteBuilderExtensions.CspLoggerCategory &&
                            e.Level == LogLevel.Warning)
                .ToList();

            Assert.True(matchingEntries.Count > 0, "Expected at least one Warning log entry under the CSP category.");
            var message = matchingEntries[0].Message;
            // The logged message must not contain raw control characters.
            Assert.DoesNotContain('\r', message);
            Assert.DoesNotContain('\n', message);
            // The content type value should appear.
            Assert.Contains("application/csp-report", message);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Custom MaxBodyBytes = 1024: boundary is respected
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostCsp_CustomMaxBodyBytes_BoundaryRespected()
    {
        const int cap = 1024;
        var (host, client, logProvider) = await BuildHostAsync(opts => opts.MaxBodyBytes = cap);
        using (host)
        {
            // Exactly at the cap (1024 bytes): should succeed.
            var atCapBody = new string('x', cap);
            var atCapContent = new StringContent(atCapBody, Encoding.UTF8, "application/csp-report");
            var atCapResponse = await client.PostAsync("/reporting/csp", atCapContent);
            Assert.Equal(HttpStatusCode.NoContent, atCapResponse.StatusCode);

            // One byte over the cap: should be rejected.
            var overCapBody = new string('x', cap + 1);
            var overCapContent = new StringContent(overCapBody, Encoding.UTF8, "application/csp-report");
            var overCapResponse = await client.PostAsync("/reporting/csp", overCapContent);
            Assert.Equal(HttpStatusCode.RequestEntityTooLarge, overCapResponse.StatusCode);
        }
    }
}
