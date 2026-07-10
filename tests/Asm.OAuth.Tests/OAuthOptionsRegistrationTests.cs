using Asm.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Asm.OAuth.Tests;

public class OAuthOptionsRegistrationTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static ServiceProvider BuildAzure(params (string Key, string Value)[] settings)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings.ToDictionary(s => s.Key, s => (string?)s.Value))
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddAzureOAuthOptions("AzureOAuth");
        return services.BuildServiceProvider();
    }

    private static (string, string)[] ValidAzureSettings() =>
    [
        ("AzureOAuth:Domain", "https://login.microsoftonline.com"),
        ("AzureOAuth:Audience", "api://my-api"),
        ("AzureOAuth:ClientId", "client-123"),
        ("AzureOAuth:TenantId", "11111111-1111-1111-1111-111111111111"),
    ];

    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptions_BridgesBaseOAuthOptions_AcrossAllOptionsInterfaces()
    {
        using var provider = BuildAzure(ValidAzureSettings());

        var expectedAuthority = $"https://login.microsoftonline.com/{TenantId}/v2.0";

        var fromOptions = provider.GetRequiredService<IOptions<OAuthOptions>>().Value;
        var fromMonitor = provider.GetRequiredService<IOptionsMonitor<OAuthOptions>>().CurrentValue;

        using var scope = provider.CreateScope();
        var fromSnapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<OAuthOptions>>().Value;

        // Each must resolve the polymorphic Azure options (correct Authority), not a null/default instance.
        Assert.Equal(expectedAuthority, fromOptions.Authority);
        Assert.Equal(expectedAuthority, fromSnapshot.Authority);
        Assert.Equal(expectedAuthority, fromMonitor.Authority);
        Assert.Equal("api://my-api", fromOptions.Audience);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptions_ValidateAudience_DefaultsToTrueThroughBinding()
    {
        using var provider = BuildAzure(ValidAzureSettings());

        var options = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value;

        Assert.True(options.ValidateAudience);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptions_MissingTenantId_FailsValidation()
    {
        using var provider = BuildAzure(
            ("AzureOAuth:Domain", "https://login.microsoftonline.com"),
            ("AzureOAuth:Audience", "api://my-api"),
            ("AzureOAuth:ClientId", "client-123"));

        var exception = Assert.Throws<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value);

        Assert.Contains("TenantId", exception.Message);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptions_TrailingSlashDomain_FailsValidation()
    {
        using var provider = BuildAzure(
            ("AzureOAuth:Domain", "https://login.microsoftonline.com/"),
            ("AzureOAuth:Audience", "api://my-api"),
            ("AzureOAuth:ClientId", "client-123"),
            ("AzureOAuth:TenantId", "11111111-1111-1111-1111-111111111111"));

        Assert.Throws<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddOAuthOptions_MissingRequiredFields_FailsValidation()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection([]).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddOAuthOptions("OAuth");
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<OAuthOptions>>().Value);
    }
}
