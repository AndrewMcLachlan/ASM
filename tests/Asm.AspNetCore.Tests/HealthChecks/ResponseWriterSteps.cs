using System.Text.Json;
using Asm.AspNetCore.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Asm.AspNetCore.Tests.HealthChecks;

[Binding]
public class ResponseWriterSteps
{
    private HealthReport _healthReport;
    private DefaultHttpContext _httpContext;
    private MemoryStream _responseStream;
    private JsonDocument _responseJson;
    private readonly Dictionary<string, HealthReportEntry> _entries = [];
    private TimeSpan _duration = TimeSpan.FromMilliseconds(100);

    [Given(@"I have a health report with status '(.*)'")]
    public void GivenIHaveAHealthReportWithStatus(string status)
    {
        var healthStatus = Enum.Parse<HealthStatus>(status);
        _healthReport = new HealthReport(_entries, healthStatus, _duration);
    }

    [Given(@"I have a health report with status '(.*)' and duration (\d+) milliseconds")]
    public void GivenIHaveAHealthReportWithStatusAndDuration(string status, int milliseconds)
    {
        _duration = TimeSpan.FromMilliseconds(milliseconds);
        var healthStatus = Enum.Parse<HealthStatus>(status);
        _healthReport = new HealthReport(_entries, healthStatus, _duration);
    }

    [Given(@"the report has no entries")]
    public void GivenTheReportHasNoEntries()
    {
        // Entries dictionary is already empty
    }

    [Given(@"the report has an entry '(.*)' with status '(.*)' and description '(.*)'")]
    public void GivenTheReportHasAnEntryWithStatusAndDescription(string name, string status, string description)
    {
        var healthStatus = Enum.Parse<HealthStatus>(status);
        var entry = new HealthReportEntry(healthStatus, description, TimeSpan.FromMilliseconds(10), null, null);
        _entries[name] = entry;
        _healthReport = new HealthReport(_entries, _healthReport?.Status ?? HealthStatus.Healthy, _duration);
    }

    [Given(@"the report has an entry '(.*)' with status '(.*)' and data key '(.*)' value '(.*)'")]
    public void GivenTheReportHasAnEntryWithStatusAndData(string name, string status, string key, string value)
    {
        var healthStatus = Enum.Parse<HealthStatus>(status);
        var data = new Dictionary<string, object> { { key, value } };
        var entry = new HealthReportEntry(healthStatus, null, TimeSpan.FromMilliseconds(10), null, data.AsReadOnly());
        _entries[name] = entry;
        _healthReport = new HealthReport(_entries, _healthReport?.Status ?? HealthStatus.Healthy, _duration);
    }

    [When(@"I write the health response")]
    public async Task WhenIWriteTheHealthResponse()
    {
        _responseStream = new MemoryStream();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = _responseStream;

        await ResponseWriter.WriteResponse(_httpContext, _healthReport);

        _responseStream.Position = 0;
        _responseJson = await JsonDocument.ParseAsync(_responseStream);
    }

    [Then(@"the response content type should contain '(.*)'")]
    public void ThenTheResponseContentTypeShouldContain(string contentType)
    {
        Assert.Contains(contentType, _httpContext.Response.ContentType);
    }

    [Then(@"the response status should be '(.*)'")]
    public void ThenTheResponseStatusShouldBe(string status)
    {
        var actualStatus = _responseJson.RootElement.GetProperty("status").GetString();
        Assert.Equal(status, actualStatus);
    }

    [Then(@"the response should contain (\d+) checks")]
    public void ThenTheResponseShouldContainChecks(int count)
    {
        var checks = _responseJson.RootElement.GetProperty("checks");
        Assert.Equal(count, checks.GetArrayLength());
    }

    [Then(@"the response should contain a check named '(.*)' with status '(.*)'")]
    public void ThenTheResponseShouldContainACheckNamedWithStatus(string name, string status)
    {
        var checks = _responseJson.RootElement.GetProperty("checks");
        var found = false;

        foreach (var check in checks.EnumerateArray())
        {
            if (check.GetProperty("name").GetString() == name)
            {
                Assert.Equal(status, check.GetProperty("status").GetString());
                found = true;
                break;
            }
        }

        Assert.True(found, $"Check '{name}' not found in response");
    }

    [Then(@"the response should contain a check with data")]
    public void ThenTheResponseShouldContainACheckWithData()
    {
        var checks = _responseJson.RootElement.GetProperty("checks");
        var hasData = false;

        foreach (var check in checks.EnumerateArray())
        {
            if (check.TryGetProperty("data", out var data) && data.ValueKind != JsonValueKind.Null)
            {
                hasData = true;
                break;
            }
        }

        Assert.True(hasData, "No check with data found in response");
    }

    [Then(@"the response should include total duration")]
    public void ThenTheResponseShouldIncludeTotalDuration()
    {
        Assert.True(_responseJson.RootElement.TryGetProperty("totalDuration", out _));
    }
}
