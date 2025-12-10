using Asm.AspNetCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules.Tests.Routing;

[Binding]
[Scope(Feature = "IModule")]
public class IModuleSteps
{
    private class TestModule : IModule
    {
        public bool AddServicesCalled { get; set; }
        public bool MapEndpointsCalled { get; set; }

        public IServiceCollection AddServices(IServiceCollection services)
        {
            AddServicesCalled = true;
            services.AddScoped<IModuleTestService>();
            return services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            MapEndpointsCalled = true;
            endpoints.MapGet("/test", () => "test").WithName("TestEndpoint");
            return endpoints;
        }
    }

    private interface IModuleTestService;

    private TestModule _testModule;
    private IServiceCollection _serviceCollection;
    private IEndpointRouteBuilder _endpointRouteBuilder;

    [Given(@"I have a test module")]
    public void GivenIHaveATestModule()
    {
        _testModule = new TestModule();
    }

    [Given(@"I have a service collection")]
    public void GivenIHaveAServiceCollection()
    {
        _serviceCollection = new ServiceCollection();
    }

    [Given(@"I have an endpoint route builder")]
    public void GivenIHaveAnEndpointRouteBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        _endpointRouteBuilder = builder.Build();
    }

    [When(@"I add services from the module")]
    public void WhenIAddServicesFromTheModule()
    {
        _serviceCollection = _testModule.AddServices(_serviceCollection);
    }

    [When(@"I map endpoints from the module")]
    public void WhenIMapEndpointsFromTheModule()
    {
        _endpointRouteBuilder = _testModule.MapEndpoints(_endpointRouteBuilder);
    }

    [Then(@"the services should be added to the collection")]
    public void ThenTheServicesShouldBeAddedToTheCollection()
    {
        Assert.Contains(_serviceCollection, sd => sd.ServiceType == typeof(IModuleTestService));
    }

    [Then(@"the module should confirm services were added")]
    public void ThenTheModuleShouldConfirmServicesWereAdded()
    {
        Assert.True(_testModule.AddServicesCalled);
    }

    [Then(@"the endpoints should be mapped")]
    public void ThenTheEndpointsShouldBeMapped()
    {
        Assert.NotNull(_endpointRouteBuilder);
    }

    [Then(@"the module should confirm endpoints were mapped")]
    public void ThenTheModuleShouldConfirmEndpointsWereMapped()
    {
        Assert.True(_testModule.MapEndpointsCalled);
    }

    [Then(@"the service collection should be returned")]
    public void ThenTheServiceCollectionShouldBeReturned()
    {
        Assert.NotNull(_serviceCollection);
    }

    [Then(@"the endpoint route builder should be returned")]
    public void ThenTheEndpointRouteBuilderShouldBeReturned()
    {
        Assert.NotNull(_endpointRouteBuilder);
    }
}
