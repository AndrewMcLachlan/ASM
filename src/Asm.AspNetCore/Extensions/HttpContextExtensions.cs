using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore;

/// <summary>
/// Extension methods for <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    private const string UserNameItemKey = "Asm.UserName";

    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> instance that this method extends.</param>
    /// <returns>The current user's name, or "-" if the current user is not available.</returns>
    public static string GetUserName(this HttpContext? context)
    {
        if (context is null)
        {
            return "-";
        }

        // Cache per request: this runs once per log record / activity / request-enrichment, so
        // re-scanning the claims each time would be wasteful.
        if (context.Items.TryGetValue(UserNameItemKey, out var cached) && cached is string cachedName)
        {
            return cachedName;
        }

        var name = ResolveUserName(context);
        context.Items[UserNameItemKey] = name;
        return name;
    }

    private static string ResolveUserName(HttpContext context)
    {
        if (context.User?.Identity is not ClaimsIdentity identity)
        {
            return "-";
        }

        // FindFirst tolerates duplicate claims (SingleOrDefault would throw) and scans once.
        var name = identity.FindFirst("name")?.Value;
        if (name is null)
        {
            return "-";
        }

        var preferredUsername = identity.FindFirst("preferred_username")?.Value;
        return $"{name} ({preferredUsername})";
    }
}
