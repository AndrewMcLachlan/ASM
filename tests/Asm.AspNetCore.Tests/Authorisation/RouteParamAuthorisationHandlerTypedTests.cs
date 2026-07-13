using System.Security.Claims;
using Asm.AspNetCore.Authorisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Tests.Authorisation;

public class RouteParamAuthorisationHandlerTypedTests
{
    private sealed class TestHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }

    private sealed class TestRequirement(string name) : RouteParamAuthorisationRequirement(name);

    // Strongly-typed handler: extracts the route value as a Guid before authorising.
    private sealed class GuidRouteParamHandler(IHttpContextAccessor accessor, Guid allowed)
        : RouteParamAuthorisationHandler<TestRequirement, Guid>(accessor)
    {
        public Guid? ReceivedValue { get; private set; }

        protected override ValueTask<bool> IsAuthorised(Guid value)
        {
            ReceivedValue = value;
            return ValueTask.FromResult(value == allowed);
        }
    }

    private static (GuidRouteParamHandler handler, AuthorizationHandlerContext context, TestRequirement requirement) Setup(string routeValue, Guid allowed)
    {
        var httpContext = new DefaultHttpContext
        {
            Request = { RouteValues = new RouteValueDictionary { { "id", routeValue } } },
        };

        var accessor = new TestHttpContextAccessor(httpContext);
        var requirement = new TestRequirement("id");
        var handler = new GuidRouteParamHandler(accessor, allowed);
        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(new ClaimsIdentity()), null);
        return (handler, context, requirement);
    }

    /// <summary>
    /// Given a route value that converts to the allowed Guid
    /// When the requirement is handled
    /// Then the converted value is received and the context succeeds
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncConvertsRouteValueAndAuthorises()
    {
        var allowed = Guid.NewGuid();
        var (handler, context, _) = Setup(allowed.ToString(), allowed);

        await handler.HandleAsync(context);

        Assert.Equal(allowed, handler.ReceivedValue);
        Assert.True(context.HasSucceeded);
    }

    /// <summary>
    /// Given a route value that converts to a Guid other than the allowed one
    /// When the requirement is handled
    /// Then the converted value is received and the context fails
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncUnauthorisedTypedValueFails()
    {
        var allowed = Guid.NewGuid();
        var other = Guid.NewGuid();
        var (handler, context, _) = Setup(other.ToString(), allowed);

        await handler.HandleAsync(context);

        Assert.Equal(other, handler.ReceivedValue);
        Assert.True(context.HasFailed);
    }

    /// <summary>
    /// Given a route value that cannot be converted to a Guid
    /// When the requirement is handled
    /// Then no value is received and the context fails
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncUnconvertibleRouteValueFails()
    {
        var allowed = Guid.NewGuid();
        var (handler, context, _) = Setup("not-a-guid", allowed);

        await handler.HandleAsync(context);

        Assert.Null(handler.ReceivedValue);
        Assert.True(context.HasFailed);
    }
}
