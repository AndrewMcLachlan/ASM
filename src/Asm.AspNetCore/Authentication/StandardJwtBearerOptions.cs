using Asm.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Asm.AspNetCore.Authentication;

/// <summary>
/// Azure AD bearer token authentication options.
/// </summary>
public record StandardJwtBearerOptions
{
    /// <summary>
    /// Gets or sets the Azure OAuth options.
    /// </summary>
    public required OAuthOptions OAuthOptions { get; set; }

    /// <summary>
    /// Gets or sets the JWT bearer events.
    /// </summary>
    public JwtBearerEvents Events { get; set; } = new();
}
