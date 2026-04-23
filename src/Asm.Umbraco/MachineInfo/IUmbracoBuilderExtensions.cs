using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Factories;
using Umbraco.Extensions;
using Asm.Umbraco.MachineInfo;

namespace Umbraco.Cms.Core.DependencyInjection;

/// <summary>
/// Extensions on <see cref="IUmbracoBuilder"/> for registering
/// <see cref="FixedMachineInfoFactory"/>.
/// </summary>
public static class IUmbracoBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="FixedMachineInfoFactory"/> as the Umbraco
    /// <see cref="IMachineInfoFactory"/> with the supplied options.
    /// </summary>
    /// <param name="builder">The Umbraco builder.</param>
    /// <param name="configure">Callback to configure the factory options.</param>
    /// <returns>The Umbraco builder.</returns>
    public static IUmbracoBuilder AddFixedMachineInfoFactory(
        this IUmbracoBuilder builder,
        Action<FixedMachineInfoFactoryOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure(configure);
        builder.Services.AddUnique<IMachineInfoFactory, FixedMachineInfoFactory>();
        return builder;
    }

    /// <summary>
    /// Registers <see cref="FixedMachineInfoFactory"/> as the Umbraco
    /// <see cref="IMachineInfoFactory"/>, binding options from the supplied configuration section.
    /// </summary>
    /// <param name="builder">The Umbraco builder.</param>
    /// <param name="configuration">The configuration section to bind options from.</param>
    /// <returns>The Umbraco builder.</returns>
    public static IUmbracoBuilder AddFixedMachineInfoFactory(
        this IUmbracoBuilder builder,
        IConfigurationSection configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.Services.Configure<FixedMachineInfoFactoryOptions>(configuration);
        builder.Services.AddUnique<IMachineInfoFactory, FixedMachineInfoFactory>();
        return builder;
    }
}
