# Asm.Umbraco.Authentication

Back-office authentication helpers for Umbraco CMS.

## Microsoft Entra ID

Adds a Microsoft Entra ID external login provider to the Umbraco back office. Registers an `IPackageManifestReader` so the login button appears in the back-office SPA without any `App_Plugins` folder in the consuming application.

### Usage

```csharp
// Bind options from configuration:
umbracoBuilder.AddEntraIdAuthentication(builder.Configuration.GetSection("EntraId"));

// Or configure inline:
umbracoBuilder.AddEntraIdAuthentication(opts =>
{
    opts.TenantId = "...";
    opts.ClientId = "...";
    opts.ClientSecret = "...";
});
```

### Configuration shape

```json
{
  "EntraId": {
    "TenantId": "...",
    "ClientId": "...",
    "ClientSecret": "..."
  }
}
```

### App registration

Register the app in Entra ID with redirect URI `https://<your-host>/signin-oidc`. Grant `openid`, `profile`, and `email` delegated permissions.
