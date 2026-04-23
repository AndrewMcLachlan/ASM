using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Asm.Umbraco.Authentication.EntraId;

/// <summary>
/// Supplies the back-office authentication-provider manifest for Entra ID
/// programmatically, avoiding the need for an <c>App_Plugins</c> folder in consumers.
/// </summary>
internal sealed class EntraIdManifestReader : IPackageManifestReader
{
    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var manifest = new PackageManifest
        {
            Name = "Asm.Umbraco.Authentication.EntraId",
            AllowPublicAccess = true,
            Extensions =
            [
                new
                {
                    type = "authProvider",
                    alias = "Asm.AuthProvider.EntraId",
                    name = "Microsoft Entra ID Auth Provider",
                    forProviderName = global::Umbraco.Cms.Core.Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName,
                    meta = new { label = "Microsoft" }
                }
            ]
        };

        return Task.FromResult<IEnumerable<PackageManifest>>([manifest]);
    }
}
