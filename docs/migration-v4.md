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

## Batch 2 — CQRS surface

### Void-command dispatch renamed to `Execute`

`ICommandDispatcher` had two `Dispatch` overloads: `Dispatch<TResponse>(ICommand<TResponse>)` and the void `Dispatch(ICommand)`. Because `ICommand<T>` derives from `ICommand`, a value-returning command held in a variable typed as the non-generic `ICommand` bound silently to the **void** overload — a compile-time trap. The void overload is now:

```csharp
ValueTask Execute(ICommand command, CancellationToken cancellationToken = default);
```

`Dispatch(ICommand)` remains as an `[Obsolete]` default-interface method forwarding to `Execute` for one major cycle, so existing calls keep compiling with a deprecation warning. **What to change:** replace `dispatcher.Dispatch(voidCommand)` with `dispatcher.Execute(voidCommand)`. Value-returning `Dispatch<TResponse>(...)` is unchanged.

### Variance removed from the CQRS interfaces

The covariant/contravariant annotations provided no usable benefit (the dispatcher resolves exact closed generic handler types via DI, where variance does not participate) and were inconsistent between commands and queries. They are removed:

| Interface | Before | After |
|---|---|---|
| `IDispatchable<TResponse>` | `<out TResponse>` | invariant |
| `ICommand<TResponse>` | `<out TResponse>` | invariant |
| `IQuery<TResponse>` | `<out TResponse>` | invariant |
| `ICommandHandler<TCommand, TResponse>` | `<in TCommand, …>` | invariant (aligns with `IQueryHandler`) |

**What to change:** only code that relied on the variance — e.g. assigning an `IQuery<Derived>` to an `IQuery<Base>` variable, or an `ICommandHandler<BaseCommand, …>` where a derived-command handler was expected. Handler registrations and normal dispatch are unaffected.

### Endpoint mapping: consistent binding, no forced status codes

The `Map*` command extensions previously came in pairs (with/without a `CommandBinding`), and the two paths bound differently — the simple overload hard-coded `[AsParameters]` while the binding overload defaulted to body. They are now **single methods** with one consistent, overridable default:

```csharp
MapCommand<TRequest, TResponse>(pattern, CommandBinding binding = CommandBinding.Parameters)
MapCommand<TRequest>(pattern, CommandBinding binding = CommandBinding.Parameters)
// …likewise MapPatchCommand, MapPutCommand, MapPostCreate, MapPutCreate, MapDelete
```

- **Default binding is `CommandBinding.Parameters`** (`[AsParameters]`) everywhere — matching what the old simple overloads did. Pass `CommandBinding.Body` to bind the whole request from the JSON body, or `CommandBinding.None` to let the framework decide.
- **Body-returning commands take an optional `statusCode` (default 200 OK).** `MapCommand<TRequest, TResponse>`, `MapPatchCommand`, `MapPutCommand` and `MapDelete<TRequest, TResponse>` accept `int statusCode = StatusCodes.Status200OK` before `binding`, so a command that returns a body can use e.g. `202 Accepted` — `MapCommand<Cmd, Result>(pattern, StatusCodes.Status202Accepted)`. 200 uses a typed `Ok<T>` result; other codes are emitted via `Results.Json(result, statusCode)`.
- **Void commands take an optional `statusCode` (default 204 No Content).** `MapCommand<TRequest>` / `MapPutCommand<TRequest>` respond with the given code via `Results.StatusCode(statusCode)`, declared through `.Produces(statusCode)` — so the 204 default and the ability to override (e.g. `202 Accepted`) both survive from v3. Create maps still return 201, and `MapDelete` (no response) returns 204.

> **Note on `.Produces(...)`:** it only adds OpenAPI metadata — it does **not** change the runtime status. That's why the status is set inside the handler (`Results.StatusCode` / `Results.Json`) rather than left to the framework default and "documented" with `.Produces`.

**What to change:**
- Void `MapCommand<TRequest>` / `MapPutCommand<TRequest>` are source-compatible with v3 — the status-code parameter is retained (renamed `returnStatusCode` → `statusCode`, same position and `204` default), now with `binding` added after it.
- If you passed `binding` **positionally** to a body-returning map (`MapCommand<Cmd, Result>(pattern, CommandBinding.Body)`), it now needs to be named — `MapCommand<Cmd, Result>(pattern, binding: CommandBinding.Body)` — because `statusCode` is now the second parameter. Everything else is source-compatible (both `statusCode` and `binding` are optional).

## Batch 6 — Nybble arithmetic

> ⚠️ **Silent behavioural change — no compiler will flag most call sites.** The `+` operator on `Asm.Nybble` now performs **arithmetic addition**. In v3 it performed **concatenation** (packing two 4-bit values into a byte). Code that compiles unchanged against v4 will silently compute different results.

### What changed

In v3 the `+` operators (and the underlying `Add` methods) were documented as "adds" but actually **concatenated** — they shifted the left operand up four bits and OR-ed the nybble into the low four bits:

```csharp
// v3
Nybble a = new(0x5), b = new(0x3);
byte result = a + b;   // 0x53 == 83  (concatenation, despite the "Add" name)
```

In v4 `+` does what the documentation always promised — it **adds**:

```csharp
// v4
Nybble a = new(0x5), b = new(0x3);
Nybble result = a + b; // 0x8 == 8   (arithmetic sum)
```

### Overflow decision

`Nybble + Nybble` **wraps modulo 16** (only the low four bits are kept), mirroring how a physical 4-bit register overflows. It never throws and always yields a valid `Nybble`:

```csharp
Nybble max = new(0xF);
Nybble wrapped = max + max;  // 30 & 0xF == 14  (0xE)
```

The integer-mixed operators wrap in the usual unchecked manner for their result type.

### Return types are now coherent (breaking)

Each operator now returns the type that arithmetic addition naturally yields, rather than the old mismatched set (`byte` / `ulong` / `int`):

| Expression        | v3 return (concatenation) | v4 return (arithmetic)      |
|-------------------|---------------------------|-----------------------------|
| `Nybble + Nybble` | `byte`                    | `Nybble` (wraps mod 16)     |
| `uint + Nybble`   | `ulong`                   | `uint`                      |
| `byte + Nybble`   | `int`                     | `int`                       |

The `Nybble.Add(...)` static methods changed correspondingly — they too now add instead of concatenate, matching their long-standing XML docs.

### Keeping the old concatenation behaviour

The v3 concatenation is preserved under explicit, well-named members — replace old `+`/`Add` concatenation call sites with these:

```csharp
Nybble.Combine(high, low);      // two nybbles -> byte  (was: nybbleA + nybbleB)
Nybble.Append(uintValue, n);    // append nybble -> ulong (was: uintValue + nybble)
Nybble.Append(byteValue, n);    // append nybble -> int   (was: byteValue + nybble)
```

`Combine(0x5, 0x3)` returns `0x53` (83) — identical to what `+` produced in v3.

### What to change

- **Audit every `a + b` / `Nybble.Add(...)` involving a `Nybble`.** If you relied on the packing behaviour (building a byte/word out of nybbles), switch to `Nybble.Combine` / `Nybble.Append`.
- If you genuinely wanted addition, you were previously getting the wrong answer — `+` now returns the correct arithmetic sum (and, for two nybbles, a `Nybble` that wraps mod 16).
- Update any variable declarations whose type was inferred from the old return type (e.g. `byte r = a + b;` no longer compiles for two nybbles — the result is now a `Nybble`).

## Batch 4 — Hosts redesign

`Asm.Win32.Hosts` (the Windows hosts-file reader/writer) has been hardened around its entry
collection, made thread-safe on refresh, annotated as Windows-only, and made strict about
malformed input. `HostEntry` and `HostEntryType` are unchanged.

### `Entries` is now a read-only snapshot

`Hosts.Entries` changed from a **mutable** `IList<HostEntry>` to an immutable
`IReadOnlyList<HostEntry>`. Each `get` returns a point-in-time **snapshot**, so it can no longer
be mutated in place, and a reference you captured earlier does not observe later changes.

Direct mutation is replaced by explicit methods on `Hosts`:

| Old (v3) | New (v4) |
|---|---|
| `hosts.Entries.Add(entry)` | `hosts.AddEntry(entry)` |
| `hosts.Entries.Insert(i, entry)` | `hosts.InsertEntry(i, entry)` |
| `hosts.Entries[i] = entry` | `hosts.UpdateEntry(i, entry)` |
| `hosts.Entries.Remove(entry)` | `hosts.RemoveEntry(entry)` |
| `hosts.Entries.RemoveAt(i)` | `hosts.RemoveEntryAt(i)` |
| `hosts.Entries.Clear()` | `hosts.ClearEntries()` |

All mutators guard their arguments (`AddEntry`/`InsertEntry`/`UpdateEntry` throw
`ArgumentNullException` for a null entry; the index-based mutators throw
`ArgumentOutOfRangeException` for an out-of-range index). This is a breaking, **compile-time**
change with no `[Obsolete]` shim — the member type itself changed, so old mutation call sites
will not compile until switched to the methods above. Read access (`Entries[i]`, `.Count`,
enumeration) is unchanged.

### Refresh is now thread-safe

The internal poll timer calls `Refresh()` on a background thread. Previously the reparse cleared
and repopulated the live entry list while callers could be enumerating `Entries`, which could
throw `InvalidOperationException` ("Collection was modified") or observe a half-built list.

Parsing now builds a local list and swaps it into the shared state under a lock, and `Entries`
returns a locked snapshot. Concurrent reads and a concurrent refresh no longer tear or throw.
No API change — existing code simply becomes safe.

### `[SupportedOSPlatform("windows")]`

`Hosts` is now annotated `[SupportedOSPlatform("windows")]` (it targets the Windows hosts file at
`%SystemRoot%\System32\drivers\etc\hosts`). Consumers that call it from code not already scoped to
Windows will get analyzer warning **CA1416**. Guard usage with `OperatingSystem.IsWindows()`, or
annotate the calling member/assembly with `[SupportedOSPlatform("windows")]`. The attribute is
analyzer-only and does not change runtime behaviour, so stream/file-based usage (e.g. in tests)
continues to work cross-platform. `HostEntry`/`HostEntryType` are pure data types and are **not**
annotated.

### Malformed entries throw instead of degrading silently

An inconsistent/malformed host line (a non-blank, non-comment line whose first token is not a
valid IP address) now throws a `FormatException` with an actionable message that names the
offending line **and its line number**, e.g.:

```
Malformed host entry at line 2: 'Wibble'. Expected an IP address followed by an optional alias and comment (e.g. '127.0.0.1 localhost #comment').
```

Previously such input was reported with a terse `Unexpected entry in hosts file: <line>` message
(no line number). The exception **type is unchanged (`FormatException`)**, so `catch` blocks keep
working; only the message text and precision improved. Genuinely blank lines and comment lines
(including `#`-prefixed free text) still parse as `Blank`/`Comment` as before — only truly
malformed address lines throw.

**What to change:** if you asserted on the exact old message text, update it. If you relied on a
malformed line being swallowed, either sanitise the file first or catch `FormatException`.

## Batch 7 — Options-pattern conformance

This batch brings the `Asm.AspNetCore` canonical-URL and security-header/reporting extensions into line with the standard ASP.NET Core options pattern, and makes the reporting↔security-header coupling independent of registration order.

### Canonical URLs: configure via `AddCanonicalUrls`, resolve via DI

`UseCanonicalUrls` no longer builds options inline. Configure `CanonicalUrlOptions` at service-registration time with the new `AddCanonicalUrls`, and call the parameterless `UseCanonicalUrls()`, which resolves `IOptions<CanonicalUrlOptions>` from DI at request time.

```csharp
// Before (v3)
app.UseCanonicalUrls(opts => opts.ForceLowercase = false);

// After (v4)
builder.Services.AddCanonicalUrls(opts => opts.ForceLowercase = false);
// …
app.UseCanonicalUrls();
```

- **New:** `IServiceCollection AddCanonicalUrls(this IServiceCollection, Action<CanonicalUrlOptions>? configure = null)` — registers/configures the options via `services.AddOptions<CanonicalUrlOptions>()`.
- **New:** `IApplicationBuilder UseCanonicalUrls(this IApplicationBuilder app)` — parameterless; resolves the options from DI. With no `AddCanonicalUrls` call the middleware runs with `CanonicalUrlOptions` defaults supplied by the options infrastructure.
- **Obsolete forwarder:** `UseCanonicalUrls(this IApplicationBuilder, Action<CanonicalUrlOptions>? configure)` remains for one major cycle, marked `[Obsolete]`. It still works (builds `Options.Create(...)` inline) so existing call sites keep compiling with a deprecation warning.

**What to change:** move your inline `UseCanonicalUrls(configure)` configuration to `AddCanonicalUrls(configure)` at registration time and switch to the parameterless `UseCanonicalUrls()`.

### `AddStandardSecurityHeaders` now returns `IServiceCollection`

**Breaking signature change.** `AddStandardSecurityHeaders` previously returned the `HeaderPolicyCollection` singleton (so callers could mutate it in place). It now returns `IServiceCollection` for idiomatic chaining, and accepts an optional `Action<HeaderPolicyCollection>` to add or override policies (for example, a Content-Security-Policy).

```csharp
// Before (v3) — mutate the returned collection
var policies = services.AddStandardSecurityHeaders();
policies.AddContentSecurityPolicy(csp => csp.AddDefaultSrc().Self());

// After (v4) — supply a configure callback
services.AddStandardSecurityHeaders(policies =>
    policies.AddContentSecurityPolicy(csp => csp.AddDefaultSrc().Self()));
```

- Signature: `IServiceCollection AddStandardSecurityHeaders(this IServiceCollection services, Action<HeaderPolicyCollection>? configure = null)`.
- The `configure` callback runs after the standard defaults and before the reporting policies are composed.
- The composed `HeaderPolicyCollection` is still directly resolvable from DI (`GetRequiredService<HeaderPolicyCollection>()`) — it is bridged from `IOptions<HeaderPolicyCollection>.Value`, so `UseStandardSecurityHeaders()` and any consumer that resolved the collection continue to work.

**What to change:** if you captured the return value to mutate the collection, move that mutation into the `configure` callback. If you only called `services.AddStandardSecurityHeaders();` and `app.UseStandardSecurityHeaders();`, nothing changes.

### Security-reporting coupling is now order-independent

Previously `AddSecurityReporting` **had to be called before** `AddStandardSecurityHeaders` for the `Reporting-Endpoints` / `Report-To` header policies to be emitted; calling it afterwards silently did nothing. The coupling is now performed by an `IPostConfigureOptions<HeaderPolicyCollection>` registered by `AddSecurityReporting`, which runs after every `Configure` callback that populates the collection. As a result the two registrations compose correctly **in either order**.

```csharp
// Both orders now yield identical results:
services.AddSecurityReporting();
services.AddStandardSecurityHeaders();

// …or…
services.AddStandardSecurityHeaders();
services.AddSecurityReporting();
```

- `AddSecurityReporting` still registers the `SecurityReportingOptions` singleton (so `MapSecurityReporting` continues to require it), and additionally registers `IPostConfigureOptions<HeaderPolicyCollection>`.
- The old order-dependent auto-coupling (a `services.FirstOrDefault(...)` probe inside `AddStandardSecurityHeaders`) has been removed.

**What to change:** nothing is required. Behaviour only improves — reporting headers are now emitted regardless of the order in which the two methods were registered. If your code relied on the old behaviour of *suppressing* reporting headers by deliberately registering reporting last, register the two methods in separate service collections instead.

## Batch 3 — Repositories & specifications

This batch tightens the repository generics, adds ergonomic lookup and bulk-add members, and reworks
`ISpecification<TEntity>` so specifications can target any class and be composed with `And`/`Or`/`Not`.

### Repository key constraint relaxed: `struct` → `notnull`

`IRepository<TEntity, TKey>`, `IWritableRepository<TEntity, TKey>`, `IDeletableRepository<TEntity, TKey>`
and the `RepositoryBase`/`RepositoryWriteBase`/`RepositoryDeleteBase` classes previously constrained
`TKey` to `struct` (non-nullable value types only). They now constrain it to `notnull`, which additionally
permits **reference-type keys** such as `string`.

**What to change:** nothing. `notnull` is a *wider* constraint than `struct`, so every existing value-type
key (`int`, `Guid`, …) still satisfies it and all existing repositories compile unchanged. You may now
declare repositories over string- or record-keyed entities.

### `AddRange` is now on `IWritableRepository`

`RepositoryWriteBase` already exposed `AddRange`; the member is now declared on the
`IWritableRepository<TEntity, TKey>` interface so it can be called through the abstraction. It ships as a
**default interface method** (adds each entity via `Add`), so existing implementers are not broken;
`RepositoryWriteBase` overrides it with the provider-optimised `DbSet.AddRange`.

**What to change:** nothing. Override `AddRange` if you want a bulk-optimised path.

### New null-returning lookups: `Find` and `TryGet`

`Get(TKey …)` throws `NotFoundException` when the entity is absent — unchanged. Two new
**default interface methods** return `TEntity?` (null) instead, for when a missing entity is an expected
outcome:

```csharp
Task<TEntity?> Find(TKey id, CancellationToken ct = default);
Task<TEntity?> Find(TKey id, ISpecification<TEntity> specification, CancellationToken ct = default);
Task<TEntity?> TryGet(TKey id, CancellationToken ct = default);            // alias for Find
Task<TEntity?> TryGet(TKey id, ISpecification<TEntity> specification, CancellationToken ct = default);
```

`RepositoryBase` overrides `Find`/`TryGet` with an efficient keyed query (`SingleOrDefaultAsync`); the
interface default (filtering the full collection) exists only for hand-rolled implementations.

**What to change:** nothing. Prefer `Get` when absence is an error (it throws `NotFoundException`), and
`Find`/`TryGet` when absence is normal (they return `null`).

### `ISpecification<TEntity>` — relaxed constraint, expression criteria, and composition

**Breaking (widening):** the constraint changed from `where TEntity : Entity` to `where TEntity : class`,
so specifications can now target read models, DTOs and projected interfaces — not just domain entities.
`Entity` is a class, so existing entity specifications are unaffected, and the `IQueryable.Specify(…)`
extensions were relaxed to `class` to match.

The interface gained an **expression-based criterion** alongside the existing `Apply`:

```csharp
public interface ISpecification<TEntity> where TEntity : class
{
    Expression<Func<TEntity, bool>> Criteria => static _ => true;      // NEW — the filter predicate
    IQueryable<TEntity> Apply(IQueryable<TEntity> query) => query.Where(Criteria); // now defaulted
    ISpecification<TEntity> And(ISpecification<TEntity> specification); // NEW
    ISpecification<TEntity> Or(ISpecification<TEntity> specification);  // NEW
    ISpecification<TEntity> Not();                                     // NEW
}
```

- `Criteria` is a translatable expression tree, so it round-trips through EF Core.
- `Apply` is now a default method (`query.Where(Criteria)`); override it to add `Include`/`OrderBy`/paging.
- `And`/`Or`/`Not` combine the operands' **`Criteria`** (via `Expression.AndAlso`/`OrElse`/`Not`), rebinding
  the two lambdas onto a single parameter with an `ExpressionVisitor` so the result stays EF-translatable.
  Composition intentionally combines the `Criteria` only — it does **not** merge the `Apply`-side shaping
  (there is no meaningful way to combine, say, two `OrderBy` clauses under an `OR`).

New supporting types (all in `Asm.Domain`):

- `Specification<TEntity>` — an abstract base; override `Criteria` and optionally `Apply`.
- `AsmDomainExpressionExtensions` — public `AndAlso`/`OrElse`/`Not` predicate combinators with parameter
  rebinding, usable directly on `Expression<Func<T, bool>>`.

**What to change:** existing specifications that implement `Apply` continue to work unchanged (their
`Criteria` defaults to "match all", which only affects composition). To make a specification composable,
express its filter as `Criteria` (typically by deriving from `Specification<TEntity>`):

```csharp
public sealed class RecentOrders : Specification<Order>
{
    public override Expression<Func<Order, bool>> Criteria => o => o.OrderDate > DateTime.UtcNow.AddDays(-30);
}

// Compose:
var spec = new RecentOrders().And(new HighValue()).Not();
var results = await repository.Get(spec, cancellationToken);
```

## Batch 9 — Consistency

A grab-bag of consistency and correctness fixes. Additive/annotation-only items (`ToIPAddress`, Reqnroll numeric null transforms, nullability attributes, `MockDbSet` refactor, MCP referenced-assembly loading) are source-compatible and listed briefly at the end; the breaking changes are detailed first.

### `AsmException` is now `abstract`

`Asm.AsmException` is now `abstract` with `protected` constructors. It exists purely as a base so consumers can derive their own exception types and so ASM exceptions can be distinguished from framework ones. The existing concrete ASM exception types (`NotFoundException`, `ExistsException`, `NotAuthorisedException`, `BoundExceededException`) are unaffected — they derive from the most appropriate framework base, not `AsmException`.

**What to change:** stop instantiating `AsmException` directly (`new AsmException(...)` no longer compiles). Derive a concrete type and construct that instead:

```csharp
public sealed class MyException(string message, int errorId) : AsmException(message, errorId);
```

### `EntraId` back-office scheme name and callback path (Umbraco)

The Umbraco Entra ID back-office login provider previously named its scheme `"OpenIdConnect"` and squatted on `/signin-oidc` — the default callback path of the real OpenID Connect handler — even though it uses the OAuth 2.0 `AddMicrosoftAccount` handler, not OIDC. Both have been renamed to provider-specific values:

| | Before | After |
|---|---|---|
| `EntraIdLoginOptions.SchemeName` | `"OpenIdConnect"` | `"EntraId"` |
| `MicrosoftAccountOptions.CallbackPath` | `/signin-oidc` | `/signin-entraid` |

The OAuth `AddMicrosoftAccount` handler is retained (this is **not** a switch to `AddOpenIdConnect`).

**What to change (breaking — `SchemeName` is a `const` so it cannot carry an `[Obsolete]` shim):**
- Update the Entra **app-registration redirect URI** from `https://<host>/signin-oidc` to `https://<host>/signin-entraid`.
- Existing back-office external logins keyed on the old `"OpenIdConnect"` scheme name may need to be **re-linked** (the persisted login provider key changes).

### `EndpointGroupBase` — `Name` removed, nullable sentinels, multiple tags, anonymous opt-out

`Asm.AspNetCore.Routing.EndpointGroupBase` no longer uses `String.Empty` sentinels and now supports richer configuration:

| Member | Before | After |
|---|---|---|
| `Name` | `abstract string` | **removed** |
| `Tags` | `virtual string` (`String.Empty`) | `virtual string[]?` (default `null`) |
| `AuthorisationPolicy` | `virtual string` (`String.Empty`) | `virtual string?` (default `null`) |
| `AllowAnonymous` | — | `virtual bool` (default `false`) |

- **`Name` removed (endpoint names are per-endpoint):** the base previously called `WithName(Name)` on the *group*, which stamps that name onto **every** endpoint in the group — and endpoint names must be globally unique, so any group with more than one endpoint threw `InvalidOperationException` at startup. `WithName` is not an OpenAPI grouping mechanism. Group-level labelling is `Tags` (Swagger UI sections); set endpoint names on individual endpoints inside `MapEndpoints` (`builder.MapGet(...).WithName("GetFoo")`).
- **Multiple tags:** `Tags` is now a `string[]`, applied via `WithTags(params string[])`.
- **Anonymous opt-out:** override `AllowAnonymous => true` to make the whole group anonymous (calls `AllowAnonymous()` instead of `RequireAuthorization`).

**What to change:** remove any `override string Name` from derived groups (move the name to the individual endpoint via `.WithName(...)` if you relied on it). Derived groups that `override string Tags` / `override string AuthorisationPolicy` must update the member type — e.g. `public override string Tags => "a,b";` becomes `public override string[] Tags => ["a", "b"];`. Groups that only override `Path` and `MapEndpoints` are unaffected.

### `IEnumerable<T>.Page` / `IQueryable<T>.Page` now validate arguments

Both `Page` overloads now throw `ArgumentOutOfRangeException` when `pageSize` or `pageNumber` is less than one, and `ArgumentNullException` for a null source/`IPageable`. Previously an invalid page number silently produced the wrong page (a negative `Skip` is ignored) and a negative page size silently returned an empty sequence.

**What to change:** ensure callers pass `pageSize >= 1` and `pageNumber >= 1` (pages are 1-based).

### `IEnumerator.GetEnumerator<T>()` renamed to `AsGeneric<T>()`

The `System.Collections.IEnumerator` extension that adapts to `IEnumerator<T>` was named `GetEnumerator<T>`, which is confusing (it is not the `foreach` pattern method). It is renamed to `AsGeneric<T>`. The old name remains as an `[Obsolete]` forwarder for one major cycle.

**What to change:** replace `enumerator.GetEnumerator<T>()` with `enumerator.AsGeneric<T>()`.

### `RouteHandlerBuilder.WithValidation<T>` locates the parameter by type

`WithValidation<T>()` now finds the argument to validate by its **type** rather than by position. The positional `WithValidation<T>(int parameterIndex)` overload is retained but `[Obsolete]`.

**What to change:** prefer `WithValidation<T>()` (no index). The positional overload still works but is deprecated.

### `OneOfAuthorisationRequirementHandler` — correct any-of semantics

The handler for `OneOfAuthorisationRequirement` had three problems, now fixed:
- It called `context.Fail()` when no sub-requirement passed. In an any-of handler a single failing option must **not** veto the whole requirement, so `context.Fail()` is no longer called — an unmet requirement is denied by the middleware anyway, but other handlers for the same requirement can still succeed.
- It passed `null` as the resource to each sub-check; it now passes `context.Resource` through.
- The `abstract IsAuthorised` hook was never invoked. It is now consulted (with the resource) when none of the sub-requirements succeed, and its signature is `ValueTask<bool> IsAuthorised(object? resource)`.

**What to change:** implementations of `IsAuthorised` should accept `object?` (the resource) and return whether to grant the custom check. Return `false` to preserve pure any-of behaviour.

### `RouteParamAuthorisationHandler` — new strongly-typed variant

A new `RouteParamAuthorisationHandler<TRequirement, TValue>` extracts the route value as a strongly-typed `TValue` (via `TypeConverter`, overridable through `TryConvert`) before calling `ValueTask<bool> IsAuthorised(TValue value)`. The original stringly-typed `RouteParamAuthorisationHandler<TRequirement>` is unchanged.

### Additive / source-compatible changes

- **`Asm.Net`:** added `uint.ToIPAddress()`, the inverse of `IPAddress.ToUInt32()`.
- **`Asm.Reqnroll`:** added `<NULL>` step-argument transforms for `long?`, `decimal?`, and `double?`, mirroring the existing `int?` transform.
- **Nullability:** `IsNullOrEmpty` extensions (enumerable/list/array) are annotated `[NotNullWhen(false)]`; `Asm.Reqnroll` `DecodeWhitespace` is annotated `[return: NotNullIfNotNull]` and accepts `string?`.
- **`WebApplicationStart`:** is now a `static` class and gained `RunAsync`; `AppStart.Run`/`RunAsync` share a single implementation.
- **`Asm.Testing.Domain`:** `MockDbSet<T>` now delegates its setup to `MockDbSetFactory` (no duplicated Moq wiring).
- **`Asm.ModelContextProtocol`:** `WithToolsFromAssemblies` guards null/empty inputs and also loads matching *referenced* assemblies that are not yet loaded.
