using Microsoft.AspNetCore.Http;

namespace Asm.AmCom.Web;

public static class CookieHelper
{
    public const string CookieAcceptanceCookieName = "AcceptedCookies";

    public static bool HasAcceptedCookies(HttpContext context)
    {
        string? acceptance = context.Request.Cookies[CookieAcceptanceCookieName];

        return acceptance != null && acceptance == "1";
    }

    public static void AcceptCookies(HttpContext context)
    {
        context.Response.Cookies.Append(CookieAcceptanceCookieName, "1", new CookieOptions { Expires = DateTime.Now.AddYears(10), Secure = true });
    }
}
