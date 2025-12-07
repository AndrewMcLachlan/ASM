using Asm.AspNetCore.Authorisation;

namespace Asm.AspNetCore.Tests.Authorisation;

[Binding]
public class RouteParamAuthorisationRequirementSteps
{
    private RouteParamAuthorisationRequirement _requirement;

    [When(@"I create a RouteParamAuthorisationRequirement with name '(.*)'")]
    public void WhenICreateARouteParamAuthorisationRequirementWithName(string name)
    {
        _requirement = new RouteParamAuthorisationRequirement(name);
    }

    [Then(@"the requirement name should be '(.*)'")]
    public void ThenTheRequirementNameShouldBe(string expectedName)
    {
        Assert.Equal(expectedName, _requirement.Name);
    }
}
