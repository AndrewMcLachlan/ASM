using System.Reflection;

namespace Asm.Cqrs.AspNetCore;

/// <summary>
/// Helpers to map groups of endpoints.
/// </summary>
public static class Groups
{
    /// <summary>
    /// Map <see cref="IEndpointGroup"/> groups in the executing assembly.
    /// </summary>
    /// <param name="builder">The <see cref="RouteGroupBuilder"/> object that this method extends.</param>
    /// <returns>A <see cref="RouteGroupBuilder"/> instance for call chaining.</returns>
    public static RouteGroupBuilder MapGroups(this RouteGroupBuilder builder) =>
        builder.MapGroups(Assembly.GetExecutingAssembly());

    /// <summary>
    /// Map <see cref="IEndpointGroup"/> groups  in the provided assembly.
    /// </summary>
    /// <param name="builder">The <see cref="RouteGroupBuilder"/> object that this method extends.</param>
    /// <param name="endpointsAssembly">The assembly containing the <see cref="IEndpointGroup"/> instance(s).</param>
    /// <returns>A <see cref="RouteGroupBuilder"/> instance for call chaining.</returns>
    public static RouteGroupBuilder MapGroups(this RouteGroupBuilder builder, Assembly endpointsAssembly)
    {
        var endpointGroups = endpointsAssembly.DefinedTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var endpointGroup in endpointGroups)
        {
            var instance = Activator.CreateInstance(endpointGroup) as IEndpointGroup;
            instance?.MapGroup(builder);
        }

        return builder;
    }

}
