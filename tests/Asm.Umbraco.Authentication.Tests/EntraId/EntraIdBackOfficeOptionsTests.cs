using Asm.Umbraco.Authentication.EntraId;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdBackOfficeOptionsTests
{
    private const string TenantId = "aaaabbbb-1111-2222-3333-ccccddddeeee";
    private const string ClientId = "my-client-id";
    private const string ClientSecret = "my-client-secret";

    private const string MatchingName =
        Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName;

    private static EntraIdBackOfficeOptions CreateSut() =>
        new(Options.Create(new EntraIdOptions
        {
            TenantId = TenantId,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        }));

    // -----------------------------------------------------------------------
    // Configure(string?, MicrosoftAccountOptions)
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsClientId()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(ClientId, options.ClientId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsClientSecret()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(ClientSecret, options.ClientSecret);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsCallbackPath()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal("/signin-oidc", options.CallbackPath);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsTokenEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token",
            options.TokenEndpoint);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsAuthorizationEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize",
            options.AuthorizationEndpoint);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithNonMatchingName_LeavesClientIdUnchanged()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();
        var originalClientId = options.ClientId;

        sut.Configure("SomeDifferentScheme", options);

        Assert.Equal(originalClientId, options.ClientId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithNonMatchingName_LeavesClientSecretUnchanged()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();
        var originalSecret = options.ClientSecret;

        sut.Configure("SomeDifferentScheme", options);

        Assert.Equal(originalSecret, options.ClientSecret);
    }

    // -----------------------------------------------------------------------
    // Configure(MicrosoftAccountOptions) — no-name overload
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsClientId()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(ClientId, options.ClientId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsClientSecret()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(ClientSecret, options.ClientSecret);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsCallbackPath()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal("/signin-oidc", options.CallbackPath);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsTokenEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token",
            options.TokenEndpoint);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsAuthorizationEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize",
            options.AuthorizationEndpoint);
    }
}
