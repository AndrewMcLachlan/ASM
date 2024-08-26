using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Asm.Cqrs.AspNetCore.Tests;

[Binding]
public class IEndpointRouteBuilderExtensionsSteps
{
    private IEndpointRouteBuilder _endpoints;
    private RouteHandlerBuilder _routeHandlerBuilder;

    [Given(@"I have an IEndpointRouteBuilder")]
    public void GivenIHaveAnIEndpointRouteBuilder()
    {
        var mockEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();
        mockEndpointRouteBuilder.Setup(e => e.ServiceProvider).Returns(new Mock<IServiceProvider>().Object);
        mockEndpointRouteBuilder.Setup(e => e.DataSources).Returns([]);
        _endpoints = mockEndpointRouteBuilder.Object;
    }

    [When(@"I map a query with pattern ""(.*)""")]
    public void WhenIMapAQueryWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapQuery<TestQuery, TestResponse>(pattern);
    }

    [When(@"I map a POST create command with pattern ""(.*)"" and route name ""(.*)""")]
    public void WhenIMapAPostCreateCommandWithPatternAndRouteName(string pattern, string routeName)
    {
        _routeHandlerBuilder = _endpoints.MapPostCreate<TestCommand, TestResponse>(pattern, routeName, response => new { id = response.Id });
    }

    [When(@"I map a DELETE command with pattern ""(.*)""")]
    public void WhenIMapADeleteCommandWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapDelete<TestCommandEmpty>(pattern);
    }

    [Then(@"the route should be mapped correctly")]
    public void ThenTheRouteShouldBeMappedCorrectly()
    {
        Assert.NotNull(_routeHandlerBuilder);
    }
}

public class TestQuery : IQuery<TestResponse> { }
public class TestCommand : ICommand<TestResponse> { }
public class TestCommandEmpty : ICommand { }
public class TestResponse
{
    public int Id { get; set; }
}
