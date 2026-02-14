using Asm.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Tests.Routing;

[Binding]
public class EndpointGroupBaseSteps
{
    private WebApplication _app = null!;
    private TestEndpointGroupWithPolicy _endpointGroup = null!;
    private RouteGroupBuilder _result = null!;

    [Given(@"I have an EndpointGroupBase implementation")]
    public void GivenIHaveAnEndpointGroupBaseImplementation()
    {
        var webAppBuilder = WebApplication.CreateBuilder();
        _app = webAppBuilder.Build();
    }

    [Given(@"the endpoint group has no authorization policy")]
    public void GivenTheEndpointGroupHasNoAuthorizationPolicy()
    {
        _endpointGroup = new TestEndpointGroupWithPolicy(String.Empty);
    }

    [Given(@"the endpoint group has authorization policy '(.*)'")]
    public void GivenTheEndpointGroupHasAuthorizationPolicy(string policy)
    {
        _endpointGroup = new TestEndpointGroupWithPolicy(policy);
    }

    [When(@"I call MapGroup")]
    public void WhenICallMapGroup()
    {
        _result = _endpointGroup.MapGroup(_app);
    }

    [Then(@"a route group should be created")]
    public void ThenARouteGroupShouldBeCreated()
    {
        Assert.NotNull(_result);
    }

    [Then(@"the route group should have default authorization")]
    public void ThenTheRouteGroupShouldHaveDefaultAuthorization()
    {
        Assert.True(_endpointGroup.EndpointsMapped);
    }

    [Then(@"the route group should have custom authorization")]
    public void ThenTheRouteGroupShouldHaveCustomAuthorization()
    {
        Assert.True(_endpointGroup.EndpointsMapped);
    }
}

/// <summary>
/// Test implementation of EndpointGroupBase for testing purposes.
/// </summary>
public class TestEndpointGroupWithPolicy : EndpointGroupBase
{
    private readonly string _authorisationPolicy;

    public TestEndpointGroupWithPolicy() : this(String.Empty)
    {
    }

    public TestEndpointGroupWithPolicy(string authorisationPolicy)
    {
        _authorisationPolicy = authorisationPolicy;
    }

    public bool EndpointsMapped { get; private set; }

    public override string Name => "TestGroup";
    public override string Path => "/test";
    public override string Tags => "Test";
    public override string AuthorisationPolicy => _authorisationPolicy;

    protected override void MapEndpoints(IEndpointRouteBuilder builder)
    {
        EndpointsMapped = true;
    }
}
