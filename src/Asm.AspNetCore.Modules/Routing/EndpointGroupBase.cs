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
    /// Gets the URL path of the group.
    /// </summary>
    public abstract string Path { get; }

    /// <summary>
    /// Gets the Open API tags of the group, or <c>null</c> for no tags.
    /// </summary>
    public virtual string[]? Tags => null;

    /// <summary>
    /// Gets the authorisation policy name that is applied to the group, or <c>null</c> to apply the default (any authenticated user).
    /// </summary>
    /// <remarks>
    /// Leave as <c>null</c> if no specific policy applies to the group. To make the whole group anonymous, override <see cref="AllowAnonymous"/> instead.
    /// </remarks>
    public virtual string? AuthorisationPolicy => null;

    /// <summary>
    /// Gets a value indicating whether the group is anonymous (opts out of authorisation entirely).
    /// </summary>
    public virtual bool AllowAnonymous => false;

    /// <summary>
    /// Maps the group endpoints.
    /// </summary>
    /// <remarks>
    /// Applies the group tags and authorisation policy, then maps the endpoints. Endpoint names
    /// (<c>WithName</c>) are the responsibility of individual endpoints in <see cref="MapEndpoints"/>,
    /// since a group-level name would be applied to every endpoint and endpoint names must be unique.
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
        var subBuilder = builder.MapGroup(Path);

        if (Tags is { Length: > 0 } tags)
        {
            subBuilder.WithTags(tags);
        }

        if (AllowAnonymous)
        {
            subBuilder.AllowAnonymous();
        }
        else if (String.IsNullOrEmpty(AuthorisationPolicy))
        {
            subBuilder.RequireAuthorization();
        }
        else
        {
            subBuilder.RequireAuthorization(AuthorisationPolicy);
        }

        MapEndpoints(subBuilder);

        return subBuilder;
    }

    /// <summary>
    /// When overridden in a derived class, maps the endpoints for the group.
    /// </summary>
    /// <param name="builder">The builder instance that the endpoints attach to.</param>
    protected abstract void MapEndpoints(IEndpointRouteBuilder builder);
}