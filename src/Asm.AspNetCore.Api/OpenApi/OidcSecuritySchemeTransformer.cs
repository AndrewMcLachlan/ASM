#if NET9_0_OR_GREATER
using Asm.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Asm.AspNetCore.Api;

/// <summary>
/// Adds the OIDC security scheme to the Open API document.
/// </summary>
public sealed class OidcSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider, IOptions<AzureOAuthOptions> oAuthOptions) : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["oidc"] = new OpenApiSecurityScheme
                {
                    Name = "oidc",
                    Description = "OIDC",
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri($"{oAuthOptions.Value.TenantId}{oAuthOptions.Value.TenantId}/v2.0/.well-known/openid-configuration"),
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token",
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement()
            {
                { new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference() { Id = "oidc", Type = ReferenceType.SecurityScheme },
                }, new List<string>() },
            });
        }
    }
}
#endif