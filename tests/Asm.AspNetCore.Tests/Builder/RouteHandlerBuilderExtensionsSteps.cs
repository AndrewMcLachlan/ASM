#nullable enable
using System.Net;
using System.Net.Http.Json;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asm.AspNetCore.Tests.Builder;

[Binding]
public class RouteHandlerBuilderExtensionsSteps : IDisposable
{
    private IHost? _host;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private string? _responseContent;

    [Given(@"I have a test web application with validation")]
    public void GivenIHaveATestWebApplicationWithValidation()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddScoped<IValidator<TestRequest>, TestRequestValidator>();
                });
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapPost("/validate", (TestRequest request) => Results.Ok(request))
                            .WithValidation<TestRequest>();
                    });
                });
            });

        _host = builder.Build();
        _host.Start();
        _client = _host.GetTestClient();
    }

    [When(@"I make a POST request to '(.*)' with valid data")]
    public async Task WhenIMakeAPostRequestWithValidData(string path)
    {
        var request = new TestRequest { Name = "Valid Name", Email = "valid@example.com" };
        _response = await _client!.PostAsJsonAsync(path, request);
        _responseContent = await _response.Content.ReadAsStringAsync();
    }

    [Then(@"the validation response status code should be (\d+)")]
    public void ThenTheValidationResponseStatusCodeShouldBe(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response!.StatusCode);
    }

    [Then(@"the response should contain the request data")]
    public void ThenTheResponseShouldContainTheRequestData()
    {
        Assert.Contains("Valid Name", _responseContent);
        Assert.Contains("valid@example.com", _responseContent);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _host?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class TestRequest
{
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
}

public class TestRequestValidator : AbstractValidator<TestRequest>
{
    public TestRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
