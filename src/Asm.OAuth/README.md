# Asm.OAuth

The `Asm.OAuth` library provides configuration options and utilities for implementing OAuth authentication in .NET applications. It includes support for standard OAuth flows and Azure Active Directory authentication.

## Features

- **OAuthOptions**: Configuration options for OAuth authentication
- **AzureOAuthOptions**: Specialized configuration for Azure Active Directory (Entra ID)
- **Strongly-Typed Configuration**: Type-safe OAuth configuration using C# records

## Installation

To install the `Asm.OAuth` library, use the .NET CLI:

`dotnet add package Asm.OAuth`

Or via the NuGet Package Manager:

`Install-Package Asm.OAuth`

## Usage

### Standard OAuth Configuration

Configure standard OAuth options:

```csharp
using Asm.OAuth;

var oauthOptions = new OAuthOptions
{
    Domain = "https://your-auth-domain.com",
    Audience = "your-api-audience",
    ClientId = "your-client-id",
    ValidateAudience = true
};

// Use the authority URL
var authority = oauthOptions.Authority; // Returns: "https://your-auth-domain.com"
```

### Azure Active Directory Configuration

Configure Azure AD (Entra ID) OAuth options:

```csharp
using Asm.OAuth;

var azureOptions = new AzureOAuthOptions
{
    Domain = "https://login.microsoftonline.com",
    TenantId = Guid.Parse("your-tenant-id"),
    Audience = "api://your-api-id",
    ClientId = "your-client-id",
    ValidateAudience = true
};

// The authority automatically includes the tenant ID and version
var authority = azureOptions.Authority; 
// Returns: "https://login.microsoftonline.com/{your-tenant-id}/v2.0"
```

### Integration with ASP.NET Core Authentication

Use OAuth options with ASP.NET Core authentication:

```csharp
using Asm.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
var oauthOptions = builder.Configuration.GetSection("OAuth").Get<AzureOAuthOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = oauthOptions.Authority;
        options.Audience = oauthOptions.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = oauthOptions.ValidateAudience,
            ValidAudience = oauthOptions.Audience
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### Configuration File Example

```json
{
  "OAuth": {
    "Domain": "https://login.microsoftonline.com",
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "Audience": "api://your-api-id",
    "ClientId": "your-client-id",
    "ValidateAudience": true
  }
}
```

## Properties

### OAuthOptions

- **Domain**: The OAuth provider domain
- **Audience**: The intended audience for tokens
- **ClientId**: The OAuth client identifier
- **Authority**: The full authority URL (derived from Domain)
- **ValidateAudience**: Whether to validate the token audience

### AzureOAuthOptions

Inherits all properties from `OAuthOptions` and adds:

- **TenantId**: The Azure AD tenant GUID
- **Authority**: Overridden to include tenant ID and v2.0 endpoint

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.