#nullable enable

namespace Asm.AspNetCore.Tests.Integration;

[Binding]
public class SecurityHeadersSteps(IntegrationTestContext context)
{
    [Then(@"the response should have header '(.*)' with value '(.*)'")]
    public void ThenTheResponseShouldHaveHeaderWithValue(string headerName, string expectedValue)
    {
        var response = context.Response!;
        Assert.True(response.Headers.Contains(headerName) || response.Content.Headers.Contains(headerName),
            $"Response does not contain header '{headerName}'");

        string? actualValue = null;
        if (response.Headers.TryGetValues(headerName, out var headerValues))
        {
            actualValue = headerValues.FirstOrDefault();
        }
        else if (response.Content.Headers.TryGetValues(headerName, out var contentHeaderValues))
        {
            actualValue = contentHeaderValues.FirstOrDefault();
        }

        Assert.Equal(expectedValue, actualValue);
    }
}
