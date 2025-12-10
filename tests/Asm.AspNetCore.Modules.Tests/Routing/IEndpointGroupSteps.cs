using Asm.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Modules.Tests.Routing;

[Binding]
[Scope(Feature = "IEndpointGroup")]
public class IEndpointGroupSteps
{
    private class TestEndpointGroup : IEndpointGroup
    {
        public bool MapGroupCalled { get; set; }

        public RouteGroupBuilder MapGroup(IEndpointRouteBuilder builder)
        {
            MapGroupCalled = true;
            return builder.MapGroup("/api");
        }
    }

    private TestEndpointGroup _testEndpointGroup;
    private IEndpointRouteBuilder _endpointRouteBuilder;
    private RouteGroupBuilder _routeGroupBuilder;

    [Given(@"I have a test endpoint group")]
    public void GivenIHaveATestEndpointGroup()
    {
        _testEndpointGroup = new TestEndpointGroup();
    }

    [Given(@"I have an endpoint route builder")]
    public void GivenIHaveAnEndpointRouteBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        _endpointRouteBuilder = builder.Build();
    }

    [When(@"I map the endpoint group")]
    public void WhenIMapTheEndpointGroup()
    {
        _routeGroupBuilder = _testEndpointGroup.MapGroup(_endpointRouteBuilder);
    }

    [Then(@"the route group builder should be returned")]
    public void ThenTheRouteGroupBuilderShouldBeReturned()
    {
        Assert.NotNull(_routeGroupBuilder);
        Assert.IsAssignableFrom<RouteGroupBuilder>(_routeGroupBuilder);
    }

    [Then(@"the endpoint group should confirm it was mapped")]
    public void ThenTheEndpointGroupShouldConfirmItWasMapped()
    {
        Assert.True(_testEndpointGroup.MapGroupCalled);
    }

    [Then(@"the endpoint group should implement IEndpointGroup")]
    public void ThenTheEndpointGroupShouldImplementIEndpointGroup()
    {
        Assert.IsAssignableFrom<IEndpointGroup>(_testEndpointGroup);
    }
}
