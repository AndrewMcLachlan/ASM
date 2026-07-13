using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Mvc.Tests.Extensions;

[Trait("Category", "Unit")]
public class HttpRequestExtensionsTests
{
    /// <summary>
    /// Given a request with a Referer header
    /// When UrlReferrer is read
    /// Then the header value is returned
    /// </summary>
    [Fact]
    public void UrlReferrerReturnsRefererHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Referer = "https://referrer.example/";

        Assert.Equal("https://referrer.example/", context.Request.UrlReferrer());
    }

    /// <summary>
    /// Given a request with no Referer header
    /// When UrlReferrer is read
    /// Then null is returned
    /// </summary>
    [Fact]
    public void UrlReferrerReturnsNullWhenHeaderAbsent()
    {
        Assert.Null(new DefaultHttpContext().Request.UrlReferrer());
    }

    /// <summary>
    /// Given a request whose X-Requested-With header is XMLHttpRequest
    /// When IsAjaxRequest is checked
    /// Then it returns true
    /// </summary>
    [Fact]
    public void IsAjaxRequestTrueWhenXRequestedWithIsXmlHttpRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.XRequestedWith = "XMLHttpRequest";

        Assert.True(context.Request.IsAjaxRequest());
    }

    /// <summary>
    /// Given a request with no X-Requested-With header
    /// When IsAjaxRequest is checked
    /// Then it returns false
    /// </summary>
    [Fact]
    public void IsAjaxRequestFalseWhenHeaderAbsent()
    {
        Assert.False(new DefaultHttpContext().Request.IsAjaxRequest());
    }

    /// <summary>
    /// Given a request with a host and port
    /// When the obsolete OriginHost is read
    /// Then it returns the host string
    /// </summary>
    [Fact]
    public void OriginHostReturnsRequestHost()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("host.example", 8080);

#pragma warning disable CS0618 // exercising the obsolete member for coverage
        Assert.Equal("host.example:8080", context.Request.OriginHost());
#pragma warning restore CS0618
    }
}
