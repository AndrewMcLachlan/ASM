using Asm.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Asm.AspNetCore.Tests.Extensions;

[Binding]
public class IHostApplicationBuilderExtensionsSteps
{
    private WebApplicationBuilder _builder = null!;
    private IHostApplicationBuilder _result = null!;

    [Given(@"I have a WebApplicationBuilder for OpenTelemetry")]
    public void GivenIHaveAWebApplicationBuilderForOpenTelemetry()
    {
        _builder = WebApplication.CreateBuilder();
        _builder.Services.AddHttpContextAccessor();
    }

    [When(@"I call AddStandardOpenTelemetry")]
    public void WhenICallAddStandardOpenTelemetry()
    {
        _result = _builder.AddStandardOpenTelemetry();
    }

    [Then(@"the host application builder should be returned")]
    public void ThenTheHostApplicationBuilderShouldBeReturned()
    {
        Assert.Same(_builder, _result);
    }

    [Then(@"OpenTelemetry services should be registered")]
    public void ThenOpenTelemetryServicesShouldBeRegistered()
    {
        var app = _builder.Build();

        // Verify OpenTelemetry services are registered
        var meterProvider = app.Services.GetService<MeterProvider>();
        var tracerProvider = app.Services.GetService<TracerProvider>();

        Assert.NotNull(meterProvider);
        Assert.NotNull(tracerProvider);
    }
}
