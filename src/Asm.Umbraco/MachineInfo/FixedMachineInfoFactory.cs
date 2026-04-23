using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection.Infrastructure;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Factories;

namespace Asm.Umbraco.MachineInfo;

/// <summary>
/// Returns a fixed machine identifier so that container restarts
/// don't create new server registrations in the database.
/// </summary>
public class FixedMachineInfoFactory : IMachineInfoFactory
{
    private readonly string _machineName;
    private readonly Lazy<string> _localIdentity;

    /// <summary>
    /// Creates a new <see cref="FixedMachineInfoFactory"/>.
    /// </summary>
    /// <param name="applicationDiscriminator">The Data Protection application discriminator.</param>
    /// <param name="options">The factory options.</param>
    public FixedMachineInfoFactory(
        IApplicationDiscriminator applicationDiscriminator,
        IOptions<FixedMachineInfoFactoryOptions> options)
    {
        var opts = options.Value;
        _machineName = Environment.GetEnvironmentVariable(opts.EnvironmentVariableName) ?? opts.MachineName;
        _localIdentity = new Lazy<string>(() => BuildLocalIdentity(applicationDiscriminator));
    }

    /// <inheritdoc />
    public string GetMachineIdentifier() => _machineName;

    /// <inheritdoc />
    public string GetLocalIdentity() => _localIdentity.Value;

    private static string BuildLocalIdentity(IApplicationDiscriminator applicationDiscriminator)
    {
        using var process = Process.GetCurrentProcess();
        return Environment.MachineName
             + "/" + applicationDiscriminator.Discriminator
             + " [P" + process.Id
             + "/D" + AppDomain.CurrentDomain.Id
             + "] " + Guid.NewGuid().ToString("N").ToUpper();
    }
}
