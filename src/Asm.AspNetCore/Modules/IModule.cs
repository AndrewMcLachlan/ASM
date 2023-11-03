using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Modules;

/// <summary>
/// A module is a collection of services and endpoints that can be added to an application.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Add dependencies to the service collection.
    /// </summary>
    /// <param name="services">A service collection instance.</param>
    /// <returns>The service collection instance.</returns>
    public IServiceCollection AddServices(IServiceCollection services);

    /// <summary>
    /// Maps endpoints to the application.
    /// </summary>
    /// <param name="endpoints">An endpoint route builder instance to attach the endpoints to.</param>
    /// <returns>The end point route builder.</returns>
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
