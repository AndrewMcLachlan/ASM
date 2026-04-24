using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Middleware;
using Asm.AspNetCore.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Middleware;

public class SecurityHeadersMiddlewareTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static Task<IHost> BuildHostAsync(
        Action<SecurityHeadersOptions> configure,
        Action<IApplicationBuilder> extraMiddleware = null,
        Action<IServiceCollection> configureServices = null,
        string responseContentType = "text/html",
        string path = null)
    {
        return new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                if (configureServices != null)
                    webHost.ConfigureServices(configureServices);
                webHost.Configure(app =>
                {
                    // Allow caller to inject upstream middleware (e.g. to set spurious headers)
                    extraMiddleware?.Invoke(app);
                    app.UseSecurityHeaders(configure);
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = responseContentType;
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);
    }

    private static HttpClient GetClient(IHost host, string path = null) =>
        host.GetTestClient();

    // ──────────────────────────────────────────────────────────────────────────
    // Static headers on HTML responses
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task HtmlResponse_StaticHeaders_ArePresent()
    {
        using var host = await BuildHostAsync(opts =>
        {
            opts.Headers["X-Test-Header"] = "test-value";
            opts.Headers["X-Another"] = "another-value";
        });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Test-Header"), "X-Test-Header missing");
        Assert.Equal("test-value", response.Headers.GetValues("X-Test-Header").First());
        Assert.True(response.Headers.Contains("X-Another"), "X-Another missing");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Non-HTML responses do NOT get static headers
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task NonHtmlResponse_StaticHeaders_AreNotAdded()
    {
        using var host = await BuildHostAsync(
            opts => opts.Headers["X-Test-Header"] = "test-value",
            responseContentType: "application/json");

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(response.Headers.Contains("X-Test-Header"),
            "Static security headers should not be added to non-HTML responses");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RemoveServerHeaders strips fingerprint headers
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveServerHeaders_True_RemovesFingerprintHeaders()
    {
        // Set fake fingerprint headers directly on the response (before writing),
        // so they are present when the middleware's OnStarting callback fires and removes them.
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UseSecurityHeaders(opts => opts.RemoveServerHeaders = true);
                    app.Run(async ctx =>
                    {
                        // Set fake server-fingerprint headers directly (not in OnStarting)
                        ctx.Response.Headers["Server"] = "FakeServer/1.0";
                        ctx.Response.Headers["X-Powered-By"] = "ASP.NET";
                        ctx.Response.Headers["X-AspNet-Version"] = "4.0";
                        ctx.Response.Headers["X-AspNetMvc-Version"] = "5.0";
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Fingerprint headers should have been removed by the middleware's OnStarting callback
        Assert.False(response.Headers.Contains("Server"), "Server header should be removed");
        Assert.False(response.Headers.Contains("X-Powered-By"), "X-Powered-By should be removed");
        Assert.False(response.Headers.Contains("X-AspNet-Version"), "X-AspNet-Version should be removed");
        Assert.False(response.Headers.Contains("X-AspNetMvc-Version"), "X-AspNetMvc-Version should be removed");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ExemptPathPrefixes — entire processing is skipped
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExemptPathPrefix_SkipsHeaderProcessing()
    {
        // Inject fake Server header that would be removed if processing ran
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    // Upstream middleware sets a header that fingerprint-removal would strip
                    app.Use(async (ctx, next) =>
                    {
                        ctx.Response.OnStarting(() =>
                        {
                            ctx.Response.Headers["Server"] = "ShouldSurvive";
                            return Task.CompletedTask;
                        });
                        await next(ctx);
                    });

                    app.UseSecurityHeaders(opts =>
                    {
                        opts.RemoveServerHeaders = true;
                        opts.ExemptPathPrefixes = ["/api"];
                        opts.Headers["X-Static"] = "value";
                    });

                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = GetClient(host);
        var response = await client.GetAsync("/api/foo", TestContext.Current.CancellationToken);

        // Exempt path — no header removal, no static header added
        Assert.True(response.Headers.Contains("Server"),
            "Server header should survive on exempt path");
        Assert.False(response.Headers.Contains("X-Static"),
            "Static headers should not be added on exempt path");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DynamicHeaders callback values are added
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DynamicHeaders_AreAppendedToHtmlResponse()
    {
        using var host = await BuildHostAsync(opts =>
        {
            opts.DynamicHeaders = ctx => new Dictionary<string, string>
            {
                ["X-Request-Scheme"] = ctx.Request.Scheme
            };
        });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.Contains("X-Request-Scheme"),
            "Dynamic header X-Request-Scheme missing");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SecurityReportingOptions registered → Reporting-Endpoints + Report-To present
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task WithSecurityReporting_ReportingHeadersEmittedOnHtml()
    {
        using var host = await BuildHostAsync(
            configure: opts => { /* defaults */ },
            configureServices: svc => svc.AddSecurityReporting());

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.Contains("Reporting-Endpoints"),
            "Reporting-Endpoints header missing");
        Assert.True(response.Headers.Contains("Report-To"),
            "Report-To header missing");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SecurityReportingOptions NOT registered → headers absent
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task WithoutSecurityReporting_ReportingHeadersAbsent()
    {
        // No AddSecurityReporting() call
        using var host = await BuildHostAsync(opts => { });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.False(response.Headers.Contains("Reporting-Endpoints"),
            "Reporting-Endpoints should be absent when reporting not registered");
        Assert.False(response.Headers.Contains("Report-To"),
            "Report-To should be absent when reporting not registered");
    }
}
