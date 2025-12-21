#nullable enable

using System.Diagnostics;
using System.Security.Claims;
using Asm.AspNetCore.OpenTelemetry;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.OpenTelemetry;

[Binding]
public class HttpContextProcessorsSteps
{
    private IHttpContextAccessor _httpContextAccessor = null!;
    private HttpContextTraceProcessor _traceProcessor = null!;
    private Activity _activity = null!;

    [Given(@"I have an HttpContext with username '(.*)'")]
    public void GivenIHaveAnHttpContextWithUsername(string username)
    {
        var claims = new List<Claim>
        {
            new("name", username),
            new("preferred_username", $"{username}@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _httpContextAccessor = new TestHttpContextAccessor(httpContext);
    }

    [Given(@"I have a null HttpContext for the processor")]
    public void GivenIHaveANullHttpContextForTheProcessor()
    {
        _httpContextAccessor = new TestHttpContextAccessor(null);
    }

    [When(@"the trace processor processes an activity")]
    public void WhenTheTraceProcessorProcessesAnActivity()
    {
        _traceProcessor = new HttpContextTraceProcessor(_httpContextAccessor);

        var source = new ActivitySource("TestSource");
        _activity = source.StartActivity("TestActivity") ?? new Activity("TestActivity").Start();

        _traceProcessor.OnEnd(_activity);
    }

    [Then(@"the activity should have a User custom property with value '(.*)'")]
    public void ThenTheActivityShouldHaveAUserCustomPropertyWithValue(string expectedUsername)
    {
        Assert.NotNull(_activity);
        var userProperty = _activity.GetCustomProperty("User");
        Assert.NotNull(userProperty);
        Assert.Contains(expectedUsername, userProperty?.ToString());
    }

    [Then(@"the activity should not have a User custom property")]
    public void ThenTheActivityShouldNotHaveAUserCustomProperty()
    {
        Assert.NotNull(_activity);
        var userProperty = _activity.GetCustomProperty("User");
        Assert.Null(userProperty);
    }

    private class TestHttpContextAccessor(HttpContext? httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }
}
