using Asm.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Asm.AspNetCore.Api;

#if NET10_0
/// <summary>
/// Adds the OIDC security scheme to the Open API document.
/// </summary>
public sealed class OidcSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider, IOptions<OAuthOptions> oAuthOptions) : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(oAuthOptions.Value.Authority))
        {
            return;
        }

        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["oidc"] = new OpenApiSecurityScheme
                {
                    Name = "oidc",
                    Description = "OIDC",
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri($"{oAuthOptions.Value.Authority}/.well-known/openid-configuration"),
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token",
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            // Add security requirement at document root level
            document.Security ??= [];
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oidc", document)] = []
            });
        }
    }
}
#endif

#if NET9_0
using Microsoft.OpenApi.Models;

/// <summary>
/// Adds the OIDC security scheme to the Open API document.
/// </summary>
public sealed class OidcSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider, IOptions<OAuthOptions> oAuthOptions) : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(oAuthOptions.Value.Authority))
        {
            return;
        }

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
                    OpenIdConnectUrl = new Uri($"{oAuthOptions.Value.Authority}/.well-known/openid-configuration"),
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