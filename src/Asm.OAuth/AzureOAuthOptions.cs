namespace Asm.OAuth;

/// <summary>
/// OAuth options for Azure Active Directory.
/// </summary>
public record AzureOAuthOptions : OAuthOptions
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The authority for Azure.
    /// </summary>
    public override string Authority => $"{Domain}/{TenantId}/v2.0";
}
