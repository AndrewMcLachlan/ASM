using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System;

/// <summary>
/// Gets the entry assembly's version information.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AssemblyVersion
{
    private static readonly Assembly? Assembly = Assembly.GetEntryAssembly();


    private static readonly Lazy<Version?> AssemblyVersionLazy = new(Assembly?.GetName().Version);
    private static readonly Lazy<FileVersionInfo?> FileVersionLazy = new(Assembly is null ? null : FileVersionInfo.GetVersionInfo(Assembly.Location));
    private static readonly Lazy<string?> AssemblyInformationalVersionLazy = new(Assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

    /// <summary>
    /// Gets the entry assembly's version.
    /// </summary>
    public static Version? Version => AssemblyVersionLazy.Value;

    /// <summary>
    /// Gets the entry assembly's file version information.
    /// </summary>
    public static FileVersionInfo? FileVersion => FileVersionLazy.Value;

    /// <summary>
    /// Gets the entry assembly's informational version.
    /// </summary>
    public static string? InformationalVersion => AssemblyInformationalVersionLazy.Value;
}
