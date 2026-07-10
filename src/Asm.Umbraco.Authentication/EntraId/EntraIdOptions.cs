using Umbraco.Cms.Core;

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

    /// <summary>
    /// Whether an external Entra ID account is automatically linked to (and, if necessary, creates)
    /// an approved back-office user on first sign-in. Defaults to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// With the default of <see langword="true"/>, any user who can complete the Entra ID sign-in for
    /// the configured tenant becomes an approved back-office user in <see cref="DefaultUserGroups"/>.
    /// Set to <see langword="false"/> to require accounts to be provisioned before they can sign in.
    /// </remarks>
    public bool AutoLink { get; set; } = true;

    /// <summary>
    /// The back-office user group keys assigned to auto-linked users. Defaults to the Editor group.
    /// </summary>
    public IReadOnlyList<string> DefaultUserGroups { get; set; } = [Constants.Security.EditorGroupKey.ToString()];

    /// <summary>
    /// Whether to deny local (username/password) back-office login, permitting only Entra ID.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool DenyLocalLogin { get; set; }
}
