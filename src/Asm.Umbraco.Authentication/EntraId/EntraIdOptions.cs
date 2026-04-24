namespace Asm.Umbraco.Authentication.EntraId;

/// <summary>
/// Microsoft Entra ID application settings used when configuring Umbraco back-office login.
/// </summary>
public record EntraIdOptions
{
    /// <summary>
    /// The Entra ID tenant identifier (GUID) the application is registered against.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// The Entra ID application (client) identifier.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The Entra ID application client secret.
    /// </summary>
    public required string ClientSecret { get; set; }
}
