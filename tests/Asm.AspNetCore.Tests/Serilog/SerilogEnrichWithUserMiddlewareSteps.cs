using System.Security.Claims;
using Asm.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Serilog;

[Binding]
public class SerilogEnrichWithUserMiddlewareSteps
{
    private SerilogEnrichWithUserMiddleware _middleware = null!;
    private DefaultHttpContext _httpContext = null!;
    private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
    private bool _nextDelegateCalled;

    [Given(@"I have a SerilogEnrichWithUserMiddleware")]
    public void GivenIHaveASerilogEnrichWithUserMiddleware()
    {
        _nextDelegateCalled = false;
        _middleware = new SerilogEnrichWithUserMiddleware(_ =>
        {
            _nextDelegateCalled = true;
            return Task.CompletedTask;
        });
    }

    [Given(@"I have an HttpContext with user '(.*)'")]
    public void GivenIHaveAnHttpContextWithUser(string username)
    {
        var claims = new List<Claim>
        {
            new("name", username),
            new("preferred_username", $"{username}@test.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
    }

    [Given(@"I have an HttpContext without a user for the middleware")]
    public void GivenIHaveAnHttpContextWithoutAUserForTheMiddleware()
    {
        _httpContext = new DefaultHttpContext();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
    }

    [When(@"I invoke the middleware")]
    public async Task WhenIInvokeTheMiddleware()
    {
        await _middleware.Invoke(_httpContext, _httpContextAccessor.Object);
    }

    [Then(@"the next delegate should have been called")]
    public void ThenTheNextDelegateShouldHaveBeenCalled()
    {
        Assert.True(_nextDelegateCalled);
    }
}
