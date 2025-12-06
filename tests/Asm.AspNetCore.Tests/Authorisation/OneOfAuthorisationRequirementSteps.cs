using Asm.AspNetCore.Authorisation;
using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Tests.Authorisation;

[Binding]
public class OneOfAuthorisationRequirementSteps
{
    private IAuthorizationRequirement[] _requirements;
    private OneOfAuthorisationRequirement _oneOfRequirement;

    [Given(@"I have authorization requirements")]
    public void GivenIHaveAuthorizationRequirements(Table table)
    {
        _requirements = table.Rows.Select(_ => new TestRequirement() as IAuthorizationRequirement).ToArray();
    }

    [Given(@"I have no authorization requirements")]
    public void GivenIHaveNoAuthorizationRequirements()
    {
        _requirements = [];
    }

    [When(@"I create a OneOfAuthorisationRequirement")]
    public void WhenICreateAOneOfAuthorisationRequirement()
    {
        _oneOfRequirement = new OneOfAuthorisationRequirement(_requirements);
    }

    [Then(@"the requirement should contain (.*) requirements")]
    public void ThenTheRequirementShouldContainRequirements(int count)
    {
        Assert.Equal(count, _oneOfRequirement.Requirements.Count());
    }

    private class TestRequirement : IAuthorizationRequirement
    {
    }
}
