using Asm.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Asm.Web.Security;

public static class Authentication
{
    public static AuthenticationBuilder AddAzureADBearer(this AuthenticationBuilder builder) => builder.AddAzureADBearer(_ => { });

    public static AuthenticationBuilder AddAzureADBearer(this AuthenticationBuilder builder, Action<AzureADBearerOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
        builder.AddJwtBearer();

        return builder;
    }

    private class ConfigureAzureOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AzureADBearerOptions _azureOptions;
        private readonly IHostEnvironment _environment;

        public ConfigureAzureOptions(IOptions<AzureADBearerOptions> azureOptions, IHostEnvironment environment)
        {
            _azureOptions = azureOptions.Value;
            this._environment = environment;
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            options.Audience = _azureOptions.AzureOAuthOptions.Audience;
            options.Authority = _azureOptions.AzureOAuthOptions.Authority;

            if (_environment.IsDevelopment())
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
