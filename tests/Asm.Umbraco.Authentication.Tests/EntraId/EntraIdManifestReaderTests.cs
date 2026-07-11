using Asm.Umbraco.Authentication.EntraId;
using Umbraco.Cms.Core;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdManifestReaderTests
{
    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called.
    /// Then it returns exactly one manifest.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncReturnsSingleManifest()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();

        Assert.Single(manifests);
    }

    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called and the single manifest is inspected.
    /// Then its Name is "Asm.Umbraco.Authentication.EntraId".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncManifestNameIsCorrect()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.Equal("Asm.Umbraco.Authentication.EntraId", manifest.Name);
    }

    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called and the single manifest is inspected.
    /// Then its AllowPublicAccess is true.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncManifestAllowsPublicAccess()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.True(manifest.AllowPublicAccess);
    }

    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called and the single manifest is inspected.
    /// Then its Extensions collection is non-null and contains exactly one extension.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncManifestHasExactlyOneExtension()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var manifest = manifests.Single();

        Assert.NotNull(manifest.Extensions);
        Assert.Single(manifest.Extensions);
    }

    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called and the single manifest's single extension is inspected.
    /// Then the extension's type is "authProvider".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncExtensionTypeIsAuthProvider()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        // Extensions are anonymous objects — use dynamic to read properties
        dynamic ext = extension;
        Assert.Equal("authProvider", (string)ext.type);
    }

    /// <summary>
    /// Given an EntraIdManifestReader and the expected provider name derived from the scheme name.
    /// When ReadPackageManifestsAsync is called and the single manifest's single extension is inspected.
    /// Then the extension's forProviderName matches the expected provider name.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncExtensionForProviderNameMatchesSchemeName()
    {
        var expectedProviderName =
            Constants.Security.BackOfficeExternalAuthenticationTypePrefix + EntraIdLoginOptions.SchemeName;

        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        dynamic ext = extension;
        Assert.Equal(expectedProviderName, (string)ext.forProviderName);
    }

    /// <summary>
    /// Given an EntraIdManifestReader.
    /// When ReadPackageManifestsAsync is called and the single manifest's single extension is inspected.
    /// Then the extension's meta.label is "Microsoft".
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReadPackageManifestsAsyncExtensionMetaLabelIsMicrosoft()
    {
        var sut = new EntraIdManifestReader();

        var manifests = await sut.ReadPackageManifestsAsync();
        var extension = manifests.Single().Extensions!.Single();

        dynamic ext = extension;
        Assert.Equal("Microsoft", (string)ext.meta.label);
    }
}
