using System.Reflection;
using Asm.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Tests.Routing;

[Binding]
public class RouteGroupBuilderExtensionsSteps
{
    private WebApplication _app = null!;
    private RouteGroupBuilder _builder = null!;
    private RouteGroupBuilder _result = null!;
    private Assembly _assembly = null!;
    private bool _endpointGroupMapped;

    [Given(@"I have a RouteGroupBuilder")]
    public void GivenIHaveARouteGroupBuilder()
    {
        var webAppBuilder = WebApplication.CreateBuilder();
        _app = webAppBuilder.Build();
        _builder = _app.MapGroup("/api");
    }

    [Given(@"I have an assembly with an IEndpointGroup implementation")]
    public void GivenIHaveAnAssemblyWithAnIEndpointGroupImplementation()
    {
        // Use the current test assembly which contains TestEndpointGroup
        _assembly = typeof(TestEndpointGroup).Assembly;
        _endpointGroupMapped = false;
        TestEndpointGroup.WasMapped = false;
    }

    [Given(@"I have an assembly with no IEndpointGroup implementations")]
    public void GivenIHaveAnAssemblyWithNoIEndpointGroupImplementations()
    {
        // Use mscorlib which has no IEndpointGroup implementations
        _assembly = typeof(string).Assembly;
    }

    [When(@"I call MapGroups with the assembly")]
    public void WhenICallMapGroupsWithTheAssembly()
    {
        _result = _builder.MapGroups(_assembly);
        _endpointGroupMapped = TestEndpointGroup.WasMapped;
    }

    [Then(@"the endpoint group should be mapped")]
    public void ThenTheEndpointGroupShouldBeMapped()
    {
        Assert.True(_endpointGroupMapped);
    }

    [Then(@"the route group builder should be returned")]
    public void ThenTheRouteGroupBuilderShouldBeReturned()
    {
        Assert.NotNull(_result);
        Assert.Same(_builder, _result);
    }
}

/// <summary>
/// Test implementation of IEndpointGroup for testing purposes.
/// </summary>
public class TestEndpointGroup : IEndpointGroup
{
    public static bool WasMapped { get; set; }

    public RouteGroupBuilder MapGroup(IEndpointRouteBuilder builder)
    {
        WasMapped = true;
        return builder.MapGroup("/test");
    }
}
