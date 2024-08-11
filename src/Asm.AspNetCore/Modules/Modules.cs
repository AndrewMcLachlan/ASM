using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules;

/// <summary>
/// Extensions for <see cref="WebApplicationBuilder"/>, <see cref="IServiceCollection"/> and <see cref="IEndpointRouteBuilder"/> to register and map modules in a web application.
/// </summary>
public static class Modules
{
    private static List<IModule> RegisteredModules = [];

    /// <summary>
    /// Registers modules.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance that this method extends.</param>
    /// <param name="modules">A function that returns a list of module instances.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> instance so that calls can be chained.</returns>
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, Func<IEnumerable<IModule>> modules)
    {
        RegisteredModules = modules().ToList();
        builder.Services.AddModules();
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
        RegisteredModules = modulesAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IModule)))
                                            .Select(Activator.CreateInstance).Cast<IModule>()
                                            .ToList();
        builder.Services.AddModules();
        return builder;
    }

    /// <summary>
    /// Maps module endpoints.
    /// </summary>
    /// <param name="endpoints">An endpoint route builder instance to attach the endpoints to.</param>
    /// <returns>The end point route builder.</returns>
    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in RegisteredModules)
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private static IServiceCollection AddModules(this IServiceCollection services)
    {
        foreach (var module in RegisteredModules)
        {
            module.AddServices(services);
        }

        return services;
    }
}
