using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.EntraId;

/// <summary>
/// Configures the Umbraco back-office external-login provider for Microsoft Entra ID.
/// </summary>
internal sealed class EntraIdLoginOptions(IOptions<EntraIdOptions> options) : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
{
    /// <summary>
    /// The scheme name (without Umbraco's back-office prefix) used for the Entra ID login provider.
    /// </summary>
    /// <remarks>
    /// This is a provider-specific value. The underlying handler is <c>AddMicrosoftAccount</c> (OAuth 2.0),
    /// not an OpenID Connect handler, so the scheme is named after the provider rather than "OpenIdConnect".
    /// </remarks>
    public const string SchemeName = "EntraId";

    private readonly EntraIdOptions _options = options.Value;

    public void Configure(string? name, BackOfficeExternalLoginProviderOptions options)
    {
        if (name == Constants.Security.BackOfficeExternalAuthenticationTypePrefix + SchemeName)
        {
            Configure(options);
        }
    }

    public void Configure(BackOfficeExternalLoginProviderOptions options)
    {
        options.AutoLinkOptions = new ExternalSignInAutoLinkOptions(
            autoLinkExternalAccount: _options.AutoLink,
            defaultUserGroups: [.. _options.DefaultUserGroups],
            defaultCulture: null)
        {
            OnAutoLinking = (autoLinkUser, loginInfo) =>
            {
                autoLinkUser.IsApproved = true;
            },
            OnExternalLogin = (user, loginInfo) => true,
        };

        options.DenyLocalLogin = _options.DenyLocalLogin;
    }
}
