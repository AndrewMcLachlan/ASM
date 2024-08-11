using System.Security.Claims;
using Asm.Security;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Security;

/// <summary>
/// A principal provider that uses the principal from the current HTTP context.
/// </summary>
/// <param name="contextAccessor">An <see cref="IHttpContextAccessor"/> implementation.</param>
public class HttpContextPrincipalProvider(IHttpContextAccessor contextAccessor) : IPrincipalProvider
{
    /// <summary>
    /// Gets the principal from the current HTTP context.
    /// </summary>
    public ClaimsPrincipal? Principal => contextAccessor.HttpContext?.User;
}
