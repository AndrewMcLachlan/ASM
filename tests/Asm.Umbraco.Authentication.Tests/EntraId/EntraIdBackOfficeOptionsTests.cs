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

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the MicrosoftAccountOptions ClientId is set from the EntraId options.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsClientId()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(ClientId, options.ClientId);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the MicrosoftAccountOptions ClientSecret is set from the EntraId options.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsClientSecret()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(ClientSecret, options.ClientSecret);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the MicrosoftAccountOptions CallbackPath is "/signin-entraid".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsCallbackPath()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal("/signin-entraid", options.CallbackPath);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the MicrosoftAccountOptions TokenEndpoint is the tenant-specific token URL.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsTokenEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token",
            options.TokenEndpoint);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the MicrosoftAccountOptions AuthorizationEndpoint is the tenant-specific authorize URL.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsAuthorizationEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(MatchingName, options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize",
            options.AuthorizationEndpoint);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with a non-matching scheme name.
    /// Then the MicrosoftAccountOptions ClientId is left unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithNonMatchingNameLeavesClientIdUnchanged()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();
        var originalClientId = options.ClientId;

        sut.Configure("SomeDifferentScheme", options);

        Assert.Equal(originalClientId, options.ClientId);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When Configure is called with a non-matching scheme name.
    /// Then the MicrosoftAccountOptions ClientSecret is left unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithNonMatchingNameLeavesClientSecretUnchanged()
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

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When the no-name Configure overload is called.
    /// Then the MicrosoftAccountOptions ClientId is set from the EntraId options.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsClientId()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(ClientId, options.ClientId);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When the no-name Configure overload is called.
    /// Then the MicrosoftAccountOptions ClientSecret is set from the EntraId options.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsClientSecret()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(ClientSecret, options.ClientSecret);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When the no-name Configure overload is called.
    /// Then the MicrosoftAccountOptions CallbackPath is "/signin-entraid".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsCallbackPath()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal("/signin-entraid", options.CallbackPath);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When the no-name Configure overload is called.
    /// Then the MicrosoftAccountOptions TokenEndpoint is the tenant-specific token URL.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsTokenEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token",
            options.TokenEndpoint);
    }

    /// <summary>
    /// Given EntraIdBackOfficeOptions and MicrosoftAccountOptions.
    /// When the no-name Configure overload is called.
    /// Then the MicrosoftAccountOptions AuthorizationEndpoint is the tenant-specific authorize URL.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsAuthorizationEndpoint()
    {
        var sut = CreateSut();
        var options = new MicrosoftAccountOptions();

        sut.Configure(options);

        Assert.Equal(
            $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize",
            options.AuthorizationEndpoint);
    }
}
