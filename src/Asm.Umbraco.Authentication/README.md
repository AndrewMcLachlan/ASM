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
    "ClientSecret": "...",
    "AutoLink": true,
    "DefaultUserGroups": [ "..." ],
    "DenyLocalLogin": false
  }
}
```

`TenantId`, `ClientId` and `ClientSecret` are required and validated at startup.

### ⚠️ Auto-linking is permissive by default

By default (`AutoLink = true`), **any** user who can complete Entra ID sign-in for the configured
tenant is automatically linked to (and, if necessary, creates) an **approved** back-office user in
the **Editor** group, and local username/password login stays enabled. For most tenants this means
every tenant member can access the back office.

Harden this to suit your app:

- `AutoLink = false` — require back-office accounts to be provisioned before they can sign in.
- `DefaultUserGroups` — assign a less-privileged group than Editor to auto-linked users.
- `DenyLocalLogin = true` — allow only Entra ID sign-in (disable local login).

### App registration

Register the app in Entra ID with redirect URI `https://<your-host>/signin-oidc`. Grant `openid`, `profile`, and `email` delegated permissions.
