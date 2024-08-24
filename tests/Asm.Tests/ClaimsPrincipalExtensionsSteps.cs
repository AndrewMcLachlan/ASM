using System.Security.Claims;

namespace Asm.Testing;

[Binding]
public class ClaimsPrincipalExtensionsSteps(ScenarioContext context)
{
    private ClaimsPrincipal _claimsPrincipal;
    private object _result;

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

    [Then(@"the result should be ""(.*)""")]
    public void ThenTheResultShouldBe(string expected)
    {
        Assert.Equal(expected, _result?.ToString());
    }

    [Then(@"the result should be (.*)")]
    public void ThenTheResultShouldBe(int expected)
    {
        Assert.Equal(expected, _result);
    }

    [Then(@"the result should be null")]
    public void ThenTheResultShouldBeNull()
    {
        Assert.Null(_result);
    }
}
