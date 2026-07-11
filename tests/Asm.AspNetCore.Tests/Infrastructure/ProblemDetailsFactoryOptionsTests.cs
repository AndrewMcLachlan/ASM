using Asm.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcProblemDetailsFactory = Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory;

namespace Asm.AspNetCore.Tests.Infrastructure;

/// <summary>
/// Verifies that <see cref="ProblemDetailsFactory"/> handlers registered via options are isolated per
/// dependency-injection container and do not leak between containers.
/// </summary>
public class ProblemDetailsFactoryOptionsTests
{
    private sealed class TestException(string message) : Exception(message);

    [Fact]
    public void Handler_registered_via_options_is_used_by_the_factory()
    {
        var provider = BuildProvider(handlerTitle: "Handled", handlerStatus: StatusCodes.Status418ImATeapot);

        var factory = provider.GetRequiredService<MvcProblemDetailsFactory>();
        var context = CreateHttpContextWithException(new TestException("boom"));

        var problemDetails = factory.CreateProblemDetails(context);

        Assert.Equal("Handled", problemDetails.Title);
        Assert.Equal(StatusCodes.Status418ImATeapot, problemDetails.Status);
    }

    [Fact]
    public void Handlers_are_isolated_between_containers()
    {
        var providerOne = BuildProvider(handlerTitle: "One", handlerStatus: StatusCodes.Status418ImATeapot);
        var providerTwo = BuildProvider(handlerTitle: "Two", handlerStatus: StatusCodes.Status422UnprocessableEntity);

        var factoryOne = providerOne.GetRequiredService<MvcProblemDetailsFactory>();
        var factoryTwo = providerTwo.GetRequiredService<MvcProblemDetailsFactory>();

        var problemOne = factoryOne.CreateProblemDetails(CreateHttpContextWithException(new TestException("boom")));
        var problemTwo = factoryTwo.CreateProblemDetails(CreateHttpContextWithException(new TestException("boom")));

        Assert.Equal("One", problemOne.Title);
        Assert.Equal(StatusCodes.Status418ImATeapot, problemOne.Status);

        Assert.Equal("Two", problemTwo.Title);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problemTwo.Status);
    }

    [Fact]
    public void A_container_without_a_handler_does_not_see_another_containers_handler()
    {
        // Container one registers a handler for TestException...
        _ = BuildProvider(handlerTitle: "One", handlerStatus: StatusCodes.Status418ImATeapot);

        // ...container two registers none, so TestException must fall through to the default 500 mapping.
        var providerTwo = BuildProviderWithoutHandler();

        var factoryTwo = providerTwo.GetRequiredService<MvcProblemDetailsFactory>();
        var problemTwo = factoryTwo.CreateProblemDetails(CreateHttpContextWithException(new TestException("boom")));

        Assert.NotEqual("One", problemTwo.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemTwo.Status);
    }

    private static ServiceProvider BuildProvider(string handlerTitle, int handlerStatus)
    {
        var services = new ServiceCollection();
        services.AddSingleton(CreateHostEnvironment());
        services.AddProblemDetailsFactory();
        services.AddProblemDetailsHandler<TestException>((_, _) => new ProblemDetails
        {
            Title = handlerTitle,
            Status = handlerStatus,
        });
        return services.BuildServiceProvider();
    }

    private static ServiceProvider BuildProviderWithoutHandler()
    {
        var services = new ServiceCollection();
        services.AddSingleton(CreateHostEnvironment());
        services.AddProblemDetailsFactory();
        return services.BuildServiceProvider();
    }

    private static IHostEnvironment CreateHostEnvironment()
    {
        var mock = new Mock<IHostEnvironment>();
        mock.Setup(x => x.EnvironmentName).Returns("Production");
        return mock.Object;
    }

    private static DefaultHttpContext CreateHttpContextWithException(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var feature = new Mock<IExceptionHandlerFeature>();
        feature.Setup(x => x.Error).Returns(exception);
        httpContext.Features.Set(feature.Object);
        return httpContext;
    }
}
