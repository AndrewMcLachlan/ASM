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

    /// <summary>
    /// Given Azure OAuth options registered from valid configuration
    /// When base OAuthOptions are resolved via IOptions, IOptionsMonitor and IOptionsSnapshot
    /// Then each returns the polymorphic Azure options with the correct Authority and Audience
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptionsBridgesBaseOAuthOptionsAcrossAllOptionsInterfaces()
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

    /// <summary>
    /// Given Azure OAuth options bound from configuration that does not set ValidateAudience
    /// When the AzureOAuthOptions are resolved
    /// Then ValidateAudience defaults to true
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptionsValidateAudienceDefaultsToTrueThroughBinding()
    {
        using var provider = BuildAzure(ValidAzureSettings());

        var options = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value;

        Assert.True(options.ValidateAudience);
    }

    /// <summary>
    /// Given Azure OAuth configuration that omits the TenantId
    /// When the AzureOAuthOptions are resolved
    /// Then an OptionsValidationException is thrown whose message mentions TenantId
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptionsMissingTenantIdFailsValidation()
    {
        using var provider = BuildAzure(
            ("AzureOAuth:Domain", "https://login.microsoftonline.com"),
            ("AzureOAuth:Audience", "api://my-api"),
            ("AzureOAuth:ClientId", "client-123"));

        var exception = Assert.Throws<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value);

        Assert.Contains("TenantId", exception.Message);
    }

    /// <summary>
    /// Given Azure OAuth configuration whose Domain has a trailing slash
    /// When the AzureOAuthOptions are resolved
    /// Then an OptionsValidationException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddAzureOAuthOptionsTrailingSlashDomainFailsValidation()
    {
        using var provider = BuildAzure(
            ("AzureOAuth:Domain", "https://login.microsoftonline.com/"),
            ("AzureOAuth:Audience", "api://my-api"),
            ("AzureOAuth:ClientId", "client-123"),
            ("AzureOAuth:TenantId", "11111111-1111-1111-1111-111111111111"));

        Assert.Throws<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<AzureOAuthOptions>>().Value);
    }

    /// <summary>
    /// Given plain OAuth options registered from empty configuration missing required fields
    /// When the OAuthOptions are resolved
    /// Then an OptionsValidationException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddOAuthOptionsMissingRequiredFieldsFailsValidation()
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
