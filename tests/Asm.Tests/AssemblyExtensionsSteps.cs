using System.Reflection;

namespace Asm.Tests;

[Binding]
public class AssemblyExtensionsSteps
{
    private Assembly _assembly;
    private string _versionString;
    private bool _versionExists;

    [Given(@"I have the current assembly")]
    public void GivenIHaveTheCurrentAssembly()
    {
        _assembly = typeof(AssemblyExtensionsSteps).Assembly;
    }

    [Given(@"I have the Asm library assembly")]
    public void GivenIHaveTheAsmLibraryAssembly()
    {
        _assembly = typeof(AsmException).Assembly;
    }

    [When(@"I get the assembly version")]
    public void WhenIGetTheAssemblyVersion()
    {
        try
        {
            var version = _assembly.Version();
            _versionString = version?.ToString() ?? String.Empty;
            _versionExists = version != null;
        }
        catch
        {
            _versionExists = false;
        }
    }

    [When(@"I get the assembly file version")]
    public void WhenIGetTheAssemblyFileVersion()
    {
        try
        {
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(_assembly.Location);
            _versionString = fileVersionInfo?.FileVersion ?? String.Empty;
            _versionExists = !String.IsNullOrEmpty(_versionString);
        }
        catch
        {
            _versionExists = false;
        }
    }

    [When(@"I get the assembly informational version")]
    public void WhenIGetTheAssemblyInformationalVersion()
    {
        try
        {
            var infoVersion = _assembly.InformationalVersion();
            _versionString = infoVersion ?? String.Empty;
            _versionExists = !String.IsNullOrEmpty(infoVersion);
        }
        catch
        {
            _versionExists = false;
        }
    }

    [Then(@"the version should exist")]
    public void ThenTheVersionShouldExist()
    {
        Assert.True(_versionExists, "Version should be available");
    }

    [Then(@"the version string should not be empty")]
    public void ThenTheVersionStringShouldNotBeEmpty()
    {
        Assert.False(String.IsNullOrEmpty(_versionString), "Version string should not be empty");
    }

    [Then(@"the version should contain a version number")]
    public void ThenTheVersionShouldContainAVersionNumber()
    {
        Assert.Matches(@"\d+\.\d+", _versionString);
    }
}
