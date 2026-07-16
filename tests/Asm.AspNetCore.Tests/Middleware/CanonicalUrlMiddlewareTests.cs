using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Middleware;

[Trait("Category", "Unit")]

public class CanonicalUrlMiddlewareTests
{
    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a test host with <see cref="CanonicalUrlMiddleware"/> and a terminal
    /// 200 handler. The HttpClient is configured to NOT follow redirects so we can
    /// assert on the 301 location header directly.
    /// </summary>
    private static async Task<(IHost host, HttpClient client)> BuildAsync(
        Action<CanonicalUrlOptions>? configure = null)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services => services.AddCanonicalUrls(configure));
                webHost.Configure(app =>
                {
                    // Options are resolved from DI (the options pattern).
                    app.UseCanonicalUrls();
                    app.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = 200;
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        // Don't follow redirects — we need to inspect the 301 Location header
        var client = host.GetTestServer().CreateClient();
        client.DefaultRequestHeaders.Clear();

        return (host, client);
    }

    private static string? Location(HttpResponseMessage r) =>
        r.Headers.Location?.ToString();

    // ──────────────────────────────────────────────────────────────────────────
    // ForceLowercase — uppercase path → 301 to lowercase
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the canonical URL middleware with default options
    /// When an uppercase path is requested
    /// Then it responds 301 redirecting to the lowercase path
    /// </summary>
    [Fact]
    public async Task UppercasePathRedirectsToLowercase()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/About", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RemoveTrailingSlash — path ending in / → 301 without slash
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the canonical URL middleware with default options
    /// When a path ending in a trailing slash is requested
    /// Then it responds 301 redirecting to the path without the slash
    /// </summary>
    [Fact]
    public async Task TrailingSlashRedirectsWithoutSlash()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/about/", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Canonical path passes through (200)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the canonical URL middleware with default options
    /// When an already-canonical path is requested
    /// Then the request passes through with 200
    /// </summary>
    [Fact]
    public async Task CanonicalPathPassesThroughWith200()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/about", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // LowercaseExcludedExtensions — .pdf is exempt by default
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the default excluded extensions include .pdf
    /// When an uppercase path ending in .PDF is requested
    /// Then it is exempt from lowercasing and passes through with 200
    /// </summary>
    [Fact]
    public async Task PdfExtensionUpperCasePassesThrough()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            // The default excluded extension is .pdf (case-insensitive)
            var response = await client.GetAsync("/something.PDF", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ExemptPathPrefixes — /umbraco prefix bypasses canonicalisation
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given an exempt path prefix is configured
    /// When an uppercase path under that prefix is requested
    /// Then canonicalisation is bypassed and the request passes through with 200
    /// </summary>
    [Fact]
    public async Task ExemptPrefixUppercasePathPassesThrough()
    {
        var (host, client) = await BuildAsync(opts =>
            opts.ExemptPathPrefixes = ["/umbraco"]);
        using (host)
        {
            var response = await client.GetAsync("/Umbraco/Foo", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ForceLowercase = false — uppercase path passes through
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given ForceLowercase is disabled
    /// When an uppercase path is requested
    /// Then the request passes through with 200
    /// </summary>
    [Fact]
    public async Task ForceLowercaseFalseUppercasePathPassesThrough()
    {
        var (host, client) = await BuildAsync(opts => opts.ForceLowercase = false);
        using (host)
        {
            var response = await client.GetAsync("/About", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RemoveTrailingSlash = false — trailing slash passes through
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given RemoveTrailingSlash is disabled
    /// When a path ending in a trailing slash is requested
    /// Then the request passes through with 200
    /// </summary>
    [Fact]
    public async Task RemoveTrailingSlashFalseTrailingSlashPassesThrough()
    {
        var (host, client) = await BuildAsync(opts => opts.RemoveTrailingSlash = false);
        using (host)
        {
            var response = await client.GetAsync("/about/", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Query string is preserved on redirect
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the canonical URL middleware with default options
    /// When an uppercase path with a query string is requested
    /// Then the 301 redirect preserves the query string
    /// </summary>
    [Fact]
    public async Task UppercasePathWithQueryStringRedirectPreservesQueryString()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/About?x=1", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about?x=1", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // PathBase is preserved on redirect (app hosted under a sub-path)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the app is hosted under a path base
    /// When an uppercase path under that base is requested
    /// Then the 301 redirect keeps the path base
    /// </summary>
    [Fact]
    public async Task UppercasePathUnderPathBaseRedirectKeepsPathBase()
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app =>
                {
                    app.UsePathBase("/app");
                    app.UseCanonicalUrls();
                    app.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = 200;
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        using (host)
        {
            var client = host.GetTestServer().CreateClient();
            client.DefaultRequestHeaders.Clear();

            var response = await client.GetAsync("/app/About", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/app/about", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Non-GET/HEAD methods are not redirected (would drop the request body)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Given the canonical URL middleware with default options
    /// When a POST is made to an uppercase path
    /// Then it is not redirected and passes through with 200
    /// </summary>
    [Fact]
    public async Task PostToUppercasePathPassesThrough()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.PostAsync("/About", new StringContent("body"), TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
