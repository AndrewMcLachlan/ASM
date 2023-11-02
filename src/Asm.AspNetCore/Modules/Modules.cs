using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules;

public static class Modules
{
    private static List<IModule> _modules = new();

    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, Func<IEnumerable<IModule>> modules)
    {
        _modules = modules().ToList();
        builder.Services.AddModules();
        return builder;
    }

    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder, Assembly modulseAssembly)
    {
        _modules = modulseAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IModule)))
                                            .Select(Activator.CreateInstance).Cast<IModule>()
                                            .ToList();
        builder.Services.AddModules();
        return builder;
    }

    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in _modules)
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private static IServiceCollection AddModules(this IServiceCollection services)
    {
        foreach (var module in _modules)
        {
            module.AddServices(services);
        }

        return services;
    }
}
