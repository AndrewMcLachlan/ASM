using System.Text.Json;

namespace Asm.AspNetCore.Tests.Integration;

[Binding]
public class ExceptionHandlerSteps(IntegrationTestContext context)
{
    [Then(@"the response should contain ProblemDetails")]
    public void ThenTheResponseShouldContainProblemDetails()
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;

        Assert.True(root.TryGetProperty("status", out _), "ProblemDetails should have 'status' property");
        Assert.True(root.TryGetProperty("title", out _), "ProblemDetails should have 'title' property");
    }

    [Then(@"the response should contain ProblemDetails with type")]
    public void ThenTheResponseShouldContainProblemDetailsWithType()
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;

        Assert.True(root.TryGetProperty("type", out var typeProperty), "ProblemDetails should have 'type' property");
        Assert.False(String.IsNullOrEmpty(typeProperty.GetString()), "ProblemDetails type should not be empty");
    }

    [Then(@"the response should contain ProblemDetails with detail containing '(.*)'")]
    public void ThenTheResponseShouldContainProblemDetailsWithDetailContaining(string expectedDetail)
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;

        Assert.True(root.TryGetProperty("detail", out var detailProperty), "ProblemDetails should have 'detail' property");
        var detail = detailProperty.GetString();
        Assert.NotNull(detail);
        Assert.Contains(expectedDetail, detail);
    }
}
