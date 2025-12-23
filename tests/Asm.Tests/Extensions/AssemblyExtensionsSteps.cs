#nullable enable
using System.Diagnostics;
using System.Reflection;

namespace Asm.Tests.Extensions;

[Binding]
public class AssemblyExtensionsSteps
{
    private Assembly _assembly = null!;
    private Version _version = null!;
    private FileVersionInfo _fileVersionInfo = null!;
    private string? _informationalVersion;

    [Given(@"I have the current assembly")]
    public void GivenIHaveTheCurrentAssembly()
    {
        _assembly = typeof(AssemblyExtensionsSteps).Assembly;
    }

    [When(@"I call the Version extension method")]
    public void WhenICallTheVersionExtensionMethod()
    {
        _version = _assembly.Version()!;
    }

    [When(@"I call the FileVersion extension method")]
    public void WhenICallTheFileVersionExtensionMethod()
    {
        _fileVersionInfo = _assembly.FileVersion()!;
    }

    [When(@"I call the InformationalVersion extension method")]
    public void WhenICallTheInformationalVersionExtensionMethod()
    {
        _informationalVersion = _assembly.InformationalVersion();
    }

    [Then(@"the version should not be null")]
    public void ThenTheVersionShouldNotBeNull()
    {
        Assert.NotNull(_version);
    }

    [Then(@"the file version info should not be null")]
    public void ThenTheFileVersionInfoShouldNotBeNull()
    {
        Assert.NotNull(_fileVersionInfo);
    }

    [Then(@"the informational version should be a string or null")]
    public void ThenTheInformationalVersionShouldBeAStringOrNull()
    {
        Assert.NotNull(_informationalVersion);
    }
}
