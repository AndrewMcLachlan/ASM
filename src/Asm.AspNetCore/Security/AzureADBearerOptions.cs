using Asm.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Asm.AspNetCore.Security;

/// <summary>
/// Azure AD bearer token authentication options.
/// </summary>
public record AzureADBearerOptions
{
    /// <summary>
    /// Gets or sets the Azure OAuth options.
    /// </summary>
    public required AzureOAuthOptions AzureOAuthOptions { get; set; }

    /// <summary>
    /// Gets or sets the JWT bearer events.
    /// </summary>
    public JwtBearerEvents Events { get; set; } = new();
}
