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

    [When(@"I call AddAsmExceptionHandler")]
    public void WhenICallAddAsmExceptionHandler()
    {
        _returnedServices = _services.AddAsmExceptionHandler();
    }

    [When(@"I call AddPrincipalProvider")]
    public void WhenICallAddPrincipalProvider()
    {
        _returnedServices = _services.AddPrincipalProvider();
    }

    [Then(@"the Asm exception handler should be registered")]
    public void ThenTheAsmExceptionHandlerShouldBeRegistered()
    {
        // The AsmExceptionHandler is registered as an IExceptionHandler...
        Assert.Contains(_services, d =>
            d.ServiceType == typeof(Microsoft.AspNetCore.Diagnostics.IExceptionHandler) &&
            d.ImplementationType == typeof(Asm.AspNetCore.AsmExceptionHandler));

        // ...alongside the framework problem-details service that writes the responses.
        Assert.Contains(_services, d => d.ServiceType == typeof(Microsoft.AspNetCore.Http.IProblemDetailsService));

        var serviceProvider = _services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<Microsoft.AspNetCore.Http.IProblemDetailsService>());
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
