using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Authorisation handler for a <see cref="OneOfAuthorisationRequirement"/> that succeeds when any of
/// the sub-requirements pass, or when the derived <see cref="IsAuthorised"/> check grants access.
/// </summary>
/// <param name="authorisationService">An <see cref="IAuthorizationService"/> instance.</param>
public abstract class OneOfAuthorisationRequirementHandler(IAuthorizationService authorisationService) : AuthorizationHandler<OneOfAuthorisationRequirement>
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OneOfAuthorisationRequirement requirement)
    {
        foreach (var subRequirement in requirement.Requirements)
        {
            // Pass the resource through so resource-based sub-requirements can evaluate it.
            var result = await authorisationService.AuthorizeAsync(context.User, context.Resource, subRequirement);

            if (result.Succeeded)
            {
                context.Succeed(requirement);
                return;
            }
        }

        // Give derived handlers a final, custom say based on the resource. In an any-of handler a
        // single failing option must never veto the whole requirement, so we never call context.Fail();
        // an unmet requirement is denied by the authorisation middleware anyway.
        if (await IsAuthorised(context.Resource))
        {
            context.Succeed(requirement);
        }
    }

    /// <summary>
    /// Performs a custom authorisation check for the requirement's resource, consulted when none of the
    /// sub-requirements succeed.
    /// </summary>
    /// <param name="resource">The resource associated with the current authorisation context, or <c>null</c>.</param>
    /// <returns><c>true</c> to grant access; otherwise <c>false</c>.</returns>
    protected abstract ValueTask<bool> IsAuthorised(object? resource);
}
