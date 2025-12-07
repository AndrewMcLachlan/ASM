using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Extensions;

[Binding]
public class HttpContextExtensionsSteps(ScenarioContext context)
{
    private HttpContext _httpContext;
    private string _result;

    [Given(@"I have an HttpContext with user claims")]
    public void GivenIHaveAnHttpContextWithUserClaims(Table table)
    {
        var claims = table.Rows.Select(row => new Claim(row["Type"], row["Value"])).ToList();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _httpContext = new DefaultHttpContext
        {
            User = principal
        };
    }

    [Given(@"I have a null HttpContext")]
    public void GivenIHaveANullHttpContext()
    {
        _httpContext = null;
    }

    [Given(@"I have an HttpContext with no user")]
    public void GivenIHaveAnHttpContextWithNoUser()
    {
        _httpContext = new DefaultHttpContext
        {
            User = null
        };
    }

    [Given(@"I have an HttpContext with empty name claim")]
    public void GivenIHaveAnHttpContextWithEmptyNameClaim()
    {
        var identity = new ClaimsIdentity([], "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _httpContext = new DefaultHttpContext
        {
            User = principal
        };
    }

    [When(@"I call GetUserName")]
    public void WhenICallGetUserName()
    {
        _result = _httpContext.GetUserName();
        context.AddResult(_result);
    }
}
