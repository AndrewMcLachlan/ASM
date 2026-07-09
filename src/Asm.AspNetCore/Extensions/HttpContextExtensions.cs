using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore;

/// <summary>
/// Extension methods for <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> instance that this method extends.</param>
    /// <returns>The current user's name, or "-" if the current user is not available.</returns>
    public static string GetUserName(this HttpContext? context)
    {
        if (context?.User?.Identity is not ClaimsIdentity identity)
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
