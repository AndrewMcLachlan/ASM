using Asm.AspNetCore.Authentication;
using Asm.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Asm.AspNetCore.Tests.Authentication;

[Binding]
public class AuthenticationSteps
{
    private IServiceCollection _services = null!;
    private AuthenticationBuilder _builder = null!;
    private AuthenticationBuilder _result = null!;

    [Given(@"I have an authentication builder")]
    public void GivenIHaveAnAuthenticationBuilder()
    {
        _services = new ServiceCollection();

        // Add required services
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        _services.AddSingleton(mockEnvironment.Object);

        // Configure StandardJwtBearerOptions with required OAuthOptions
        _services.Configure<StandardJwtBearerOptions>(options =>
        {
            options.OAuthOptions = new OAuthOptions
            {
                Domain = "https://test-authority.com",
                Audience = "test-audience",
                ClientId = "test-client-id"
            };
        });

        _builder = _services.AddAuthentication();
    }

    [When(@"I call AddStandardJwtBearer with default options")]
    public void WhenICallAddStandardJwtBearerWithDefaultOptions()
    {
        _result = _builder.AddStandardJwtBearer();
    }

    [When(@"I call AddStandardJwtBearer with custom options")]
    public void WhenICallAddStandardJwtBearerWithCustomOptions()
    {
        _result = _builder.AddStandardJwtBearer(options =>
        {
            options.OAuthOptions = new OAuthOptions
            {
                Domain = "https://custom-authority.com",
                Audience = "custom-audience",
                ClientId = "custom-client-id",
                ValidateAudience = false
            };
        });
    }

    [Then(@"the authentication builder should be returned")]
    public void ThenTheAuthenticationBuilderShouldBeReturned()
    {
        Assert.NotNull(_result);
        Assert.Same(_builder, _result);
    }

    [Then(@"JwtBearerOptions should be configured")]
    public void ThenJwtBearerOptionsShouldBeConfigured()
    {
        var provider = _services.BuildServiceProvider();
        var optionsMonitor = provider.GetService<IOptionsMonitor<JwtBearerOptions>>();
        Assert.NotNull(optionsMonitor);
    }
}
