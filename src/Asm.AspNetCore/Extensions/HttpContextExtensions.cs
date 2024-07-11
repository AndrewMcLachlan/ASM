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
        string name;

        if (context?.User?.Identity is ClaimsIdentity identity)
        {
            name = $"{identity.Claims.SingleOrDefault(c => c.Type == "name")?.Value} ({identity.Claims.SingleOrDefault(c => c.Type == "preferred_username")?.Value})";
        }
        else
        {
            name = "-";
        }

        return name;
    }
}
