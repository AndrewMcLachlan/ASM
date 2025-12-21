#nullable enable

using System.Security.Claims;
using Asm.AspNetCore.Authorisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Tests.Authorisation;

[Binding]
public class RouteParamAuthorisationSteps
{
    private IHttpContextAccessor _httpContextAccessor = null!;
    private TestRouteParamHandler _handler = null!;
    private AuthorizationHandlerContext _authContext = null!;
    private TestRouteParamRequirement _requirement = null!;
    private bool _isAuthorised;

    [Given(@"I have an HttpContext with route parameter '(.*)' value '(.*)'")]
    public void GivenIHaveAnHttpContextWithRouteParameterValue(string paramName, string paramValue)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { paramName, paramValue }
        };

        _httpContextAccessor = new TestHttpContextAccessor(httpContext);
    }

    [Given(@"I have an HttpContext with no route parameters")]
    public void GivenIHaveAnHttpContextWithNoRouteParameters()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary();

        _httpContextAccessor = new TestHttpContextAccessor(httpContext);
    }

    [Given(@"I have an authorised route param handler")]
    public void GivenIHaveAnAuthorisedRouteParamHandler()
    {
        _isAuthorised = true;
        SetupHandler();
    }

    [Given(@"I have an unauthorised route param handler")]
    public void GivenIHaveAnUnauthorisedRouteParamHandler()
    {
        _isAuthorised = false;
        SetupHandler();
    }

    private void SetupHandler()
    {
        _requirement = new TestRouteParamRequirement("id");
        _handler = new TestRouteParamHandler(_httpContextAccessor, _isAuthorised);

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        _authContext = new AuthorizationHandlerContext([_requirement], user, null);
    }

    [When(@"the route param handler handles the requirement")]
    public async Task WhenTheRouteParamHandlerHandlesTheRequirement()
    {
        await _handler.HandleAsync(_authContext);
    }

    [Then(@"the authorisation should succeed")]
    public void ThenTheAuthorisationShouldSucceed()
    {
        Assert.True(_authContext.HasSucceeded, "Expected authorization to succeed");
    }

    [Then(@"the authorisation should fail")]
    public void ThenTheAuthorisationShouldFail()
    {
        Assert.True(_authContext.HasFailed, "Expected authorization to fail");
    }

    private class TestHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }

    private class TestRouteParamRequirement(string name) : RouteParamAuthorisationRequirement(name)
    {
    }

    private class TestRouteParamHandler(IHttpContextAccessor httpContextAccessor, bool shouldAuthorise)
        : RouteParamAuthorisationHandler<TestRouteParamRequirement>(httpContextAccessor)
    {
        protected override ValueTask<bool> IsAuthorised(object value)
        {
            return ValueTask.FromResult(shouldAuthorise);
        }
    }
}
