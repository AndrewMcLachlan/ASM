using System.Net;
using System.Net.Http;
using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Middleware;

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
                webHost.Configure(app =>
                {
                    app.UseCanonicalUrls(configure);
                    app.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = 200;
                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync();

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

    [Fact]
    public async Task UppercasePath_Redirects_ToLowercase()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/About");

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RemoveTrailingSlash — path ending in / → 301 without slash
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task TrailingSlash_Redirects_WithoutSlash()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/about/");

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about", Location(response));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Canonical path passes through (200)
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CanonicalPath_PassesThrough_With200()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/about");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // LowercaseExcludedExtensions — .pdf is exempt by default
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PdfExtension_UpperCase_PassesThrough()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            // The default excluded extension is .pdf (case-insensitive)
            var response = await client.GetAsync("/something.PDF");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ExemptPathPrefixes — /umbraco prefix bypasses canonicalisation
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExemptPrefix_UppercasePath_PassesThrough()
    {
        var (host, client) = await BuildAsync(opts =>
            opts.ExemptPathPrefixes = ["/umbraco"]);
        using (host)
        {
            var response = await client.GetAsync("/Umbraco/Foo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ForceLowercase = false — uppercase path passes through
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ForceLowercaseFalse_UppercasePath_PassesThrough()
    {
        var (host, client) = await BuildAsync(opts => opts.ForceLowercase = false);
        using (host)
        {
            var response = await client.GetAsync("/About");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RemoveTrailingSlash = false — trailing slash passes through
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveTrailingSlashFalse_TrailingSlash_PassesThrough()
    {
        var (host, client) = await BuildAsync(opts => opts.RemoveTrailingSlash = false);
        using (host)
        {
            var response = await client.GetAsync("/about/");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Query string is preserved on redirect
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UppercasePath_WithQueryString_RedirectPreservesQueryString()
    {
        var (host, client) = await BuildAsync();
        using (host)
        {
            var response = await client.GetAsync("/About?x=1");

            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/about?x=1", Location(response));
        }
    }
}
