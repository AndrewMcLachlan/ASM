using System.Security.Claims;
using Asm.Security;
using Microsoft.AspNetCore.Http;

namespace Asm.Web.Security;

public class HttpContextPrincipalProvider : IPrincipalProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextPrincipalProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public ClaimsPrincipal? Principal => _contextAccessor.HttpContext?.User;
}
