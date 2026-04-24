using Asm.Umbraco.Authentication.EntraId;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdOptionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void TenantId_CanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-tenant-id", options.TenantId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ClientId_CanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-client-id", options.ClientId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ClientSecret_CanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-client-secret", options.ClientSecret);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Records_WithSameValues_AreEqual()
    {
        var a = new EntraIdOptions
        {
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret"
        };
        var b = new EntraIdOptions
        {
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret"
        };

        Assert.Equal(a, b);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Records_WithDifferentValues_AreNotEqual()
    {
        var a = new EntraIdOptions
        {
            TenantId = "tenant-a",
            ClientId = "client",
            ClientSecret = "secret"
        };
        var b = new EntraIdOptions
        {
            TenantId = "tenant-b",
            ClientId = "client",
            ClientSecret = "secret"
        };

        Assert.NotEqual(a, b);
    }
}
