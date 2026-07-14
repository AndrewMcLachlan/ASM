using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules;

/// <summary>
/// Extensions for <see cref="WebApplicationBuilder"/>, <see cref="IServiceCollection"/> and <see cref="IEndpointRouteBuilder"/> to register and map modules in a web application.
/// </summary>
/// <remarks>
/// Modules are registered into dependency injection as <see cref="IModule"/> singletons. Registration is
/// additive: registering the same or different modules more than once accumulates them rather than replacing
/// any previously-registered module. <see cref="MapModuleEndpoints(IEndpointRouteBuilder)"/> resolves the
/// registered modules from the application's <see cref="IServiceProvider"/>, so no global/static state is shared
/// between applications or containers.
/// </remarks>
public static class Modules
{
    /// <summary>
    /// Discovers and registers modules from all assemblies in the current <see cref="AppDomain"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance that this method extends.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddModules(AppDomain.CurrentDomain.GetAssemblies());
        return builder;
    }

    /// <summary>
    /// Discovers and registers modules from assemblies matching a specific pattern.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance that this method extends.</param>
    /// <param name="pattern">Only search assemblies where the name contains this pattern.</param>
    /// <example>
    /// builder.RegisterModules("MyApp.Modules");
    /// </example>
    /// <returns>The <see cref="WebApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, string pattern)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var modulesAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.Contains(pattern) == true);
        builder.Services.AddModules(modulesAssemblies);
        return builder;
    }

    /// <summary>
    /// Registers modules.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance that this method extends.</param>
    /// <param name="modules">A function that returns a list of module instances.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, Func<IEnumerable<IModule>> modules)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(modules);

        builder.Services.AddModules(modules());
        return builder;
    }

    /// <summary>
    /// Discovers and registers modules from an assembly.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance that this method extends.</param>
    /// <param name="modulesAssembly">The assembly to search.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, Assembly modulesAssembly)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(modulesAssembly);

        builder.Services.AddModules(modulesAssembly);
        return builder;
    }

    /// <summary>
    /// Registers a single module of type <typeparamref name="T"/> into dependency injection.
    /// </summary>
    /// <remarks>
    /// The module is instantiated, its <see cref="IModule.AddServices(IServiceCollection)"/> is invoked, and the
    /// instance is registered as an <see cref="IModule"/> singleton. Registration is additive.
    /// </remarks>
    /// <typeparam name="T">The module type. Must have a public parameterless constructor.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddModule<T>(this IServiceCollection services) where T : class, IModule, new()
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddModule(new T());
    }

    /// <summary>
    /// Registers a module instance into dependency injection.
    /// </summary>
    /// <remarks>
    /// The module's <see cref="IModule.AddServices(IServiceCollection)"/> is invoked and the instance is registered
    /// as an <see cref="IModule"/> singleton. Registration is additive.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <param name="module">The module instance to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddModule(this IServiceCollection services, IModule module)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(module);

        module.AddServices(services);
        services.AddSingleton(module);
        return services;
    }

    /// <summary>
    /// Registers a collection of module instances into dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <param name="modules">The module instances to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddModules(this IServiceCollection services, IEnumerable<IModule> modules)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(modules);

        foreach (var module in modules)
        {
            services.AddModule(module);
        }

        return services;
    }

    /// <summary>
    /// Discovers <see cref="IModule"/> implementations in the supplied assemblies and registers them into dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IModule"/> implementations.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddModules(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        return services.AddModules((IEnumerable<Assembly>)assemblies);
    }

    /// <summary>
    /// Discovers <see cref="IModule"/> implementations in the supplied assemblies and registers them into dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> that this method extends.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IModule"/> implementations.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
    public static IServiceCollection AddModules(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        return services.AddModules(DiscoverModules(assemblies));
    }

    /// <summary>
    /// Maps the endpoints for every <see cref="IModule"/> registered in the application's service provider.
    /// </summary>
    /// <param name="endpoints">An endpoint route builder instance to attach the endpoints to.</param>
    /// <returns>The end point route builder.</returns>
    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        foreach (var module in endpoints.ServiceProvider.GetServices<IModule>())
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private static IEnumerable<IModule> DiscoverModules(IEnumerable<Assembly> assemblies) =>
        assemblies.SelectMany(a => a.GetTypes()
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IModule)))
                                    .Select(Activator.CreateInstance)
                                    .Cast<IModule>());
}
