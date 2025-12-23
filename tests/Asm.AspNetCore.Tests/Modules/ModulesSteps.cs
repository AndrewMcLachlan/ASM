using System.Reflection;
using Asm.AspNetCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Modules;

[Binding]
public class ModulesSteps
{
    private WebApplicationBuilder _builder = null!;
    private WebApplicationBuilder _result = null!;
    private WebApplication _app = null!;
    private Assembly _assembly = null!;

    [Given(@"I have a WebApplicationBuilder")]
    public void GivenIHaveAWebApplicationBuilder()
    {
        _builder = WebApplication.CreateBuilder();
        TestModule.ServicesAdded = false;
        TestModule.EndpointsMapped = false;
    }

    [Given(@"I have an assembly with an IModule implementation")]
    public void GivenIHaveAnAssemblyWithAnIModuleImplementation()
    {
        _assembly = typeof(TestModule).Assembly;
    }

    [Given(@"I have registered modules")]
    public void GivenIHaveRegisteredModules()
    {
        _result = _builder.RegisterModules(() => [new TestModule()]);
    }

    [When(@"I call RegisterModules with the assembly")]
    public void WhenICallRegisterModulesWithTheAssembly()
    {
        _result = _builder.RegisterModules(_assembly);
    }

    [When(@"I call RegisterModules with pattern '(.*)'")]
    public void WhenICallRegisterModulesWithPattern(string pattern)
    {
        _result = _builder.RegisterModules(pattern);
    }

    [When(@"I call RegisterModules with a module factory")]
    public void WhenICallRegisterModulesWithAModuleFactory()
    {
        _result = _builder.RegisterModules(() => [new TestModule()]);
    }

    [When(@"I build the application and map module endpoints")]
    public void WhenIBuildTheApplicationAndMapModuleEndpoints()
    {
        _app = _builder.Build();
        _app.MapModuleEndpoints();
    }

    [Then(@"the builder should be returned")]
    public void ThenTheBuilderShouldBeReturned()
    {
        Assert.NotNull(_result);
        Assert.Same(_builder, _result);
    }

    [Then(@"the module services should be registered")]
    public void ThenTheModuleServicesShouldBeRegistered()
    {
        Assert.True(TestModule.ServicesAdded);
    }

    [Then(@"the module endpoints should be mapped")]
    public void ThenTheModuleEndpointsShouldBeMapped()
    {
        Assert.True(TestModule.EndpointsMapped);
    }
}

/// <summary>
/// Test implementation of IModule for testing purposes.
/// </summary>
public class TestModule : IModule
{
    public static bool ServicesAdded { get; set; }
    public static bool EndpointsMapped { get; set; }

    public IServiceCollection AddServices(IServiceCollection services)
    {
        ServicesAdded = true;
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        EndpointsMapped = true;
        return endpoints;
    }
}
