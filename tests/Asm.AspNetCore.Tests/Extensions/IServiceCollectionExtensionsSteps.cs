using Asm.AspNetCore.Security;
using Asm.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Extensions;

[Binding]
public class IServiceCollectionExtensionsSteps
{
    private IServiceCollection _services = null!;
    private IServiceCollection _returnedServices = null!;

    [Given(@"I have an empty service collection")]
    public void GivenIHaveAnEmptyServiceCollection()
    {
        _services = new ServiceCollection();
    }

    [Given(@"I have a service collection with host environment")]
    public void GivenIHaveAServiceCollectionWithHostEnvironment()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<IHostEnvironment>(new TestHostEnvironment());
    }

    [When(@"I call AddProblemDetailsFactory")]
    public void WhenICallAddProblemDetailsFactory()
    {
        _returnedServices = _services.AddProblemDetailsFactory();
    }

    [When(@"I call AddPrincipalProvider")]
    public void WhenICallAddPrincipalProvider()
    {
        _returnedServices = _services.AddPrincipalProvider();
    }

    [Then(@"ProblemDetailsFactory should be registered")]
    public void ThenProblemDetailsFactoryShouldBeRegistered()
    {
        // Check that at least one service is registered (basic sanity check)
        Assert.True(_services.Count > 0, $"Expected services to be registered, but found {_services.Count}");

        // List all registered services for debugging
        var serviceTypes = _services.Select(s => s.ServiceType.FullName).ToList();

        // Check service descriptor is registered for the abstract type
        var descriptor = _services.FirstOrDefault(d =>
            d.ServiceType == typeof(Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(Asm.AspNetCore.ProblemDetailsFactory), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);

        // Verify it can be resolved
        var serviceProvider = _services.BuildServiceProvider();
        var factory = serviceProvider.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory>();
        Assert.NotNull(factory);
        Assert.IsType<Asm.AspNetCore.ProblemDetailsFactory>(factory);
    }

    [Then(@"IPrincipalProvider should be registered as HttpContextPrincipalProvider")]
    public void ThenIPrincipalProviderShouldBeRegisteredAsHttpContextPrincipalProvider()
    {
        var descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IPrincipalProvider));
        Assert.NotNull(descriptor);
        Assert.Equal(typeof(HttpContextPrincipalProvider), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Then(@"IHttpContextAccessor should be registered")]
    public void ThenIHttpContextAccessorShouldBeRegistered()
    {
        var descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
        Assert.NotNull(descriptor);
    }

    [Then(@"the returned service collection should be the same instance")]
    public void ThenTheReturnedServiceCollectionShouldBeTheSameInstance()
    {
        Assert.Same(_services, _returnedServices);
    }

    private class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = ".";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
