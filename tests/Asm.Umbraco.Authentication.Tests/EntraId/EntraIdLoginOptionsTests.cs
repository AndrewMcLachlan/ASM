using Asm.Umbraco.Authentication.EntraId;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Security;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdLoginOptionsTests
{
    private const string MatchingName =
        Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName;

    private static EntraIdLoginOptions CreateSut(EntraIdOptions? options = null) =>
        new(Options.Create(options ?? new EntraIdOptions
        {
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret",
        }));

    [Fact]
    [Trait("Category", "Unit")]
    public void SchemeName_IsProviderSpecific()
    {
        // v4: renamed from "OpenIdConnect" so it does not masquerade as an OIDC handler.
        Assert.Equal("EntraId", EntraIdLoginOptions.SchemeName);
    }

    // -----------------------------------------------------------------------
    // Configure(string?, BackOfficeExternalLoginProviderOptions)
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsAutoLinkOptionsNotNull()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.NotNull(options.AutoLinkOptions);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_AutoLinksExternalAccount()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.True(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithMatchingName_SetsDenyLocalLoginFalse()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.False(options.DenyLocalLogin);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithNonMatchingName_LeavesAutoLinkOptionsUnchanged()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        // Capture the default state before calling Configure with a non-matching name
        var defaultAutoLinkExternalAccount = options.AutoLinkOptions.AutoLinkExternalAccount;

        sut.Configure("SomeDifferentScheme", options);

        // The Configure call should have been a no-op — auto-link state must be unchanged
        Assert.Equal(defaultAutoLinkExternalAccount, options.AutoLinkOptions.AutoLinkExternalAccount);
        Assert.False(options.AutoLinkOptions.AutoLinkExternalAccount,
            "Non-matching name must not enable auto-linking.");
    }

    // -----------------------------------------------------------------------
    // Configure(BackOfficeExternalLoginProviderOptions) — no-name overload
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsAutoLinkOptionsNotNull()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.NotNull(options.AutoLinkOptions);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_AutoLinksExternalAccount()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.True(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_WithoutName_SetsDenyLocalLoginFalse()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.False(options.DenyLocalLogin);
    }

    // -----------------------------------------------------------------------
    // OnAutoLinking callback
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void OnAutoLinking_SetsIsApprovedTrue()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();
        sut.Configure(options);

        var user = new BackOfficeIdentityUser(new GlobalSettings(), 1, []);
        var loginInfo = CreateExternalLoginInfo();

        options.AutoLinkOptions.OnAutoLinking?.Invoke(user, loginInfo);

        Assert.True(user.IsApproved);
    }

    // -----------------------------------------------------------------------
    // OnExternalLogin callback
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void OnExternalLogin_ReturnsTrue()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();
        sut.Configure(options);

        var user = new BackOfficeIdentityUser(new GlobalSettings(), 1, []);
        var loginInfo = CreateExternalLoginInfo();

        var result = options.AutoLinkOptions.OnExternalLogin?.Invoke(user, loginInfo);

        Assert.True(result);
    }

    // -----------------------------------------------------------------------
    // Options flow through from EntraIdOptions
    // -----------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_HonoursAutoLinkFalse()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", AutoLink = false });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.False(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_HonoursDenyLocalLoginTrue()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", DenyLocalLogin = true });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.True(options.DenyLocalLogin);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_HonoursCustomDefaultUserGroups()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", DefaultUserGroups = ["writer", "translator"] });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.Equal(["writer", "translator"], options.AutoLinkOptions.DefaultUserGroups);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Configure_DefaultUserGroups_IsEditor()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.Equal([Constants.Security.EditorGroupKey.ToString()], options.AutoLinkOptions.DefaultUserGroups);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static ExternalLoginInfo CreateExternalLoginInfo() =>
        new(
            new System.Security.Claims.ClaimsPrincipal(),
            "test-provider",
            "test-key",
            "Test Provider");
}
