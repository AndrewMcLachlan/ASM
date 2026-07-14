using System.Security.Claims;
using Asm.AspNetCore.Authorisation;
using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Tests.Authorisation;

public class OneOfAuthorisationRequirementHandlerTests
{
    private sealed class TestSubRequirement(bool shouldSucceed) : IAuthorizationRequirement
    {
        public bool ShouldSucceed { get; } = shouldSucceed;
    }

    // Grants success when the single sub-requirement it is asked about is a TestSubRequirement that
    // should succeed. Records the resource it was passed.
    private sealed class FakeAuthorizationService : IAuthorizationService
    {
        public object? LastResource { get; private set; }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            LastResource = resource;

            var succeeded = requirements.All(r => r is TestSubRequirement { ShouldSucceed: true });
            return Task.FromResult(succeeded ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName) =>
            AuthorizeAsync(user, resource, [new TestSubRequirement(false)]);
    }

    private sealed class TestHandler(IAuthorizationService authorisationService, bool isAuthorisedResult)
        : OneOfAuthorisationRequirementHandler(authorisationService)
    {
        public bool IsAuthorisedInvoked { get; private set; }

        public object? IsAuthorisedResource { get; private set; }

        protected override ValueTask<bool> IsAuthorised(object? resource)
        {
            IsAuthorisedInvoked = true;
            IsAuthorisedResource = resource;
            return ValueTask.FromResult(isAuthorisedResult);
        }
    }

    private static AuthorizationHandlerContext CreateContext(OneOfAuthorisationRequirement requirement, object? resource = null) =>
        new([requirement], new ClaimsPrincipal(new ClaimsIdentity()), resource);

    /// <summary>
    /// Given an any-of requirement where one sub-requirement succeeds
    /// When the requirement is handled
    /// Then the context succeeds without failing
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncOneOptionSucceedsSucceeds()
    {
        var service = new FakeAuthorizationService();
        var handler = new TestHandler(service, isAuthorisedResult: false);
        var requirement = new OneOfAuthorisationRequirement(new TestSubRequirement(false), new TestSubRequirement(true));
        var context = CreateContext(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        Assert.False(context.HasFailed);
    }

    /// <summary>
    /// Given an any-of requirement where every sub-requirement fails
    /// When the requirement is handled
    /// Then the context neither succeeds nor fails
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncAllOptionsFailDoesNotFailTheRequirement()
    {
        var service = new FakeAuthorizationService();
        var handler = new TestHandler(service, isAuthorisedResult: false);
        var requirement = new OneOfAuthorisationRequirement(new TestSubRequirement(false), new TestSubRequirement(false));
        var context = CreateContext(requirement);

        await handler.HandleAsync(context);

        // In an any-of handler a failing option must not veto the whole requirement.
        Assert.False(context.HasSucceeded);
        Assert.False(context.HasFailed);
    }

    /// <summary>
    /// Given every sub-requirement fails but the handler's IsAuthorised grants access
    /// When the requirement is handled
    /// Then IsAuthorised is invoked and the context succeeds
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncAllOptionsFailIsAuthorisedInvokedAndCanGrant()
    {
        var service = new FakeAuthorizationService();
        var handler = new TestHandler(service, isAuthorisedResult: true);
        var requirement = new OneOfAuthorisationRequirement(new TestSubRequirement(false));
        var context = CreateContext(requirement);

        await handler.HandleAsync(context);

        Assert.True(handler.IsAuthorisedInvoked);
        Assert.True(context.HasSucceeded);
    }

    /// <summary>
    /// Given a resource on the authorization context
    /// When the requirement is handled
    /// Then the same resource is passed to the authorization service and IsAuthorised
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task HandleAsyncPassesResourceThrough()
    {
        var resource = new object();
        var service = new FakeAuthorizationService();
        var handler = new TestHandler(service, isAuthorisedResult: false);
        var requirement = new OneOfAuthorisationRequirement(new TestSubRequirement(false));
        var context = CreateContext(requirement, resource);

        await handler.HandleAsync(context);

        Assert.Same(resource, service.LastResource);
        Assert.Same(resource, handler.IsAuthorisedResource);
    }
}
