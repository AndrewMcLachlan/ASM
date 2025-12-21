#nullable enable

using System.Net.Http;

namespace Asm.AspNetCore.Tests.Integration;

public class IntegrationTestContext : IDisposable
{
    public TestWebApplication? Factory { get; set; }
    public HttpClient? Client { get; set; }
    public HttpResponseMessage? Response { get; set; }
    public string? ResponseContent { get; set; }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
