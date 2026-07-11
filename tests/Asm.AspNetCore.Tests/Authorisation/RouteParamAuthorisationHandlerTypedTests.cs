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

    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsync_ConvertsRouteValueAndAuthorises()
    {
        var allowed = Guid.NewGuid();
        var (handler, context, _) = Setup(allowed.ToString(), allowed);

        await handler.HandleAsync(context);

        Assert.Equal(allowed, handler.ReceivedValue);
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsync_UnauthorisedTypedValue_Fails()
    {
        var allowed = Guid.NewGuid();
        var other = Guid.NewGuid();
        var (handler, context, _) = Setup(other.ToString(), allowed);

        await handler.HandleAsync(context);

        Assert.Equal(other, handler.ReceivedValue);
        Assert.True(context.HasFailed);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsync_UnconvertibleRouteValue_Fails()
    {
        var allowed = Guid.NewGuid();
        var (handler, context, _) = Setup("not-a-guid", allowed);

        await handler.HandleAsync(context);

        Assert.Null(handler.ReceivedValue);
        Assert.True(context.HasFailed);
    }
}
