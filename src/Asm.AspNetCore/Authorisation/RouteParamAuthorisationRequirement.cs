using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Route parameter authorisation requirement.
/// </summary>
/// <param name="name">The name of the route parameter.</param>
public class RouteParamAuthorisationRequirement(string name) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the name of the route parameter.
    /// </summary>
    public string Name { get; } = name;
}
