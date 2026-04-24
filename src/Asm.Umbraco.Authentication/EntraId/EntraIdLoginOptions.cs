using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.EntraId;

/// <summary>
/// Configures the Umbraco back-office external-login provider for Microsoft Entra ID.
/// </summary>
internal sealed class EntraIdLoginOptions : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
{
    /// <summary>
    /// The scheme name (without Umbraco's back-office prefix) used for the Entra ID login provider.
    /// </summary>
    public const string SchemeName = "OpenIdConnect";

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
            autoLinkExternalAccount: true,
            defaultUserGroups: [Constants.Security.EditorGroupKey.ToString()],
            defaultCulture: null)
        {
            OnAutoLinking = (autoLinkUser, loginInfo) =>
            {
                autoLinkUser.IsApproved = true;
            },
            OnExternalLogin = (user, loginInfo) => true,
        };

        options.DenyLocalLogin = false;
    }
}
