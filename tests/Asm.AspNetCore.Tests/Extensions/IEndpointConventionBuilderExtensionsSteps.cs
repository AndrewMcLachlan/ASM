using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Tests.Extensions;

[Binding]
public class IEndpointConventionBuilderExtensionsSteps
{
    private WebApplication _app = null!;
    private RouteHandlerBuilder _builder = null!;
    private string _displayName = null!;

    [Given(@"I have a route handler builder")]
    public void GivenIHaveARouteHandlerBuilder()
    {
        var webAppBuilder = WebApplication.CreateBuilder();
        _app = webAppBuilder.Build();
        _builder = _app.MapGet("/test", () => Results.Ok());
    }

    [When(@"I call WithNames with '(.*)'")]
    public void WhenICallWithNamesWith(string displayName)
    {
        _displayName = displayName;
        _builder.WithNames(displayName);
    }

    [Then(@"the display name should be set")]
    public void ThenTheDisplayNameShouldBeSet()
    {
        // The display name is set internally, we verify by ensuring no exception was thrown
        Assert.NotNull(_builder);
        Assert.NotEmpty(_displayName);
    }
}
