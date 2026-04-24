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
    // Default headers emitted on HTML responses
    // ──────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Content-Security-Policy", "default-src 'self'")]
    [InlineData("Cross-Origin-Opener-Policy", "same-origin-allow-popups")]
    [InlineData("Cross-Origin-Embedder-Policy", "require-corp")]
    [InlineData("Cross-Origin-Resource-Policy", "same-origin")]
    [InlineData("X-Frame-Options", "SAMEORIGIN")]
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("Referrer-Policy", "strict-origin-when-cross-origin")]
    [InlineData("X-Permitted-Cross-Domain-Policies", "none")]
    public async Task HtmlResponse_DefaultHeaders_ArePresent(string headerName, string expectedValue)
    {
        using var host = await BuildHostAsync(opts => { /* use all defaults */ });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(headerName), $"{headerName} missing");
        Assert.Equal(expectedValue, response.Headers.GetValues(headerName).First());
    }

    [Fact]
    public async Task HtmlResponse_PermissionsPolicy_NotPresentByDefault()
    {
        using var host = await BuildHostAsync(opts => { /* use all defaults */ });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.False(response.Headers.Contains("Permissions-Policy"),
            "Permissions-Policy should not be emitted by default (null)");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Individual header property overrides
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ContentSecurityPolicy_Override_IsUsed()
    {
        using var host = await BuildHostAsync(opts =>
            opts.ContentSecurityPolicy = "default-src 'self'; img-src *");

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal("default-src 'self'; img-src *",
            response.Headers.GetValues("Content-Security-Policy").First());
    }

    [Fact]
    public async Task ReferrerPolicy_Override_IsUsed()
    {
        using var host = await BuildHostAsync(opts =>
            opts.ReferrerPolicy = "no-referrer");

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal("no-referrer",
            response.Headers.GetValues("Referrer-Policy").First());
    }

    [Fact]
    public async Task PermissionsPolicy_Set_IsEmitted()
    {
        using var host = await BuildHostAsync(opts =>
            opts.PermissionsPolicy = "geolocation=(), camera=()");

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.Contains("Permissions-Policy"),
            "Permissions-Policy should be present when set");
        Assert.Equal("geolocation=(), camera=()",
            response.Headers.GetValues("Permissions-Policy").First());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Setting a property to null suppresses that header
    // ──────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Content-Security-Policy", nameof(SecurityHeadersOptions.ContentSecurityPolicy))]
    [InlineData("Cross-Origin-Opener-Policy", nameof(SecurityHeadersOptions.CrossOriginOpenerPolicy))]
    [InlineData("Cross-Origin-Embedder-Policy", nameof(SecurityHeadersOptions.CrossOriginEmbedderPolicy))]
    [InlineData("Cross-Origin-Resource-Policy", nameof(SecurityHeadersOptions.CrossOriginResourcePolicy))]
    [InlineData("X-Frame-Options", nameof(SecurityHeadersOptions.XFrameOptions))]
    [InlineData("X-Content-Type-Options", nameof(SecurityHeadersOptions.XContentTypeOptions))]
    [InlineData("Referrer-Policy", nameof(SecurityHeadersOptions.ReferrerPolicy))]
    [InlineData("X-Permitted-Cross-Domain-Policies", nameof(SecurityHeadersOptions.XPermittedCrossDomainPolicies))]
    public async Task NullProperty_SuppressesHeader(string headerName, string propertyName)
    {
        using var host = await BuildHostAsync(opts =>
        {
            var prop = typeof(SecurityHeadersOptions).GetProperty(propertyName)!;
            prop.SetValue(opts, null);
        });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.False(response.Headers.Contains(headerName),
            $"{headerName} should be suppressed when property is null");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // CustomHeaders dict entries appear in response
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task HtmlResponse_CustomHeaders_ArePresent()
    {
        using var host = await BuildHostAsync(opts =>
        {
            opts.CustomHeaders["X-Test-Header"] = "test-value";
            opts.CustomHeaders["X-Another"] = "another-value";
        });

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Test-Header"), "X-Test-Header missing");
        Assert.Equal("test-value", response.Headers.GetValues("X-Test-Header").First());
        Assert.True(response.Headers.Contains("X-Another"), "X-Another missing");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Non-HTML responses do NOT get policy headers
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task NonHtmlResponse_PolicyHeaders_AreNotAdded()
    {
        using var host = await BuildHostAsync(
            opts => opts.CustomHeaders["X-Test-Header"] = "test-value",
            responseContentType: "application/json");

        var client = GetClient(host);
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(response.Headers.Contains("X-Test-Header"),
            "Custom headers should not be added to non-HTML responses");
        Assert.False(response.Headers.Contains("Content-Security-Policy"),
            "CSP should not be added to non-HTML responses");
        Assert.False(response.Headers.Contains("Referrer-Policy"),
            "Referrer-Policy should not be added to non-HTML responses");
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
                        opts.CustomHeaders["X-Static"] = "value";
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
            "Custom headers should not be added on exempt path");
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
