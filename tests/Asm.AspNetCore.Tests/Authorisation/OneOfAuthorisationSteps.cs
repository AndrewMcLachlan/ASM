using Asm.AspNetCore.Authorisation;
using Microsoft.AspNetCore.Authorization;

namespace Asm.AspNetCore.Tests.Authorisation;

[Binding]
public class OneOfAuthorisationSteps
{
    private OneOfAuthorisationRequirement _requirement = null!;

    [When(@"I create a OneOfAuthorisationRequirement with (\d+) sub-requirements")]
    public void WhenICreateAOneOfAuthorisationRequirementWithSubRequirements(int count)
    {
        var subRequirements = new IAuthorizationRequirement[count];
        for (int i = 0; i < count; i++)
        {
            subRequirements[i] = new TestRequirement();
        }
        _requirement = new OneOfAuthorisationRequirement(subRequirements);
    }

    [Then(@"the requirement should have (\d+) sub-requirements")]
    public void ThenTheRequirementShouldHaveSubRequirements(int expectedCount)
    {
        Assert.Equal(expectedCount, _requirement.Requirements.Count());
    }

    private class TestRequirement : IAuthorizationRequirement
    {
    }
}
