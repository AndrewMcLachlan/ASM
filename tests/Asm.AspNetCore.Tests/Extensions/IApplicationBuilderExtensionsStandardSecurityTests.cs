using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreApplicationBuilderExtensions.UseStandardSecurityHeaders"/>.
/// Uses <see cref="ITestServer"/> via <c>HostBuilder</c>, following the same pattern as
/// <c>CanonicalUrlMiddlewareTests</c>.
/// </summary>
[Trait("Category", "Unit")]
public class IApplicationBuilderExtensionsStandardSecurityTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a test host with <see cref="AsmAspNetCoreApplicationBuilderExtensions.UseStandardSecurityHeaders"/>
    /// registered and a terminal HTML-200 handler.
    /// </summary>
    private static async Task<(IHost host, HttpClient client)> BuildAsync(
        Action<IServiceCollection> configureServices = null,
        string[] exemptPrefixes = null,
        Action<HeaderPolicyCollection> extend = null,
        string responseContentType = "text/html")
    {
        exemptPrefixes ??= [];

        var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    configureServices?.Invoke(services);
                    services.AddStandardSecurityHeaders(extend);
                });
                webHost.Configure(app =>
                {
                    app.UseStandardSecurityHeaders(exemptPrefixes);
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = responseContentType;
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestServer().CreateClient();
        return (host, client);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Null guard
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a null IApplicationBuilder
    /// When UseStandardSecurityHeaders is called
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    public void UseStandardSecurityHeadersNullAppThrowsArgumentNullException()
    {
        IApplicationBuilder app = null!;
        Assert.Throws<ArgumentNullException>(() => app.UseStandardSecurityHeaders());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Default header set emitted on HTML responses
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a host with UseStandardSecurityHeaders registered
    /// When an HTML response is returned
    /// Then each expected default security header is present with its expected value
    /// </summary>
    [Theory]
    [InlineData("Cross-Origin-Opener-Policy", "same-origin-allow-popups")]
    [InlineData("Cross-Origin-Embedder-Policy", "require-corp")]
    [InlineData("Cross-Origin-Resource-Policy", "same-origin")]
    [InlineData("X-Frame-Options", "SAMEORIGIN")]
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("Referrer-Policy", "strict-origin-when-cross-origin")]
    [InlineData("X-Permitted-Cross-Domain-Policies", "none")]
    public async Task HtmlResponseDefaultHeadersArePresent(string headerName, string expectedValue)
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains(headerName), $"{headerName} should be present on HTML responses");
            Assert.Equal(expectedValue, response.Headers.GetValues(headerName).First());
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Server header is removed
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a host with UseStandardSecurityHeaders registered
    /// When an HTML response is returned
    /// Then the Server header is removed
    /// </summary>
    [Fact]
    public async Task HtmlResponseServerHeaderIsRemoved()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.False(response.Headers.Contains("Server"), "Server header should be removed");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Custom CSP via extend overrides the default
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given a host configured with a custom CSP via the extend callback
    /// When an HTML response is returned
    /// Then the Content-Security-Policy header is present
    /// </summary>
    [Fact]
    public async Task WithExtendCustomCspOverridesDefault()
    {
        var (host, client) = await BuildAsync(extend: policies =>
            policies.AddContentSecurityPolicy(csp => csp.AddDefaultSrc().Self().From("cdn.example.com")));
        using (host)
        {
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("Content-Security-Policy"), "CSP header should be present");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Exempt path prefix — header processing is bypassed
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given an exempt path prefix is configured
    /// When a request to an exempt path is made
    /// Then the security headers are not added
    /// </summary>
    [Fact]
    public async Task ExemptPathPrefixBypassesHeaderProcessing()
    {
        var (host, client) = await BuildAsync(exemptPrefixes: ["/api"]);
        using (host)
        {
            var response = await client.GetAsync("/api/data", TestContext.Current.CancellationToken);

            // Exempt path: security headers should NOT be added
            Assert.False(response.Headers.Contains("X-Frame-Options"),
                "X-Frame-Options should not be added on an exempt path");
            Assert.False(response.Headers.Contains("X-Content-Type-Options"),
                "X-Content-Type-Options should not be added on an exempt path");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Non-exempt path — headers ARE added
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given an exempt path prefix is configured
    /// When a request to a non-exempt path is made
    /// Then the security headers are added
    /// </summary>
    [Fact]
    public async Task NonExemptPathHeadersAreAdded()
    {
        var (host, client) = await BuildAsync(exemptPrefixes: ["/api"]);
        using (host)
        {
            var response = await client.GetAsync("/page", TestContext.Current.CancellationToken);

            Assert.True(response.Headers.Contains("X-Frame-Options"),
                "X-Frame-Options should be present on non-exempt paths");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AddSecurityReporting registered → Reporting-Endpoints + Report-To on HTML
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting is registered before AddStandardSecurityHeaders
    /// When an HTML response is returned
    /// Then the Reporting-Endpoints and Report-To headers are emitted
    /// </summary>
    [Fact]
    public async Task WithAddSecurityReportingBeforeAddStandardHeadersReportingHeadersEmitted()
    {
        var (host, client) = await BuildAsync(configureServices: services =>
            services.AddSecurityReporting());
        using (host)
        {
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("Reporting-Endpoints"),
                "Reporting-Endpoints should be emitted when AddSecurityReporting is called before AddStandardSecurityHeaders");
            Assert.True(response.Headers.Contains("Report-To"),
                "Report-To should be emitted when AddSecurityReporting is called before AddStandardSecurityHeaders");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Order-independence: AddSecurityReporting AFTER AddStandardSecurityHeaders
    // still emits the reporting headers at runtime
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting is registered after AddStandardSecurityHeaders
    /// When an HTML response is returned
    /// Then the Reporting-Endpoints and Report-To headers are still emitted
    /// </summary>
    [Fact]
    public async Task WithAddSecurityReportingAfterAddStandardHeadersReportingHeadersEmitted()
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddStandardSecurityHeaders();   // headers first
                    services.AddSecurityReporting();         // reporting AFTER — must still couple
                });
                webHost.Configure(app =>
                {
                    app.UseStandardSecurityHeaders();
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        using (host)
        {
            var client = host.GetTestServer().CreateClient();
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("Reporting-Endpoints"),
                "Reporting-Endpoints should be emitted even when AddSecurityReporting is called AFTER AddStandardSecurityHeaders");
            Assert.True(response.Headers.Contains("Report-To"),
                "Report-To should be emitted even when AddSecurityReporting is called AFTER AddStandardSecurityHeaders");
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // AddSecurityReporting NOT registered → reporting headers absent
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given AddSecurityReporting is not registered
    /// When an HTML response is returned
    /// Then the Reporting-Endpoints and Report-To headers are absent
    /// </summary>
    [Fact]
    public async Task WithoutAddSecurityReportingReportingHeadersAbsent()
    {
        var (host, client) = await BuildAsync(); // no AddSecurityReporting
        using (host)
        {
            var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            Assert.False(response.Headers.Contains("Reporting-Endpoints"),
                "Reporting-Endpoints should be absent when AddSecurityReporting was not called");
            Assert.False(response.Headers.Contains("Report-To"),
                "Report-To should be absent when AddSecurityReporting was not called");
        }
    }
}
