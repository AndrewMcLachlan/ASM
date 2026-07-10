using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Routing;

/// <summary>
/// Helpers to map groups of endpoints.
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// Map <see cref="IEndpointGroup"/> groups in the calling assembly.
    /// </summary>
    /// <param name="builder">The <see cref="RouteGroupBuilder"/> object that this method extends.</param>
    /// <returns>A <see cref="RouteGroupBuilder"/> instance for call chaining.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static RouteGroupBuilder MapGroups(this RouteGroupBuilder builder) =>
        builder.MapGroups(Assembly.GetCallingAssembly());

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
