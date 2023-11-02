using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules;

/// <summary>
/// A module is a collection of services and endpoints that can be added to an application.
/// </summary>
public interface IModule
{
    public IServiceCollection AddServices(IServiceCollection services);

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
