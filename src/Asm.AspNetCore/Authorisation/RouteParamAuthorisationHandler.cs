using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Route parameter authorisation handler.
/// </summary>
/// <param name="httpContextAccessor">An <see cref="IHttpContextAccessor"/> instance.</param>
public abstract class RouteParamAuthorisationHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<RouteParamAuthorisationRequirement>
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RouteParamAuthorisationRequirement requirement)
    {
        if (httpContextAccessor.HttpContext?.Request.RouteValues.TryGetValue(requirement.Name, out var value) == true && value is not null)
        {
            if (await IsAuthorised(value))
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
