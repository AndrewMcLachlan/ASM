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

    /// <summary>
    /// Given the EntraIdLoginOptions type.
    /// When its SchemeName constant is read.
    /// Then it is the provider-specific value "EntraId".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SchemeNameIsProviderSpecific()
    {
        // v4: renamed from "OpenIdConnect" so it does not masquerade as an OIDC handler.
        Assert.Equal("EntraId", EntraIdLoginOptions.SchemeName);
    }

    // -----------------------------------------------------------------------
    // Configure(string?, BackOfficeExternalLoginProviderOptions)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When Configure is called with the matching scheme name.
    /// Then the AutoLinkOptions are not null.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsAutoLinkOptionsNotNull()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.NotNull(options.AutoLinkOptions);
    }

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When Configure is called with the matching scheme name.
    /// Then AutoLinkExternalAccount is enabled.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameAutoLinksExternalAccount()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.True(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When Configure is called with the matching scheme name.
    /// Then DenyLocalLogin is false.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithMatchingNameSetsDenyLocalLoginFalse()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(MatchingName, options);

        Assert.False(options.DenyLocalLogin);
    }

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When Configure is called with a non-matching scheme name.
    /// Then AutoLinkExternalAccount is unchanged and remains disabled.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithNonMatchingNameLeavesAutoLinkOptionsUnchanged()
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

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When the no-name Configure overload is called.
    /// Then the AutoLinkOptions are not null.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsAutoLinkOptionsNotNull()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.NotNull(options.AutoLinkOptions);
    }

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When the no-name Configure overload is called.
    /// Then AutoLinkExternalAccount is enabled.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameAutoLinksExternalAccount()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.True(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    /// <summary>
    /// Given EntraIdLoginOptions and BackOfficeExternalLoginProviderOptions.
    /// When the no-name Configure overload is called.
    /// Then DenyLocalLogin is false.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureWithoutNameSetsDenyLocalLoginFalse()
    {
        var sut = CreateSut();
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.False(options.DenyLocalLogin);
    }

    // -----------------------------------------------------------------------
    // OnAutoLinking callback
    // -----------------------------------------------------------------------

    /// <summary>
    /// Given configured EntraIdLoginOptions and a back-office identity user with external login info.
    /// When the AutoLinkOptions OnAutoLinking callback is invoked.
    /// Then the user's IsApproved is set to true.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void OnAutoLinkingSetsIsApprovedTrue()
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

    /// <summary>
    /// Given configured EntraIdLoginOptions and a back-office identity user with external login info.
    /// When the AutoLinkOptions OnExternalLogin callback is invoked.
    /// Then it returns true.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void OnExternalLoginReturnsTrue()
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

    /// <summary>
    /// Given EntraIdOptions with AutoLink set to false.
    /// When Configure is called on the resulting login options.
    /// Then AutoLinkExternalAccount is disabled.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureHonoursAutoLinkFalse()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", AutoLink = false });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.False(options.AutoLinkOptions.AutoLinkExternalAccount);
    }

    /// <summary>
    /// Given EntraIdOptions with DenyLocalLogin set to true.
    /// When Configure is called on the resulting login options.
    /// Then DenyLocalLogin is true.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureHonoursDenyLocalLoginTrue()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", DenyLocalLogin = true });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.True(options.DenyLocalLogin);
    }

    /// <summary>
    /// Given EntraIdOptions with custom DefaultUserGroups.
    /// When Configure is called on the resulting login options.
    /// Then the AutoLinkOptions DefaultUserGroups match the configured groups.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureHonoursCustomDefaultUserGroups()
    {
        var sut = CreateSut(new EntraIdOptions { TenantId = "t", ClientId = "c", ClientSecret = "s", DefaultUserGroups = ["writer", "translator"] });
        var options = new BackOfficeExternalLoginProviderOptions();

        sut.Configure(options);

        Assert.Equal(["writer", "translator"], options.AutoLinkOptions.DefaultUserGroups);
    }

    /// <summary>
    /// Given EntraIdLoginOptions created with default EntraIdOptions.
    /// When Configure is called.
    /// Then the AutoLinkOptions DefaultUserGroups default to the editor group.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ConfigureDefaultUserGroupsIsEditor()
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
