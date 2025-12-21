using System.Net;

namespace Asm.AspNetCore.Tests.Integration;

[Binding]
public class SharedIntegrationSteps(IntegrationTestContext context)
{
    [Given(@"I have a test web application")]
    public void GivenIHaveATestWebApplication()
    {
        context.Factory = new TestWebApplication();
        context.Client = context.Factory.CreateClient();
    }

    [When(@"I make a GET request to '(.*)'")]
    public async Task WhenIMakeAGetRequestTo(string path)
    {
        context.Response = await context.Client!.GetAsync(path);
        context.ResponseContent = await context.Response.Content.ReadAsStringAsync();
    }

    [Then(@"the response status code should be (\d+)")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, context.Response!.StatusCode);
    }

    [Then(@"the response content type should be '(.*)'")]
    public void ThenTheResponseContentTypeShouldBe(string contentType)
    {
        Assert.NotNull(context.Response!.Content.Headers.ContentType);
        Assert.StartsWith(contentType, context.Response.Content.Headers.ContentType.ToString());
    }
}
