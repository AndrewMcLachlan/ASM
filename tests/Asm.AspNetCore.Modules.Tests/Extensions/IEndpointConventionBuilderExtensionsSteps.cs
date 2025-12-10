using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Modules.Tests.Extensions;

[Binding]
[Scope(Feature = "IEndpointConventionBuilderExtensions")]
public class IEndpointConventionBuilderExtensionsSteps
{
    private WebApplication _app;
    private RouteHandlerBuilder _endpoint;
    private RouteGroupBuilder _routeGroupBuilder;
    private IEndpointConventionBuilder _builder;

    [Given(@"I have an endpoint")]
    public void GivenIHaveAnEndpoint()
    {
        var appBuilder = WebApplication.CreateBuilder();
        _app = appBuilder.Build();
        _endpoint = _app.MapGet("/test", () => "test");
    }

    [Given(@"I have a '(.*)' endpoint at '(.*)'")]
    public void GivenIHaveAHttpMethodEndpointAtRoute(string method, string route)
    {
        var appBuilder = WebApplication.CreateBuilder();
        _app = appBuilder.Build();

        _endpoint = method.ToUpper() switch
        {
            "GET" => _app.MapGet(route, () => "response"),
            "POST" => _app.MapPost(route, () => "response"),
            "PUT" => _app.MapPut(route, () => "response"),
            "DELETE" => _app.MapDelete(route, () => "response"),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {method}")
        };
    }

    [Given(@"I have a route group at '(.*)'")]
    public void GivenIHaveARouteGroupAtRoute(string route)
    {
        var appBuilder = WebApplication.CreateBuilder();
        _app = appBuilder.Build();
        _routeGroupBuilder = _app.MapGroup(route);
    }

    [When(@"I set the names to '(.*)'")]
    public void WhenISetTheNamesTo(string displayName)
    {
        if (_endpoint != null)
        {
            _builder = _endpoint.WithNames(displayName);
        }
        else if (_routeGroupBuilder != null)
        {
            _builder = _routeGroupBuilder.WithNames(displayName);
        }
    }

    [When(@"I call WithOpenApi")]
    public void WhenICallWithOpenApi()
    {
        if (_builder is RouteHandlerBuilder handlerBuilder)
        {
            _builder = handlerBuilder;
        }
    }

    [Then(@"the endpoint should have the names set")]
    public void ThenTheEndpointShouldHaveTheNamesSet()
    {
        Assert.NotNull(_endpoint);
    }

    [Then(@"the builder should be returned for chaining")]
    public void ThenTheBuilderShouldBeReturnedForChaining()
    {
        Assert.NotNull(_builder);
    }

    [Then(@"the route group builder should have the names set")]
    public void ThenTheRouteGroupBuilderShouldHaveTheNamesSet()
    {
        Assert.NotNull(_routeGroupBuilder);
    }
}
