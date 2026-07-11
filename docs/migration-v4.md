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
