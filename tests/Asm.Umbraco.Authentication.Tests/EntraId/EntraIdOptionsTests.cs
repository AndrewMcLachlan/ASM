using Asm.Umbraco.Authentication.EntraId;

namespace Asm.Umbraco.Authentication.Tests.EntraId;

public class EntraIdOptionsTests
{
    /// <summary>
    /// Given an EntraIdOptions initialised with a TenantId.
    /// When the TenantId property is read.
    /// Then it returns the value that was set.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TenantIdCanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-tenant-id", options.TenantId);
    }

    /// <summary>
    /// Given an EntraIdOptions initialised with a ClientId.
    /// When the ClientId property is read.
    /// Then it returns the value that was set.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ClientIdCanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-client-id", options.ClientId);
    }

    /// <summary>
    /// Given an EntraIdOptions initialised with a ClientSecret.
    /// When the ClientSecret property is read.
    /// Then it returns the value that was set.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ClientSecretCanBeSetAndRead()
    {
        var options = new EntraIdOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        Assert.Equal("test-client-secret", options.ClientSecret);
    }

    /// <summary>
    /// Given two EntraIdOptions records with identical values and a shared DefaultUserGroups instance.
    /// When they are compared for equality.
    /// Then they are considered equal.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RecordsWithSameValuesAreEqual()
    {
        // DefaultUserGroups is a reference-type collection, so record equality requires the same
        // instance (C# records compare members by their own Equals). Share it to compare the rest.
        string[] groups = ["editor"];

        var a = new EntraIdOptions
        {
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret",
            DefaultUserGroups = groups
        };
        var b = new EntraIdOptions
        {
            TenantId = "tenant",
            ClientId = "client",
            ClientSecret = "secret",
            DefaultUserGroups = groups
        };

        Assert.Equal(a, b);
    }

    /// <summary>
    /// Given two EntraIdOptions records that differ by TenantId.
    /// When they are compared for equality.
    /// Then they are considered not equal.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RecordsWithDifferentValuesAreNotEqual()
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
