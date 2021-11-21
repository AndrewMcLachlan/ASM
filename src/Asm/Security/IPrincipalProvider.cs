using System.Security.Claims;

namespace Asm.Security;

public interface IPrincipalProvider
{
    ClaimsPrincipal? Principal { get; }
}
