using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Mvc.Tests;

[Trait("Category", "Unit")]
public class CookieHelperTests
{
    private static HttpContext ContextWithCookie(string value)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = $"{CookieHelper.CookieAcceptanceCookieName}={value}";
        return context;
    }

    /// <summary>
    /// Given a request whose acceptance cookie is set to "1"
    /// When HasAcceptedCookies is checked
    /// Then it returns true
    /// </summary>
    [Fact]
    public void AcceptanceCookieSetToOneReturnsTrue()
    {
        Assert.True(CookieHelper.HasAcceptedCookies(ContextWithCookie("1")));
    }

    /// <summary>
    /// Given a request whose acceptance cookie holds a value other than "1"
    /// When HasAcceptedCookies is checked
    /// Then it returns false
    /// </summary>
    [Fact]
    public void AcceptanceCookieWithOtherValueReturnsFalse()
    {
        Assert.False(CookieHelper.HasAcceptedCookies(ContextWithCookie("0")));
    }

    /// <summary>
    /// Given a request with no acceptance cookie
    /// When HasAcceptedCookies is checked
    /// Then it returns false
    /// </summary>
    [Fact]
    public void NoAcceptanceCookieReturnsFalse()
    {
        Assert.False(CookieHelper.HasAcceptedCookies(new DefaultHttpContext()));
    }

    /// <summary>
    /// Given a response
    /// When AcceptCookies is called
    /// Then a secure acceptance cookie set to "1" is appended
    /// </summary>
    [Fact]
    public void AcceptCookiesAppendsSecureAcceptanceCookie()
    {
        var context = new DefaultHttpContext();

        CookieHelper.AcceptCookies(context);

        string setCookie = context.Response.Headers.SetCookie.ToString();
        Assert.Contains($"{CookieHelper.CookieAcceptanceCookieName}=1", setCookie);
        Assert.Contains("secure", setCookie, StringComparison.OrdinalIgnoreCase);
    }
}
