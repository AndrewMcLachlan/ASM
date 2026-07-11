# Migrating to ASM v4.0

v4.0 is a major release that batches the breaking changes from the [code audit](code-audit-2026-07-08.md) (tracked in [#411](https://github.com/AndrewMcLachlan/ASM/issues/411)). This document accumulates the migration notes as each batch lands.

Most changes are **source-compatible for the common case** — the renamed extension classes are only ever called through extension-method syntax (`services.AddX()`), which resolves via the extended type, not the containing class name. You are only affected if you referenced one of these classes **by name**.

## Batch 1 — Extension-class name collisions (CS0433)

Several extension classes shared a type name *and* a namespace across different assemblies, which produces a `CS0433` ("type exists in both …") error in any project that references both packages and names the type. Each has been given a unique, assembly-specific name following the `Asm<Package><Target>Extensions` convention. The namespaces are unchanged, so `using` directives and extension-method call sites do not need to change.

| Assembly | Namespace | Old name | New name |
|---|---|---|---|
| Asm.AspNetCore | `Asm.AspNetCore.Extensions` | `IHostApplicationBuilderExtensions` | `AsmAspNetCoreHostApplicationBuilderExtensions` |
| Asm.Domain.Infrastructure | `Asm.AspNetCore.Extensions` | `IHostApplicationBuilderExtensions` | `AsmDomainInfrastructureHostApplicationBuilderExtensions` |
| Asm.AspNetCore | `Microsoft.Extensions.DependencyInjection` | `IServiceCollectionExtensions` | `AsmAspNetCoreServiceCollectionExtensions` |
| Asm.AspNetCore.Api | `Microsoft.Extensions.DependencyInjection` | `IServiceCollectionExtensions` | `AsmAspNetCoreApiServiceCollectionExtensions` |
| Asm.Cqrs | `Microsoft.Extensions.DependencyInjection` | `IServiceCollectionExtensions` | `AsmCqrsServiceCollectionExtensions` |
| Asm.OAuth | `Microsoft.Extensions.DependencyInjection` | `IServiceCollectionExtensions` | `AsmOAuthServiceCollectionExtensions` |
| Asm.Umbraco | `Umbraco.Cms.Core.DependencyInjection` | `IUmbracoBuilderExtensions` | `AsmUmbracoBuilderExtensions` |
| Asm.Umbraco.Authentication | `Umbraco.Cms.Core.DependencyInjection` | `IUmbracoBuilderExtensions` | `AsmUmbracoAuthenticationBuilderExtensions` |

**What to change:** if you referenced any of these classes by name (e.g. `IServiceCollectionExtensions.AddStandardSecurityHeaders(services)`), update to the new name. If you only ever called the extension methods (`services.AddStandardSecurityHeaders()`), nothing changes.

These renames ship **without `[Obsolete]` forwarders**: the whole point is that the colliding fully-qualified names cannot coexist, and two static classes exposing the same extension methods on the same type would make every call ambiguous (`CS0121`).

## Batch 1b — Drop the `I` prefix from static classes

A leading `I` on a **static** class is misleading (it reads as an interface). All remaining `I`-prefixed extension classes are renamed to the same `Asm<Package><Target>Extensions` convention used in Batch 1a — uniform across the library, and collision-proof (a naive "drop the I" would have re-created `CS0433` for `EndpointRouteBuilderExtensions` and `QueryableExtensions`, which are defined in more than one assembly under a shared namespace). Namespaces are unchanged, so — as in 1a — only *by-name* references need updating; extension-method call sites do not.

| Assembly | Old name | New name |
|---|---|---|
| Asm | `ICollectionExtensions` | `AsmCollectionExtensions` |
| Asm | `IEnumerableExtensions` | `AsmEnumerableExtensions` |
| Asm | `IListExtensions` | `AsmListExtensions` |
| Asm | `IEnumeratorExtensions` | `AsmEnumeratorExtensions` |
| Asm | `IQueryableExtensions` | `AsmQueryableExtensions` |
| Asm.AspNetCore | `IApplicationBuilderExtensions` | `AsmAspNetCoreApplicationBuilderExtensions` |
| Asm.AspNetCore | `IHeaderDictionaryExtensions` | `AsmAspNetCoreHeaderDictionaryExtensions` |
| Asm.AspNetCore | `IEndpointRouteBuilderExtensions` | `AsmAspNetCoreEndpointRouteBuilderExtensions` |
| Asm.AspNetCore.Api | `IEndpointRouteBuilderExtensions` | `AsmAspNetCoreApiEndpointRouteBuilderExtensions` |
| Asm.AspNetCore.Modules | `IEndpointConventionBuilderExtensions` | `AsmAspNetCoreModulesEndpointConventionBuilderExtensions` |
| Asm.AspNetCore.Mvc | `IHtmlHelperExtensions` | `AsmAspNetCoreMvcHtmlHelperExtensions` |
| Asm.Cqrs.AspNetCore | `IEndpointRouteBuilderExtensions` | `AsmCqrsAspNetCoreEndpointRouteBuilderExtensions` |
| Asm.Domain | `IQueryableExtensions` | `AsmDomainQueryableExtensions` |
| Asm.Domain.Infrastructure | `IQueryableExtensions` | `AsmDomainInfrastructureQueryableExtensions` |
| Asm.ModelContextProtocol | `IMcpServerBuilderExtensions` | `AsmModelContextProtocolMcpServerBuilderExtensions` |
| Asm.Net | `IPAddressExtensions` | `AsmNetIPAddressExtensions` |
| Asm.Serilog | `IHostBuilderExtensions` | `AsmSerilogHostBuilderExtensions` |
| Asm.Umbraco | `IPublishedContentExtensions` | `AsmUmbracoPublishedContentExtensions` |

`IPAddressExtensions` keeps its `IP` (it extends `IPAddress`; the `I` is part of the type name, not an interface marker) — so it becomes `AsmNetIPAddressExtensions`, not `AsmNetPAddressExtensions`.

### `IIdentifiableEqualityComparer` → `IdentifiableEqualityComparer`

The equality comparer is a real instantiable class (not an extension class), so it keeps its short name minus the misleading `I`, and — unlike the extension classes — ships **with** an `[Obsolete]` forwarding alias:

```csharp
[Obsolete("Renamed to IdentifiableEqualityComparer …")]
public class IIdentifiableEqualityComparer<TType, TKey> : IdentifiableEqualityComparer<TType, TKey> …
```

Existing `new IIdentifiableEqualityComparer<T, TKey>()` code keeps compiling with a deprecation warning; move to `IdentifiableEqualityComparer<T, TKey>` at your convenience. The alias is removed in the next major.
