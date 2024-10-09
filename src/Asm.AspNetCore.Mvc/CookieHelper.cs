using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Mvc;

/// <summary>
///
/// </summary>
public static class CookieHelper
{
    /// <summary>
    /// The name of the cookie that stores the user's acceptance of cookies.
    /// </summary>
    public const string CookieAcceptanceCookieName = "AcceptedCookies";

    /// <summary>
    /// Checks if the user has accepted cookies.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A value indicating whether the user has accepted cookies.</returns>
    public static bool HasAcceptedCookies(HttpContext context)
    {
        string? acceptance = context.Request.Cookies[CookieAcceptanceCookieName];

        return acceptance != null && acceptance == "1";
    }

    /// <summary>
    /// Accepts cookies.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public static void AcceptCookies(HttpContext context)
    {
        context.Response.Cookies.Append(CookieAcceptanceCookieName, "1", new CookieOptions { Expires = DateTime.Now.AddYears(10), Secure = true });
    }
}
