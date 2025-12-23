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

    [When(@"I map a paged query with pattern ""(.*)""")]
    public void WhenIMapAPagedQueryWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapPagedQuery<TestPagedQuery, TestResponse>(pattern);
    }

    [When(@"I map a DELETE command with response with pattern ""(.*)""")]
    public void WhenIMapADeleteCommandWithResponseWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapDelete<TestCommand, TestResponse>(pattern);
    }

    [When(@"I map a POST command with response with pattern ""(.*)""")]
    public void WhenIMapAPostCommandWithResponseWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapCommand<TestCommand, TestResponse>(pattern);
    }

    [When(@"I map a POST command without response with pattern ""([^""]+)""$")]
    public void WhenIMapAPostCommandWithoutResponseWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapCommand<TestCommandEmpty>(pattern);
    }

    [When(@"I map a PUT create command with pattern ""(.*)"" and route name ""(.*)""")]
    public void WhenIMapAPutCreateCommandWithPatternAndRouteName(string pattern, string routeName)
    {
        _routeHandlerBuilder = _endpoints.MapPutCreate<TestCommand, TestResponse>(pattern, routeName, response => new { id = response.Id });
    }

    [When(@"I map a PATCH command with pattern ""([^""]+)""$")]
    public void WhenIMapAPatchCommandWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapPatchCommand<TestCommand, TestResponse>(pattern);
    }

    [When(@"I map a PUT command with response with pattern ""(.*)""")]
    public void WhenIMapAPutCommandWithResponseWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapPutCommand<TestCommand, TestResponse>(pattern);
    }

    [When(@"I map a PUT command without response with pattern ""(.*)""")]
    public void WhenIMapAPutCommandWithoutResponseWithPattern(string pattern)
    {
        _routeHandlerBuilder = _endpoints.MapPutCommand<TestCommandEmpty>(pattern);
    }

    [When(@"I map a POST create command with pattern ""(.*)"", route name ""(.*)"" and binding ""(.*)""")]
    public void WhenIMapAPostCreateCommandWithPatternRouteNameAndBinding(string pattern, string routeName, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapPostCreate<TestCommand, TestResponse>(pattern, routeName, response => new { id = response.Id }, commandBinding);
    }

    [When(@"I map a PUT create command with pattern ""(.*)"", route name ""(.*)"" and binding ""(.*)""")]
    public void WhenIMapAPutCreateCommandWithPatternRouteNameAndBinding(string pattern, string routeName, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapPutCreate<TestCommand, TestResponse>(pattern, routeName, response => new { id = response.Id }, commandBinding);
    }

    [When(@"I map a POST command with response, pattern ""(.*)"" and binding ""(.*)""")]
    public void WhenIMapAPostCommandWithResponsePatternAndBinding(string pattern, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapCommand<TestCommand, TestResponse>(pattern, commandBinding);
    }

    [When(@"I map a POST command without response with pattern ""(.*)"" and status code (.*)")]
    public void WhenIMapAPostCommandWithoutResponseWithPatternAndStatusCode(string pattern, int statusCode)
    {
        _routeHandlerBuilder = _endpoints.MapCommand<TestCommandEmpty>(pattern, statusCode);
    }

    [When(@"I map a POST command without response with pattern ""(.*)"", status code (.*) and binding ""(.*)""")]
    public void WhenIMapAPostCommandWithoutResponseWithPatternStatusCodeAndBinding(string pattern, int statusCode, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapCommand<TestCommandEmpty>(pattern, statusCode, commandBinding);
    }

    [When(@"I map a PATCH command with pattern ""(.*)"" and binding ""(.*)""")]
    public void WhenIMapAPatchCommandWithPatternAndBinding(string pattern, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapPatchCommand<TestCommand, TestResponse>(pattern, commandBinding);
    }

    [When(@"I map a PUT command with pattern ""(.*)"" and binding ""(.*)""")]
    public void WhenIMapAPutCommandWithPatternAndBinding(string pattern, string binding)
    {
        var commandBinding = Enum.Parse<CommandBinding>(binding);
        _routeHandlerBuilder = _endpoints.MapPutCommand<TestCommand, TestResponse>(pattern, commandBinding);
    }

    [Then(@"the route should be mapped correctly")]
    public void ThenTheRouteShouldBeMappedCorrectly()
    {
        Assert.NotNull(_routeHandlerBuilder);
    }
}

public class TestQuery : IQuery<TestResponse> { }
public class TestPagedQuery : IQuery<PagedResult<TestResponse>> { }
public class TestCommand : ICommand<TestResponse> { }
public class TestCommandEmpty : ICommand { }
public class TestResponse
{
    public int Id { get; set; }
}
