using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension for the <see cref="IMcpServerBuilder"/> interface.
/// </summary>
public static class AsmModelContextProtocolMcpServerBuilderExtensions
{
    /// <summary>
    /// Adds tools from all assemblies whose name contains the specified pattern.
    /// </summary>
    /// <remarks>
    /// Both the assemblies already loaded into the current <see cref="AppDomain"/> and any matching
    /// referenced assemblies (which may not yet be loaded) are searched, so tools are discovered even
    /// when their assembly has not been touched at start-up.
    /// </remarks>
    /// <param name="builder">The <see cref="IMcpServerBuilder"/> instance that this method extends.</param>
    /// <param name="pattern">Only search assemblies where the name contains this pattern.</param>
    /// <returns>The <see cref="IMcpServerBuilder"/> so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="pattern"/> is <c>null</c> or empty.</exception>
    public static IMcpServerBuilder WithToolsFromAssemblies(this IMcpServerBuilder builder, string pattern)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(pattern);

        var matches = new Dictionary<string, Assembly>(StringComparer.Ordinal);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;
            if (name is not null && name.Contains(pattern, StringComparison.Ordinal))
            {
                matches[name] = assembly;
            }

            // Referenced assemblies may not be loaded yet; load any that match so their tools are discoverable.
            foreach (var referenced in assembly.GetReferencedAssemblies())
            {
                if (referenced.Name is not string referencedName || matches.ContainsKey(referencedName) || !referencedName.Contains(pattern, StringComparison.Ordinal))
                {
                    continue;
                }

                try
                {
                    matches[referencedName] = Assembly.Load(referenced);
                }
                catch (Exception ex) when (ex is FileNotFoundException or FileLoadException or BadImageFormatException)
                {
                    // Ignore assemblies that cannot be loaded.
                }
            }
        }

        foreach (var assembly in matches.Values)
        {
            builder.WithToolsFromAssembly(assembly);
        }

        return builder;
    }
}
