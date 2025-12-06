using Asm.AspNetCore.Authentication;
using Asm.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Asm.AspNetCore.Tests.Authentication;

[Binding]
public class StandardJwtBearerOptionsSteps
{
    private OAuthOptions _oAuthOptions;
    private JwtBearerEvents _customEvents;
    private StandardJwtBearerOptions _standardJwtBearerOptions;

    [Given(@"I have OAuthOptions with domain '(.*)' and audience '(.*)' and clientId '(.*)'")]
    public void GivenIHaveOAuthOptionsWithDomainAndAudienceAndClientId(string domain, string audience, string clientId)
    {
        _oAuthOptions = new OAuthOptions
        {
            Domain = domain,
            Audience = audience,
            ClientId = clientId
        };
    }

    [Given(@"I have custom JwtBearerEvents")]
    public void GivenIHaveCustomJwtBearerEvents()
    {
        _customEvents = new JwtBearerEvents
        {
            OnAuthenticationFailed = context => Task.CompletedTask
        };
    }

    [When(@"I create StandardJwtBearerOptions with the OAuthOptions")]
    public void WhenICreateStandardJwtBearerOptionsWithTheOAuthOptions()
    {
        _standardJwtBearerOptions = new StandardJwtBearerOptions
        {
            OAuthOptions = _oAuthOptions
        };
    }

    [When(@"I create StandardJwtBearerOptions with the OAuthOptions and custom events")]
    public void WhenICreateStandardJwtBearerOptionsWithTheOAuthOptionsAndCustomEvents()
    {
        _standardJwtBearerOptions = new StandardJwtBearerOptions
        {
            OAuthOptions = _oAuthOptions,
            Events = _customEvents
        };
    }

    [Then(@"the StandardJwtBearerOptions should have the correct OAuthOptions")]
    public void ThenTheStandardJwtBearerOptionsShouldHaveTheCorrectOAuthOptions()
    {
        Assert.NotNull(_standardJwtBearerOptions.OAuthOptions);
        Assert.Equal(_oAuthOptions.Domain, _standardJwtBearerOptions.OAuthOptions.Domain);
        Assert.Equal(_oAuthOptions.Audience, _standardJwtBearerOptions.OAuthOptions.Audience);
        Assert.Equal(_oAuthOptions.ClientId, _standardJwtBearerOptions.OAuthOptions.ClientId);
    }

    [Then(@"the StandardJwtBearerOptions should have default JwtBearerEvents")]
    public void ThenTheStandardJwtBearerOptionsShouldHaveDefaultJwtBearerEvents()
    {
        Assert.NotNull(_standardJwtBearerOptions.Events);
    }

    [Then(@"the StandardJwtBearerOptions should have the custom JwtBearerEvents")]
    public void ThenTheStandardJwtBearerOptionsShouldHaveTheCustomJwtBearerEvents()
    {
        Assert.Same(_customEvents, _standardJwtBearerOptions.Events);
    }
}
