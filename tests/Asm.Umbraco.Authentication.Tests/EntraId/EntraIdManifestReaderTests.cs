using Asm.Umbraco.Authentication.EntraId;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdManifestReaderTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ReturnsSingleManifest()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();

        Assert.Single(manifests);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ManifestNameIsCorrect()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.Equal("Asm.Umbraco.Authentication.EntraId", manifest.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ManifestAllowsPublicAccess()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.True(manifest.AllowPublicAccess);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ManifestHasExactlyOneExtension()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.NotNull(manifest.Extensions);
        Assert.Single(manifest.Extensions);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ExtensionTypeIsAuthProvider()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        // Extensions are anonymous objects — use dynamic to read properties
        dynamic ext = extension;
        Assert.Equal("authProvider", (string)ext.type);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ExtensionForProviderNameContainsOpenIdConnect()
    {
        var expectedProviderName =
            Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName;

        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        dynamic ext = extension;
        Assert.Equal(expectedProviderName, (string)ext.forProviderName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsync_ExtensionMetaLabelIsMicrosoft()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        dynamic ext = extension;
        Assert.Equal("Microsoft", (string)ext.meta.label);
    }
}
