using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for the new overloads added to <see cref="IApplicationBuilderExtensions"/>:
/// <c>UseSecurityHeaders(Action&lt;SecurityHeadersOptions&gt;)</c> and
/// <c>UseCanonicalUrls(Action&lt;CanonicalUrlOptions&gt;?)</c>.
/// Isolated from the existing Reqnroll feature tests.
/// </summary>
public class IApplicationBuilderExtensionsSecurityTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // UseSecurityHeaders(configure) — smoke test: pipeline runs end-to-end
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UseSecurityHeaders_WithConfigure_PipelineRunsAndHeadersAreAdded()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UseSecurityHeaders(opts =>
                    {
                        opts.CustomHeaders["X-Smoke-Test"] = "ok";
                    });
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("smoke");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestClient();
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Smoke-Test"),
            "Configured static header should be present");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UseCanonicalUrls() — smoke test: pipeline runs end-to-end
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UseCanonicalUrls_NoConfigure_PipelineRunsAndRedirectsUppercase()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UseCanonicalUrls();
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("canonical");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestClient();
        var response = await client.GetAsync("/Hello", TestContext.Current.CancellationToken);

        // Default ForceLowercase=true should 301 to /hello
        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal("/hello", response.Headers.Location?.ToString());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UseCanonicalUrls with configure — smoke test
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UseCanonicalUrls_WithConfigure_PipelineRunsAndOptionsApplied()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UseCanonicalUrls(opts => opts.ForceLowercase = false);
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestClient();
        // With ForceLowercase=false, /Hello should pass through (200)
        var response = await client.GetAsync("/Hello", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Argument null checks
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public void UseSecurityHeaders_NullApp_ThrowsArgumentNullException()
    {
        IApplicationBuilder app = null!;
        Assert.Throws<ArgumentNullException>(() => app.UseSecurityHeaders((Action<SecurityHeadersOptions>)(_ => { })));
    }

    [Fact]
    public void UseSecurityHeaders_NullConfigure_ThrowsArgumentNullException()
    {
        // We need a real (or minimal) IApplicationBuilder — use a basic test double
        var app = new FakeApplicationBuilder();
        Assert.Throws<ArgumentNullException>(() => app.UseSecurityHeaders((Action<SecurityHeadersOptions>)null!));
    }

    [Fact]
    public void UseCanonicalUrls_NullApp_ThrowsArgumentNullException()
    {
        IApplicationBuilder app = null!;
        Assert.Throws<ArgumentNullException>(() => app.UseCanonicalUrls());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Minimal IApplicationBuilder stub for null-check tests that don't need TestServer
    // ──────────────────────────────────────────────────────────────────────────

    private sealed class FakeApplicationBuilder : IApplicationBuilder
    {
        public IServiceProvider ApplicationServices { get; set; } = null!;
        public Microsoft.AspNetCore.Http.Features.IFeatureCollection ServerFeatures { get; } = new Microsoft.AspNetCore.Http.Features.FeatureCollection();
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
        public RequestDelegate Build() => _ => Task.CompletedTask;
        public IApplicationBuilder New() => new FakeApplicationBuilder();
        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => this;
    }
}
