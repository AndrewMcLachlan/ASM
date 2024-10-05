using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Routing;

/// <summary>
/// Represents a group of endpoints with a common ancestor path.
/// </summary>
public interface IEndpointGroup
{
    /// <summary>
    /// Maps the group.
    /// </summary>
    /// <param name="builder">The builder instance that this group attaches to.</param>
    /// <returns>The same instance as <paramref name="builder"/>, for call chaining.</returns>
    IEndpointRouteBuilder MapGroup(IEndpointRouteBuilder builder);
}
