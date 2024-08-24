using System.Security.Claims;

namespace Asm.Tests;

[Binding]
public class ClaimsPrincipalExtensionsSteps(ScenarioContext context)
{
    private ClaimsPrincipal _claimsPrincipal;

    [Given(@"I have a ClaimsPrincipal with a claim of type ""(.*)"" and value ""(.*)""")]
    public void GivenIHaveAClaimsPrincipalWithAClaimOfTypeAndValue(string claimType, string claimValue)
    {
        var claims = new List<Claim> { new Claim(claimType, claimValue) };
        var identity = new ClaimsIdentity(claims);
        _claimsPrincipal = new ClaimsPrincipal(identity);
    }

    [Given(@"I have a ClaimsPrincipal with no claims of type ""(.*)""")]
    public void GivenIHaveAClaimsPrincipalWithNoClaimsOfType(string claimType)
    {
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        _claimsPrincipal = new ClaimsPrincipal(identity);
    }

    [When(@"I get the claim value as Guid")]
    public void WhenIGetTheClaimValueAsGuid()
    {
        context.AddResult(_claimsPrincipal.GetClaimValue<Guid>("claimType"));
    }

    [When(@"I get the claim value as int")]
    public void WhenIGetTheClaimValueAsInt()
    {
        context.AddResult(_claimsPrincipal.GetClaimValue<int>("claimType"));
    }

    [When(@"I get the claim value as string")]
    public void WhenIGetTheClaimValueAsString()
    {
        context.AddResult(_claimsPrincipal.GetClaimValue<string>("claimType"));
    }
}
