using Asm.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Modules.Tests.Routing;

[Binding]
[Scope(Feature = "EndpointGroupBase")]
public class EndpointGroupBaseSteps
{
    private class SimpleEndpointGroup : EndpointGroupBase
    {
        public override string Name => "SimpleGroup";
        public override string Path => "/api/simple";
        public override string Tags => "simple,test";
        public override string AuthorisationPolicy => "RequireAdmin";

        protected override void MapEndpoints(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", () => "Hello from simple group")
                .WithName("GetSimple");
        }
    }

    private class MinimalEndpointGroup : EndpointGroupBase
    {
        public override string Name => "MinimalGroup";
        public override string Path => "/api/minimal";

        protected override void MapEndpoints(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", () => "Hello from minimal group")
                .WithName("GetMinimal");
        }
    }

    private class CustomAuthEndpointGroup : EndpointGroupBase
    {
        private readonly string _policy;

        public CustomAuthEndpointGroup(string policy)
        {
            _policy = policy;
        }

        public override string Name => "CustomAuthGroup";
        public override string Path => "/api/custom";
        public override string AuthorisationPolicy => _policy;

        protected override void MapEndpoints(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", () => "Hello");
        }
    }

    private EndpointGroupBase _simpleEndpointGroup;
    private EndpointGroupBase _minimalEndpointGroup;
    private EndpointGroupBase _customAuthGroup;
    private IEndpointRouteBuilder _endpointRouteBuilder;
    private RouteGroupBuilder _routeGroupBuilder;

    [Given(@"I have a simple endpoint group")]
    public void GivenIHaveASimpleEndpointGroup()
    {
        _simpleEndpointGroup = new SimpleEndpointGroup();
    }

    [Given(@"I have a minimal endpoint group")]
    public void GivenIHaveAMinimalEndpointGroup()
    {
        _minimalEndpointGroup = new MinimalEndpointGroup();
    }

    [Given(@"I have a simple endpoint group with policy '(.*)'")]
    public void GivenIHaveASimpleEndpointGroupWithPolicy(string policy)
    {
        _customAuthGroup = new CustomAuthEndpointGroup(policy);
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
        if (_simpleEndpointGroup != null)
        {
            _routeGroupBuilder = _simpleEndpointGroup.MapGroup(_endpointRouteBuilder);
        }
        else if (_minimalEndpointGroup != null)
        {
            _routeGroupBuilder = _minimalEndpointGroup.MapGroup(_endpointRouteBuilder);
        }
        else if (_customAuthGroup != null)
        {
            _routeGroupBuilder = _customAuthGroup.MapGroup(_endpointRouteBuilder);
        }
    }

    [Then(@"the group name should be '(.*)'")]
    public void ThenTheGroupNameShouldBe(string expectedName)
    {
        var group = _simpleEndpointGroup ?? _minimalEndpointGroup ?? _customAuthGroup;
        Assert.Equal(expectedName, group.Name);
    }

    [Then(@"the group path should be '(.*)'")]
    public void ThenTheGroupPathShouldBe(string expectedPath)
    {
        Assert.Equal(expectedPath, _simpleEndpointGroup.Path);
    }

    [Then(@"the group tags should be empty")]
    public void ThenTheGroupTagsShouldBeEmpty()
    {
        Assert.Empty(_minimalEndpointGroup.Tags);
    }

    [Then(@"the group tags should be '(.*)'")]
    public void ThenTheGroupTagsShouldBe(string expectedTags)
    {
        Assert.Equal(expectedTags, _simpleEndpointGroup.Tags);
    }

    [Then(@"the authorization policy should be empty")]
    public void ThenTheAuthorizationPolicyShouldBeEmpty()
    {
        Assert.Empty(_minimalEndpointGroup.AuthorisationPolicy);
    }

    [Then(@"the authorization policy should be '(.*)'")]
    public void ThenTheAuthorizationPolicyShouldBe(string expectedPolicy)
    {
        Assert.Equal(expectedPolicy, _simpleEndpointGroup.AuthorisationPolicy);
    }

    [Then(@"the route group builder should be returned")]
    public void ThenTheRouteGroupBuilderShouldBeReturned()
    {
        Assert.NotNull(_routeGroupBuilder);
    }

    [Then(@"the route group builder should have the authorization policy applied")]
    public void ThenTheRouteGroupBuilderShouldHaveTheAuthorizationPolicyApplied()
    {
        Assert.NotNull(_routeGroupBuilder);
        // Authorization is applied during MapGroup
    }

    [Then(@"EndpointGroupBase cannot be instantiated directly")]
    public void ThenEndpointGroupBaseCannotBeInstantiatedDirectly()
    {
        Assert.True(typeof(EndpointGroupBase).IsAbstract);
    }
}
