using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Asm.AspNetCore.Postie.Tests;

/// <summary>
/// Proves <c>MapPagedQuery</c> over real TestServer round-trips against a mocked
/// <see cref="IEndpointDispatcher"/> — no mediator package is referenced anywhere, proving the
/// endpoint mapping is mediator-agnostic.
/// </summary>
/// <param name="context">The current scenario context.</param>
[Binding]
public class MapPagedQuerySteps(ScenarioContext context) : IDisposable
{
    private readonly Mock<IEndpointDispatcher> _dispatcherMock = new();
    private WebApplication? _app;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private IReadOnlyList<string> _expectedItems = [];

    private record TestPagedQuery(string? Term = null);

    // ── Given ───────────────────────────────────────────────────────────────

    [Given(@"a paged endpoint mapped with the default method")]
    public async Task GivenAPagedEndpointMappedWithTheDefaultMethod() => await MapPagedEndpointAsync(null);

    [Given(@"a paged endpoint mapped with method (.*)")]
    public async Task GivenAPagedEndpointMappedWithMethod(string method) => await MapPagedEndpointAsync(Enum.Parse<QueryMethod>(method));

    [Given(@"the dispatcher returns (.*) items with (.*) total")]
    public void GivenTheDispatcherReturnsItemsWithTotal(int itemCount, int total)
    {
        _expectedItems = [.. Enumerable.Range(1, itemCount).Select(i => $"item{i}")];

        _dispatcherMock
            .Setup(d => d.DispatchAsync<PagedResult<string>>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<string> { Results = _expectedItems, Total = total });
    }

    [Given(@"a route builder")]
    public void GivenARouteBuilder() => _app = WebApplication.CreateSlimBuilder().Build();

    // ── When ────────────────────────────────────────────────────────────────

    [When(@"I GET the endpoint")]
    public async Task WhenIGetTheEndpoint() => _response = await _client!.GetAsync("/paged");

    [When(@"I POST the criteria to the endpoint")]
    public async Task WhenIPostTheCriteriaToTheEndpoint() => _response = await _client!.PostAsJsonAsync("/paged", new TestPagedQuery("x"));

    [When(@"I send a QUERY request with criteria to the endpoint")]
    public async Task WhenISendAQueryRequestWithCriteriaToTheEndpoint()
    {
        var request = new HttpRequestMessage(new HttpMethod("QUERY"), "/paged") { Content = JsonContent.Create(new TestPagedQuery("x")) };
        _response = await _client!.SendAsync(request);
    }

    [When(@"I map a paged endpoint with an undefined method value")]
    public void WhenIMapAPagedEndpointWithAnUndefinedMethodValue() =>
        context.CatchException(() => _app!.MapPagedQuery<TestPagedQuery, string>("/x", (QueryMethod)42));

    // ── Then ────────────────────────────────────────────────────────────────

    [Then(@"the response status should be (.*)")]
    public void ThenTheResponseStatusShouldBe(int statusCode) => Assert.Equal(statusCode, (int)_response!.StatusCode);

    [Then(@"the response body should be the unwrapped items")]
    public async Task ThenTheResponseBodyShouldBeTheUnwrappedItems()
    {
        var items = await _response!.Content.ReadFromJsonAsync<List<string>>();
        Assert.Equal(_expectedItems, items);
    }

    [Then(@"the X-Total-Count header should be '(.*)'")]
    public void ThenTheXTotalCountHeaderShouldBe(string expected) =>
        Assert.Equal(expected, _response!.Headers.GetValues("X-Total-Count").Single());

    [Then(@"an ArgumentOutOfRangeException should be thrown for '(.*)'")]
    public void ThenAnArgumentOutOfRangeExceptionShouldBeThrownFor(string parameterName)
    {
        var exception = context.GetException<ArgumentOutOfRangeException>();
        Assert.NotNull(exception);
        Assert.Equal(parameterName, exception.ParamName);
    }

    private async Task MapPagedEndpointAsync(QueryMethod? method)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(_dispatcherMock.Object);
        _app = builder.Build();

        if (method is { } explicitMethod)
        {
            _app.MapPagedQuery<TestPagedQuery, string>("/paged", explicitMethod);
        }
        else
        {
            _app.MapPagedQuery<TestPagedQuery, string>("/paged");
        }

        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _app?.StopAsync().GetAwaiter().GetResult();
        _app?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}
