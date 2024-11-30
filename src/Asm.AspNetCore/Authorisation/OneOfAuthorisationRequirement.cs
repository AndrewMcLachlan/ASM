using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Represents an authorisation requirement that requires at least one of the specified requirements to be met.
/// </summary>
/// <param name="requirements">The requirements to check.</param>
public class OneOfAuthorisationRequirement(params IAuthorizationRequirement[] requirements) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the requirements to check.
    /// </summary>
    public IEnumerable<IAuthorizationRequirement> Requirements { get; } = requirements;
}
