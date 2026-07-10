using Asm.OAuth;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds OAuth options to the service collection, validated on startup.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="configSectionPath">The path of the section in configuration.</param>
    /// <returns>The <see cref="OptionsBuilder{OAuthOptions}"/> instance so that the options can be further configured.</returns>
    public static OptionsBuilder<OAuthOptions> AddOAuthOptions(this IServiceCollection services, string configSectionPath) =>
        services.AddOptions<OAuthOptions>()
                .BindConfiguration(configSectionPath)
                .Validate(o => !String.IsNullOrWhiteSpace(o.Domain), "OAuth options: Domain is required.")
                .Validate(o => !String.IsNullOrWhiteSpace(o.Audience), "OAuth options: Audience is required.")
                .Validate(o => !String.IsNullOrWhiteSpace(o.ClientId), "OAuth options: ClientId is required.")
                .ValidateOnStart();

    /// <summary>
    /// Adds Azure Entra OAuth options to the service collection, validated on startup.
    /// </summary>
    /// <remarks>
    /// The bound <see cref="AzureOAuthOptions"/> is also resolvable as its base
    /// <see cref="OAuthOptions"/> via <see cref="IOptions{TOptions}"/>,
    /// <see cref="IOptionsSnapshot{TOptions}"/> and <see cref="IOptionsMonitor{TOptions}"/>.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> instance that this method extends.</param>
    /// <param name="configSectionPath">The path of the section in configuration.</param>
    /// <returns>The <see cref="OptionsBuilder{AzureOAuthOptions}"/> instance so that the options can be further configured.</returns>
    public static OptionsBuilder<AzureOAuthOptions> AddAzureOAuthOptions(this IServiceCollection services, string configSectionPath)
    {
        var builder = services.AddOptions<AzureOAuthOptions>()
                              .BindConfiguration(configSectionPath)
                              .Validate(o => !String.IsNullOrWhiteSpace(o.Domain), "Azure OAuth options: Domain is required.")
                              .Validate(o => !(o.Domain?.EndsWith('/') ?? false), "Azure OAuth options: Domain must not end with '/'.")
                              .Validate(o => !String.IsNullOrWhiteSpace(o.Audience), "Azure OAuth options: Audience is required.")
                              .Validate(o => !String.IsNullOrWhiteSpace(o.ClientId), "Azure OAuth options: ClientId is required.")
                              .Validate(o => o.TenantId != Guid.Empty, "Azure OAuth options: TenantId is required.")
                              .ValidateOnStart();

        // Bridge the base OAuthOptions to the configured Azure instance so that
        // IOptions/IOptionsSnapshot/IOptionsMonitor<OAuthOptions> all resolve the same (polymorphic)
        // Azure options and reflect configuration reloads — rather than an unbound, null instance.
        services.TryAddSingleton<IOptions<OAuthOptions>>(sp => new AzureOAuthOptionsBridge(sp.GetRequiredService<IOptionsMonitor<AzureOAuthOptions>>()));
        services.TryAddScoped<IOptionsSnapshot<OAuthOptions>>(sp => new AzureOAuthOptionsBridge(sp.GetRequiredService<IOptionsMonitor<AzureOAuthOptions>>()));
        services.TryAddSingleton<IOptionsMonitor<OAuthOptions>>(sp => new AzureOAuthOptionsBridge(sp.GetRequiredService<IOptionsMonitor<AzureOAuthOptions>>()));

        return builder;
    }

    private sealed class AzureOAuthOptionsBridge(IOptionsMonitor<AzureOAuthOptions> monitor)
        : IOptions<OAuthOptions>, IOptionsSnapshot<OAuthOptions>, IOptionsMonitor<OAuthOptions>
    {
        public OAuthOptions Value => monitor.CurrentValue;

        public OAuthOptions CurrentValue => monitor.CurrentValue;

        public OAuthOptions Get(string? name) => monitor.Get(name);

        public IDisposable? OnChange(Action<OAuthOptions, string?> listener) => monitor.OnChange(listener);
    }
}
