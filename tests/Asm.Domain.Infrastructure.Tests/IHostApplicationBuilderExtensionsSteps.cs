using Asm.AspNetCore.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

[Binding]
public class IHostApplicationBuilderExtensionsSteps(ScenarioContext context)
{
    private IHostApplicationBuilder _builder = null!;
    private IHostApplicationBuilder _result = null!;
    private ServiceCollection _services = null!;

    [Given(@"I have a host application builder")]
    public void GivenIHaveAHostApplicationBuilder()
    {
        _services = new ServiceCollection();
        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(b => b.Services).Returns(_services);
        _builder = mockBuilder.Object;
    }

    [Given(@"I have a host application builder with an empty service collection")]
    public void GivenIHaveAHostApplicationBuilderWithAnEmptyServiceCollection()
    {
        _services = new ServiceCollection();
        var mockBuilder = new Mock<IHostApplicationBuilder>();
        mockBuilder.Setup(b => b.Services).Returns(_services);
        _builder = mockBuilder.Object;
    }

    [Given(@"I have a null host application builder")]
    public void GivenIHaveANullHostApplicationBuilder()
    {
        _builder = null!;
    }

    [When(@"I call AddEntityFrameworkOpenTelemetry")]
    public void WhenICallAddEntityFrameworkOpenTelemetry()
    {
        _result = _builder.AddEntityFrameworkOpenTelemetry();
    }

    [When(@"I call AddEntityFrameworkOpenTelemetry multiple times")]
    public void WhenICallAddEntityFrameworkOpenTelemetryMultipleTimes()
    {
        context.CatchException(() =>
        {
            _builder.AddEntityFrameworkOpenTelemetry();
            _builder.AddEntityFrameworkOpenTelemetry();
        });
    }

    [When(@"I call AddEntityFrameworkOpenTelemetry on the null builder")]
    public void WhenICallAddEntityFrameworkOpenTelemetryOnTheNullBuilder()
    {
        context.CatchException(() => _builder.AddEntityFrameworkOpenTelemetry());
    }

    [Then(@"the same builder instance is returned")]
    public void ThenTheSameBuilderInstanceIsReturned()
    {
        Assert.Same(_builder, _result);
    }

    [Then(@"OpenTelemetry services are registered")]
    public void ThenOpenTelemetryServicesAreRegistered()
    {
        Assert.NotEmpty(_services);
    }
}
