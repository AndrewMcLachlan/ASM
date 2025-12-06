using System.Security.Claims;
using Asm.AspNetCore.Security;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Security;

[Binding]
public class HttpContextPrincipalProviderSteps
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private HttpContextPrincipalProvider _provider;
    private ClaimsPrincipal _result;
    private ClaimsPrincipal _expectedPrincipal;

    [Given(@"I have an HttpContextAccessor with a user principal")]
    public void GivenIHaveAnHttpContextAccessorWithAUserPrincipal()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _expectedPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = _expectedPrincipal
        };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    [Given(@"I have an HttpContextAccessor with no HttpContext")]
    public void GivenIHaveAnHttpContextAccessorWithNoHttpContext()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);
    }

    [Given(@"I have an HttpContextAccessor with HttpContext but no user")]
    public void GivenIHaveAnHttpContextAccessorWithHttpContextButNoUser()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns((ClaimsPrincipal)null);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
    }

    [When(@"I get the principal from HttpContextPrincipalProvider")]
    public void WhenIGetThePrincipalFromHttpContextPrincipalProvider()
    {
        _provider = new HttpContextPrincipalProvider(_httpContextAccessorMock.Object);
        _result = _provider.Principal;
    }

    [Then(@"the principal should not be null")]
    public void ThenThePrincipalShouldNotBeNull()
    {
        Assert.NotNull(_result);
    }

    [Then(@"the principal should be null")]
    public void ThenThePrincipalShouldBeNull()
    {
        Assert.Null(_result);
    }

    [Then(@"the principal should have the expected identity")]
    public void ThenThePrincipalShouldHaveTheExpectedIdentity()
    {
        Assert.Equal(_expectedPrincipal.Identity.Name, _result.Identity.Name);
    }
}
