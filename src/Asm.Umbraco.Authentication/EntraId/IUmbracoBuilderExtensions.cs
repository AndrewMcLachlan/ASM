using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Management.Security;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Extensions;
using Asm.Umbraco.Authentication.EntraId;

namespace Umbraco.Cms.Core.DependencyInjection;

/// <summary>
/// Extensions on <see cref="IUmbracoBuilder"/> for registering
/// Microsoft Entra ID back-office authentication.
/// </summary>
public static class IUmbracoBuilderExtensions
{
    /// <summary>
    /// Registers Microsoft Entra ID as an Umbraco back-office external login provider
    /// using the supplied configuration action.
    /// </summary>
    /// <param name="builder">The Umbraco builder.</param>
    /// <param name="configure">Callback to configure the Entra ID options.</param>
    /// <returns>The Umbraco builder.</returns>
    public static IUmbracoBuilder AddEntraIdAuthentication(
        this IUmbracoBuilder builder,
        Action<EntraIdOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure(configure);
        return AddEntraIdCore(builder);
    }

    /// <summary>
    /// Registers Microsoft Entra ID as an Umbraco back-office external login provider,
    /// binding options from the supplied configuration section.
    /// </summary>
    /// <param name="builder">The Umbraco builder.</param>
    /// <param name="configuration">The configuration section to bind options from.</param>
    /// <returns>The Umbraco builder.</returns>
    public static IUmbracoBuilder AddEntraIdAuthentication(
        this IUmbracoBuilder builder,
        IConfigurationSection configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.Services.Configure<EntraIdOptions>(configuration);
        return AddEntraIdCore(builder);
    }

    private static IUmbracoBuilder AddEntraIdCore(IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<EntraIdLoginOptions>();
        builder.Services.ConfigureOptions<EntraIdBackOfficeOptions>();
        builder.Services.AddSingleton<IPackageManifestReader, EntraIdManifestReader>();

        builder.AddBackOfficeExternalLogins(logins =>
        {
            logins.AddBackOfficeLogin(backOfficeAuthenticationBuilder =>
            {
                var schemeName = BackOfficeAuthenticationBuilder.SchemeForBackOffice(EntraIdLoginOptions.SchemeName);
                ArgumentNullException.ThrowIfNull(schemeName, nameof(schemeName));

                backOfficeAuthenticationBuilder.AddMicrosoftAccount(
                    schemeName,
                    options => { /* configured via EntraIdBackOfficeOptions */ });
            });
        });

        return builder;
    }
}
