using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore;

public static class HttpContextExtensions
{
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
