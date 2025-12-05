namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension for the <see cref="IMcpServerBuilder"/> interface.
/// </summary>
public static class IMcpServerBuilderExtensions
{
    /// <summary>
    /// Adds tools from all assemblies in the current app domain that match the specified pattern. 
    /// </summary>
    /// <param name="builder">The <see cref="IMcpServerBuilder"/> instance that this method extends.</param>
    /// <param name="pattern">Only search assemblies where the name contains this pattern.</param>
    /// <returns>The <see cref="IMcpServerBuilder"/> so that calls can be chained.</returns>
    public static IMcpServerBuilder WithToolFromAssemblies(this IMcpServerBuilder builder, string pattern)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.Contains(pattern) == true);
        foreach (var assembly in assemblies)
        {
            builder.WithToolsFromAssembly(assembly);
        }
        return builder;
    }
}
