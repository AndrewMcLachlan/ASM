using Asm.OAuth;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds OAuth options to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="configSectionPath">The path of the section in configuration.</param>
    /// <returns>The <see cref="OptionsBuilder{OAuthOptions}"/> instance so that the options can be further configured.</returns>
    public static OptionsBuilder<OAuthOptions> AddOAuthOptions(this IServiceCollection services, string configSectionPath) =>
        services.AddOptions<OAuthOptions>().BindConfiguration(configSectionPath);

    /// <summary>
    /// Adds OAuth options to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="configSectionPath">The path of the section in configuration.</param>
    /// <returns>The <see cref="OptionsBuilder{AzureOAuthOptions}"/> instance so that the options can be further configured.</returns>
    public static OptionsBuilder<AzureOAuthOptions> AddAzureOAuthOptions(this IServiceCollection services, string configSectionPath)
    {
        var builder = services.AddOptions<AzureOAuthOptions>().BindConfiguration(configSectionPath);
        services.AddSingleton<IOptions<OAuthOptions>>(sp => Options.Options.Create(sp.GetRequiredService<IOptions<AzureOAuthOptions>>().Value));
        return builder;
    }
}
