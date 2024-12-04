using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Routing;

/// <summary>
/// Base class for a set of endpoints grouped under a common path.
/// </summary>
public abstract class EndpointGroupBase : IEndpointGroup
{
    /// <summary>
    /// Gets the Open API name of the group.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the URL path of the group.
    /// </summary>
    public abstract string Path { get; }

    /// <summary>
    /// Gets the Open API tags of the group.
    /// </summary>
    public virtual string Tags => String.Empty;

    /// <summary>
    /// Gets the authorisation policy name that is applied to the group.
    /// </summary>
    /// <remarks>
    /// Do not override if a policy does not apply to the group.
    /// </remarks>
    public virtual string AuthorisationPolicy => String.Empty;

    /// <summary>
    /// Maps the group endpoints.
    /// </summary>r
    /// <remarks>
    /// Sets the operation name, display name, tags, authorisation policy and maps the endpoints.
    /// </remarks>
    /// <param name="builder">The builder instance that this group attaches to.</param>
    /// <returns>
    /// A Microsoft.AspNetCore.Routing.RouteGroupBuilder that is both an Microsoft.AspNetCore.Routing.IEndpointRouteBuilder
    /// and an Microsoft.AspNetCore.Builder.IEndpointConventionBuilder. The same builder
    /// can be used to add endpoints with the given prefix, and to customize those endpoints
    /// using conventions.
    /// </returns>
    public virtual RouteGroupBuilder MapGroup(IEndpointRouteBuilder builder)
    {
        var subBuilder = builder.MapGroup(Path)
            .WithName(Name)
            .WithTags(Tags);

        subBuilder = String.IsNullOrEmpty(AuthorisationPolicy) ? subBuilder.RequireAuthorization() : subBuilder.RequireAuthorization(AuthorisationPolicy);

        MapEndpoints(subBuilder);

        return subBuilder;
    }

    /// <summary>
    /// When overridden in a derived class, maps the endpoints for the group.
    /// </summary>
    /// <param name="builder">The builder instance that the endpoints attach to.</param>
    protected abstract void MapEndpoints(IEndpointRouteBuilder builder);
}