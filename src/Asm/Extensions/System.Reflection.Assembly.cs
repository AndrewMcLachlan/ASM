using System.Diagnostics;

namespace System.Reflection;

/// <summary>
/// Gets the entry assembly's version information.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Gets the entry assembly's version.
    /// </summary>
    public static Version? Version (this Assembly assembly) => assembly.GetName().Version;

    /// <summary>
    /// Gets the entry assembly's file version information.
    /// </summary>
    public static FileVersionInfo? FileVersion(this Assembly assembly) => Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

    /// <summary>
    /// Gets the entry assembly's informational version.
    /// </summary>
    public static string? InformationalVersion(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}
