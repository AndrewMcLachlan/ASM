using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.EntraId;

/// <summary>
/// Configures <see cref="MicrosoftAccountOptions"/> with Entra ID tenant-scoped endpoints
/// for the Umbraco back-office external-login scheme.
/// </summary>
internal sealed class EntraIdBackOfficeOptions(IOptions<EntraIdOptions> entraIdOptions)
    : IConfigureNamedOptions<MicrosoftAccountOptions>
{
    public void Configure(string? name, MicrosoftAccountOptions options)
    {
        if (name == Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName)
        {
            Configure(options);
        }
    }

    public void Configure(MicrosoftAccountOptions options)
    {
        var entraId = entraIdOptions.Value;
        options.CallbackPath = "/signin-oidc";
        options.ClientId = entraId.ClientId;
        options.ClientSecret = entraId.ClientSecret;
        options.TokenEndpoint = $"https://login.microsoftonline.com/{entraId.TenantId}/oauth2/v2.0/token";
        options.AuthorizationEndpoint = $"https://login.microsoftonline.com/{entraId.TenantId}/oauth2/v2.0/authorize";
    }
}
