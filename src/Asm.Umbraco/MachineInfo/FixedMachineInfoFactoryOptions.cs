namespace Asm.Umbraco.MachineInfo;

/// <summary>
/// Options for <see cref="FixedMachineInfoFactory"/>.
/// </summary>
public record FixedMachineInfoFactoryOptions
{
    /// <summary>
    /// The fallback machine name used when no environment variable override is set.
    /// </summary>
    public required string MachineName { get; set; }

    /// <summary>
    /// The name of the environment variable whose value, when set, overrides <see cref="MachineName"/>.
    /// Defaults to <c>MACHINE_NAME</c>.
    /// </summary>
    public string EnvironmentVariableName { get; set; } = "MACHINE_NAME";
}
