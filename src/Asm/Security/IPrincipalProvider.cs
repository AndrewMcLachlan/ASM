using System.Security.Claims;

namespace Asm.Security;

/// <summary>
/// A provider for the current principal.
/// </summary>
public interface IPrincipalProvider
{
    /// <summary>
    /// Gets the current principal.
    /// </summary>
    ClaimsPrincipal? Principal { get; }
}
