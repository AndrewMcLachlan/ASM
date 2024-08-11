using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Asm.AspNetCore.Security;

/// <summary>
/// Azure authentication extensions.
/// </summary>
public static class Authentication
{
    /// <summary>
    /// Adds Azure AD bearer authentication with default options.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> that this method extends.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> so that calls can be chained.</returns>
    public static AuthenticationBuilder AddAzureADBearer(this AuthenticationBuilder builder) => builder.AddAzureADBearer(_ => { });

    /// <summary>
    /// Adds Azure AD bearer authentication.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> that this method extends.</param>
    /// <param name="configureOptions">An action for configuring options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> so that calls can be chained.</returns>
    public static AuthenticationBuilder AddAzureADBearer(this AuthenticationBuilder builder, Action<AzureADBearerOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
        builder.AddJwtBearer();

        return builder;
    }

    private class ConfigureAzureOptions(IOptions<AzureADBearerOptions> azureOptions, IHostEnvironment environment) : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AzureADBearerOptions _azureOptions = azureOptions.Value;

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            options.Audience = _azureOptions.AzureOAuthOptions.Audience;
            options.Authority = _azureOptions.AzureOAuthOptions.Authority;

            if (environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
            }

            options.Events = _azureOptions.Events;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = _azureOptions.AzureOAuthOptions.ValidateAudience,
            };
        }

    }
}
