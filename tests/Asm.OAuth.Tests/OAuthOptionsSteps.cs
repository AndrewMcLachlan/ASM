using Asm.OAuth;

namespace Asm.OAuth.Tests;

[Binding]
public class OAuthOptionsSteps
{
    private readonly ScenarioContext _scenarioContext;

    public OAuthOptionsSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"I have OAuthOptions with domain '(.*)'")]
    public void GivenIHaveOAuthOptionsWithDomain(string domain)
    {
        var options = new OAuthOptions
        {
            Domain = domain,
            Audience = "test-audience",
            ClientId = "test-client-id"
        };
        _scenarioContext["Options"] = options;
    }

    [Given(@"I have OAuthOptions with default ValidateAudience")]
    public void GivenIHaveOAuthOptionsWithDefaultValidateAudience()
    {
        var options = new OAuthOptions
        {
            Domain = "https://auth.example.com",
            Audience = "test-audience",
            ClientId = "test-client-id"
        };
        _scenarioContext["Options"] = options;
    }

    [Given(@"I have OAuthOptions with ValidateAudience set to true")]
    public void GivenIHaveOAuthOptionsWithValidateAudienceSetToTrue()
    {
        var options = new OAuthOptions
        {
            Domain = "https://auth.example.com",
            Audience = "test-audience",
            ClientId = "test-client-id",
            ValidateAudience = true
        };
        _scenarioContext["Options"] = options;
    }

    [Then(@"the Authority should be '(.*)'")]
    public void ThenTheAuthorityShouldBe(string expectedAuthority)
    {
        var options = _scenarioContext.Get<OAuthOptions>("Options");
        Assert.Equal(expectedAuthority, options.Authority);
    }

    [Then(@"ValidateAudience should be false")]
    public void ThenValidateAudienceShouldBeFalse()
    {
        var options = _scenarioContext.Get<OAuthOptions>("Options");
        Assert.False(options.ValidateAudience);
    }

    [Then(@"ValidateAudience should be true")]
    public void ThenValidateAudienceShouldBeTrue()
    {
        var options = _scenarioContext.Get<OAuthOptions>("Options");
        Assert.True(options.ValidateAudience);
    }
}
