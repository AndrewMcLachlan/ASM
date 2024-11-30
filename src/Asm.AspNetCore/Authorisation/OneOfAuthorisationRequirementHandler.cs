using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Route parameter authorisation handler.
/// </summary>
/// <param name="authorisationService">An <see cref="IAuthorizationService"/> instance.</param>
public abstract class OneOfAuthorisationRequirementHandler(IAuthorizationService authorisationService) : AuthorizationHandler<OneOfAuthorisationRequirement>
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OneOfAuthorisationRequirement requirement)
    {
        foreach (var subRequirement in requirement.Requirements)
        {
            var result = await authorisationService.AuthorizeAsync(context.User, null, subRequirement);

            if (result.Succeeded)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    /// <summary>
    /// Performs authorisation based on the route parameter value.
    /// </summary>
    protected abstract ValueTask<bool> IsAuthorised(object value);

}
