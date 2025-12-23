using Asm.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Asm.OAuth.Tests;

[Binding]
public class IServiceCollectionExtensionsSteps
{
    private ServiceCollection _services = null!;
    private OptionsBuilder<OAuthOptions> _oauthOptionsBuilder = null!;
    private OptionsBuilder<AzureOAuthOptions> _azureOptionsBuilder = null!;

    [Given(@"I have a ServiceCollection")]
    public void GivenIHaveAServiceCollection()
    {
        _services = new ServiceCollection();
    }

    [When(@"I call AddOAuthOptions with section path '(.*)'")]
    public void WhenICallAddOAuthOptionsWithSectionPath(string sectionPath)
    {
        _oauthOptionsBuilder = _services.AddOAuthOptions(sectionPath);
    }

    [When(@"I call AddAzureOAuthOptions with section path '(.*)'")]
    public void WhenICallAddAzureOAuthOptionsWithSectionPath(string sectionPath)
    {
        _azureOptionsBuilder = _services.AddAzureOAuthOptions(sectionPath);
    }

    [Then(@"an OptionsBuilder for OAuthOptions should be returned")]
    public void ThenAnOptionsBuilderForOAuthOptionsShouldBeReturned()
    {
        Assert.NotNull(_oauthOptionsBuilder);
    }

    [Then(@"an OptionsBuilder for AzureOAuthOptions should be returned")]
    public void ThenAnOptionsBuilderForAzureOAuthOptionsShouldBeReturned()
    {
        Assert.NotNull(_azureOptionsBuilder);
    }
}
