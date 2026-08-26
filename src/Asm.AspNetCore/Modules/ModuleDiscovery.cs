using System.Reflection;
using System.Runtime.Loader;

namespace Asm.AspNetCore.Modules;

/// <summary>
/// Finds the assemblies that may contain <see cref="IModule"/> implementations.
/// </summary>
/// <remarks>
/// <see cref="AppDomain.CurrentDomain"/> only reports assemblies that have already been loaded, and the
/// compiler omits a reference to an assembly whose types the application never touches. A module assembly
/// is therefore invisible to both <see cref="AppDomain.GetAssemblies"/> and
/// <see cref="Assembly.GetReferencedAssemblies"/> until something forces it to load. Probing the
/// application's own directory is what makes discovery deterministic.
/// </remarks>
internal static class ModuleDiscovery
{
    private static readonly string[] FrameworkAssemblies = ["System.", "Microsoft.", "netstandard", "mscorlib", "WindowsBase"];

    /// <summary>
    /// Gets every assembly that could contain a module: those already loaded, plus any assembly deployed
    /// alongside the application that has not been loaded yet.
    /// </summary>
    /// <remarks>
    /// Framework assemblies are skipped; they cannot contain application modules. A single-file deployment
    /// has no assemblies on disk to probe, so discovery falls back to the loaded assemblies alone.
    /// </remarks>
    /// <returns>The candidate assemblies, de-duplicated by simple name.</returns>
    public static IEnumerable<Assembly> GetCandidateAssemblies()
    {
        Dictionary<string, Assembly> candidates = new(StringComparer.Ordinal);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name is string name && !IsFrameworkAssembly(name))
            {
                candidates[name] = assembly;
            }
        }

        foreach (var path in EnumerateDeployedAssemblies())
        {
            var name = Path.GetFileNameWithoutExtension(path);

            if (candidates.ContainsKey(name) || IsFrameworkAssembly(name))
            {
                continue;
            }

            try
            {
                // Load by path rather than by name: an assembly that is deployed but absent from the
                // dependency manifest cannot be resolved by name, which is exactly the case being fixed.
                candidates[name] = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            }
            catch (Exception ex) when (ex is FileNotFoundException or FileLoadException or BadImageFormatException)
            {
                // Not a managed assembly, or it cannot be loaded into this context; there is nothing to discover in it.
            }
        }

        return candidates.Values;
    }

    private static IEnumerable<string> EnumerateDeployedAssemblies()
    {
        try
        {
            return Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll");
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static bool IsFrameworkAssembly(string name) =>
        FrameworkAssemblies.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal));
}
