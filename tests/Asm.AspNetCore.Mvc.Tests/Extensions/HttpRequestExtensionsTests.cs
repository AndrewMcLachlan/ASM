using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Mvc.Tests.Extensions;

// OriginHost is [Obsolete] but still shipped, so keep it covered.
#pragma warning disable CS0618 // Type or member is obsolete
public class HttpRequestExtensionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void OriginHost_ReturnsRequestHost()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("example.com");

        Assert.Equal("example.com", context.Request.OriginHost());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OriginHost_IgnoresRawXForwardedHostHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("example.com");
        // A client-supplied forwarded host must NOT be reflected (host/cache-poisoning guard);
        // the Forwarded Headers middleware is responsible for updating Request.Host from trusted proxies.
        context.Request.Headers["X-Forwarded-Host"] = "evil.example";

        Assert.Equal("example.com", context.Request.OriginHost());
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
