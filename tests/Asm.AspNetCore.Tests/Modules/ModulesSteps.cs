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
    private IServiceCollection _services = null!;
    private IServiceProvider _provider = null!;

    [Given(@"I have a WebApplicationBuilder")]
    public void GivenIHaveAWebApplicationBuilder()
    {
        _builder = WebApplication.CreateBuilder();
        ResetModuleFlags();
    }

    [Given(@"I have a module service collection")]
    public void GivenIHaveAModuleServiceCollection()
    {
        _services = new ServiceCollection();
        ResetModuleFlags();
    }

    [Given(@"I have registered two modules")]
    public void GivenIHaveRegisteredTwoModules()
    {
        _result = _builder.RegisterModules(() => [new TestModule(), new SecondTestModule()]);
    }

    [When(@"I add a TestModule to the collection")]
    public void WhenIAddATestModuleToTheCollection()
    {
        _services.AddModule<TestModule>();
    }

    [When(@"I add a SecondTestModule to the collection")]
    public void WhenIAddASecondTestModuleToTheCollection()
    {
        _services.AddModule<SecondTestModule>();
    }

    [Then(@"the service provider should resolve (.*) module\(s\)")]
    public void ThenTheServiceProviderShouldResolveModules(int count)
    {
        _provider = _services.BuildServiceProvider();
        Assert.Equal(count, _provider.GetServices<IModule>().Count());
    }

    [Then(@"the module services should be registered in the collection")]
    public void ThenTheModuleServicesShouldBeRegisteredInTheCollection()
    {
        Assert.True(TestModule.ServicesAdded);
    }

    [Then(@"both module endpoints should be mapped")]
    public void ThenBothModuleEndpointsShouldBeMapped()
    {
        Assert.True(TestModule.EndpointsMapped);
        Assert.True(SecondTestModule.EndpointsMapped);
    }

    private static void ResetModuleFlags()
    {
        TestModule.ServicesAdded = false;
        TestModule.EndpointsMapped = false;
        SecondTestModule.ServicesAdded = false;
        SecondTestModule.EndpointsMapped = false;
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

/// <summary>
/// A second test implementation of IModule, used to verify additive registration and DI resolution.
/// </summary>
public class SecondTestModule : IModule
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
