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
    public AzureOAuthOptions AzureOAuthOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the JWT bearer events.
    /// </summary>
    public JwtBearerEvents Events { get; set; } = new();
}
