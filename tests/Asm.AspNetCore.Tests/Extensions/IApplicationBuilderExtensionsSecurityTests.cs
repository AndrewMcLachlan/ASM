using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Extensions;

/// <summary>
/// Tests for <see cref="AsmAspNetCoreApplicationBuilderExtensions"/>:
/// <c>UseCanonicalUrls</c> and <c>UseStandardSecurityHeaders</c>.
/// Isolated from the existing Reqnroll feature tests.
/// </summary>
[Trait("Category", "Unit")]
public class IApplicationBuilderExtensionsSecurityTests
{
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
    // AddCanonicalUrls(configure) + DI-resolved UseCanonicalUrls() — options applied
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddCanonicalUrls_ConfiguredOptions_ResolvedByUseCanonicalUrls()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                    services.AddCanonicalUrls(opts => opts.ForceLowercase = false));
                webHost.Configure(app =>
                {
                    app.UseCanonicalUrls();
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestClient();
        // With ForceLowercase=false configured via DI, /Hello should pass through (200)
        var response = await client.GetAsync("/Hello", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Obsolete inline-configure overload still forwards correctly
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UseCanonicalUrls_ObsoleteInlineConfigure_StillApplies()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    app.UseCanonicalUrls(opts => opts.ForceLowercase = false);
#pragma warning restore CS0618
                    app.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var client = host.GetTestClient();
        var response = await client.GetAsync("/Hello", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Argument null checks
    // ──────────────────────────────────────────────────────────────────────────

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
