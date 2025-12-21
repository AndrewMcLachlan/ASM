using System.Text.Json;

namespace Asm.AspNetCore.Tests.Integration;

[Binding]
public class HealthChecksSteps(IntegrationTestContext context)
{
    [Then(@"the health check response should have status '(.*)'")]
    public void ThenTheHealthCheckResponseShouldHaveStatus(string status)
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;
        Assert.True(root.TryGetProperty("status", out var statusProperty), "Response should have 'status' property");
        Assert.Equal(status, statusProperty.GetString());
    }

    [Then(@"the health check response should have a checks array")]
    public void ThenTheHealthCheckResponseShouldHaveAChecksArray()
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;
        Assert.True(root.TryGetProperty("checks", out var checksProperty), "Response should have 'checks' property");
        Assert.Equal(JsonValueKind.Array, checksProperty.ValueKind);
    }

    [Then(@"the health check response should contain check '(.*)'")]
    public void ThenTheHealthCheckResponseShouldContainCheck(string checkName)
    {
        var json = JsonDocument.Parse(context.ResponseContent!);
        var root = json.RootElement;
        var checks = root.GetProperty("checks");

        var found = false;
        foreach (var check in checks.EnumerateArray())
        {
            if (check.TryGetProperty("name", out var nameProperty) && nameProperty.GetString() == checkName)
            {
                found = true;
                break;
            }
        }

        Assert.True(found, $"Health check '{checkName}' not found in response");
    }
}
