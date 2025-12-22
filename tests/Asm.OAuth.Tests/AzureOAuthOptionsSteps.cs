using Asm.OAuth;

namespace Asm.OAuth.Tests;

[Binding]
public class AzureOAuthOptionsSteps
{
    private ScenarioContext _scenarioContext;

    public AzureOAuthOptionsSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"I have AzureOAuthOptions with domain '(.*)' and tenant id '(.*)'")]
    public void GivenIHaveAzureOAuthOptionsWithDomainAndTenantId(string domain, string tenantId)
    {
        var options = new AzureOAuthOptions
        {
            Domain = domain,
            TenantId = Guid.Parse(tenantId),
            Audience = "test-audience",
            ClientId = "test-client-id"
        };
        _scenarioContext["Options"] = options;
    }

    [Given(@"I have AzureOAuthOptions with domain '(.*)' and audience '(.*)'")]
    public void GivenIHaveAzureOAuthOptionsWithDomainAndAudience(string domain, string audience)
    {
        var options = new AzureOAuthOptions
        {
            Domain = domain,
            TenantId = Guid.NewGuid(),
            Audience = audience,
            ClientId = "test-client-id"
        };
        _scenarioContext["Options"] = options;
    }

    [Then(@"the Audience should be '(.*)'")]
    public void ThenTheAudienceShouldBe(string expectedAudience)
    {
        var options = _scenarioContext.Get<OAuthOptions>("Options");
        Assert.Equal(expectedAudience, options.Audience);
    }
}
