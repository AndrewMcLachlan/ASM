# ASM Solution Code Audit

**Date:** 2026-07-08
**Scope:** All 23 projects under `src/`, their test projects under `tests/`, and the shared build files (`Directory.Build.props`, `Directory.Packages.props`, `src/Directory.Build.props`, `coverlet.runsettings`).
**Method:** Every `.cs` file was read; dead-code claims were verified by repo-wide reference search; bug claims were traced through the actual code paths. Findings are ranked by severity within each section.

## Summary

- **~60 verified bugs**, 15 high severity. Worst clusters: the bit/byte manipulation types in `Asm` (`ExtendedBitArray`, `ByteArray`, `Nybble`) are wrong for most non-trivial inputs; `Hosts` in `Asm.Win32` can corrupt the system hosts file; the ASP.NET Core exception/validation pipeline returns wrong status codes and can crash while handling errors; the bootstrap logger in `Asm.Logging` silently drops all output.
- **Three ghost project directories** (`src/Asm.Extensions`, `src/Asm.OpenTelemetry`, `src/Asm.Testing`) contain no source — only stale build artifacts.
- A recurring pattern: **the test suites encode several bugs as expected behavior** (asserting the buggy output) or assert only `NotNull`.
- Security-relevant: JWT audience validation defaults to off; Entra ID auto-links any tenant user as an approved Editor; `X-Forwarded-Host` is trusted from clients; the canonical-URL middleware probes the server filesystem with URL paths.

> **Maintainer direction (2026-07-08):** as published libraries, **nothing `public` is ever considered dead** — zero in-repo references does not justify removal, since external consumers are invisible here. Public members stay, get fixed where broken, and get test coverage. Dead-code removal applies only to non-public code: private/internal members with no callers, never-compiled blocks, .NET 9-specific code (no longer supported), ghost artifacts, dead branches, commented-out code. Broken functionality is always fixed, never removed (e.g. the `long` Stream `Read`/`Write` overloads get correct chunking logic, not deletion). See `.claude/plans/code-audit-action-plan.md` for the authoritative task list.

---

## 1. High-severity bugs

### Asm (core)

| Where | Bug |
|---|---|
| `src/Asm/ByteArray.cs:151` | `ToInt16/32/64` are implemented as `Convert.ToInt16(ToUInt16())` etc., so **any negative value throws `OverflowException`** — `{0xFF,0xFF}.ToInt16()` should be −1. Tests only use positive values. |
| `src/Asm/ExtendedBitArray.cs:452` | `ICollection.CopyTo` performs the copy **then unconditionally throws `NotImplementedException`**. |
| `src/Asm/ExtendedBitArray.cs:171` | `GetBytes()` loops `i += 8` over the *byte* array, so only byte 0 is ever written — garbage for any array longer than 8 bits. The lone test uses a 4-bit array and passes by accident. |
| `src/Asm/ExtendedBitArray.cs:400` | `ToUnsigned` little-endian branch ignores `bytesToRead`, so the documented truncation never happens — `ToByte()` on a 16-bit array with bit 9 set throws `OverflowException`. |
| `src/Asm/ExtendedBitArray.cs:339` | `ToSigned` big-endian is off by one: it never reads the last bit and mis-weights the sign bit — 8-bit BE `00000001` returns 0 instead of +1. Only little-endian and the value −1 are tested. |
| `src/Asm/Nybble.cs:283` | `ToNybbles(int)` only converts the low 8 bits and returns six always-zero elements; it also orders nibbles opposite to `ToNybbles(byte)`. The test asserts the buggy output. |
| `src/Asm/Extensions/System.IO.Stream.cs:44` | The 64-bit `Read`/`Write` chunking overloads can silently no-op, over-read, copy garbage on partial reads, or **loop forever** — the chunking logic never tracks cumulative progress and ignores the actual bytes read. Fix: track progress across chunks, size each chunk from the remaining count, and copy only `bytesRead`. |

### Asm.Win32

| Where | Bug |
|---|---|
| `src/Asm.Win32/Hosts.cs:160` | `WriteHostsToFile` uses `File.OpenWrite`, which doesn't truncate — writing fewer entries than the file previously held leaves the tail of the old content in place, **corrupting the hosts file** (and the next reload throws `FormatException`). |
| `src/Asm.Win32/Hosts.cs:148` | Parameterless `WriteHostsToFile()` always writes to the *system* hosts file, even for an instance constructed on a custom path — a silent wrong-file write. |
| `src/Asm.Win32/Hosts.cs:276` | A line consisting of a single token (e.g. just `127.0.0.1`) makes the parsing regex backtrack into `IPAddress.TryParse("127.0.0.")` failure → `FormatException`, and the whole file fails to load. |

### Asm.AspNetCore family

| Where | Bug |
|---|---|
| `src/Asm.AspNetCore/Infrastructure/ProblemDetailsFactory.cs:58` | ValidationException errors dictionary is **inverted** (keyed by message, valued by property name) and `ToDictionary` throws on duplicate messages — the error handler itself crashes, returning a bodyless 500 instead of a 400. |
| `src/Asm.AspNetCore/Infrastructure/ProblemDetailsFactory.cs:106` | The 400 set for validation errors is overwritten with the response's current status (500 under `UseExceptionHandler`), so **validation failures reach clients as HTTP 500**. Custom handlers' status codes are likewise never applied to the wire. |
| `src/Asm.AspNetCore/Routing/RouteGroupBuilderExtensions.cs:17` | Parameterless `MapGroups()` scans `Assembly.GetExecutingAssembly()` — Asm.AspNetCore itself, never the consumer's assembly — so it **always registers zero endpoints, silently**. Should be `GetCallingAssembly()`. |
| `src/Asm.AspNetCore/Middleware/CanonicalUrlMiddleware.cs:40,54` | Redirect Location is built from `Request.Path` only, dropping `PathBase` — apps hosted under a path base redirect out of the application. |
| `src/Asm.AspNetCore/Middleware/CanonicalUrlMiddleware.cs:47` | Trailing-slash logic calls `Directory.Exists`/`File.Exists` with a **URL path**, resolving against the process CWD / filesystem root — nondeterministic behavior and a primitive for probing server directory existence. |
| `src/Asm.AspNetCore.Mvc/Extensions/IHtmlHelperExtensions.cs:15` | `Html.IsDebug()` is compiled with `#if DEBUG` **at library pack time** (Release), so it always returns false in consumers' Debug builds. |
| `src/Asm.AspNetCore.Mvc/ModelBinding/Validation/DataTypeValidator.cs:13` | Email regex is lowercase-only without `IgnoreCase` (rejects `USER@EXAMPLE.COM`) and unanchored (accepts `<script>… a@b.co` via substring match). Wrong in both directions. |

### CQRS / Domain

| Where | Bug |
|---|---|
| `src/Asm.Cqrs.AspNetCore/Controllers/CommandQueryContoller.cs:35` | `ControllerName<T>()` uses `nameof(T)`, which is the literal string `"T"` — it returns `"T"` for every controller. A test comment acknowledges this and asserts only `NotNull`. |
| `src/Asm.Domain.Infrastructure/IServiceCollectionExtensions.cs:281` | The `AddReadOnlyDbContext<TContextService, TContextImplementation>(lifetime)` overload forwards `IReadOnlyDbContext` instead of `TContextService` — resolving your own interface throws at runtime. |
| `src/Asm.Domain/KeyedEntity.cs:30` + `src/Asm.Domain/IIdentifiable.cs:18` | Entity equality returns `false` whenever `Id == default` — `e.Equals(e)` is false for an unsaved entity (broken reflexivity; `HashSet` adds the same instance twice). Separately, **two unrelated entity types with the same key value compare equal** (no type check). |

### Logging / Testing

| Where | Bug |
|---|---|
| `src/Asm.Logging/BoostrapLoggerFactory.cs:17` | `using var loggerFactory` disposes the factory before returning the logger built from it — **everything logged through the returned logger is dropped** (Seq sink shut down). The App/Env scope at line 50 is also disposed before return, so it never applies. The README documents a different return type than the code has. |
| `src/Asm.Serilog/LoggingConfigurator.cs:83` | `Logging:LogLevel` values are bound to Serilog's `LogEventLevel`, which has no `Trace`/`Critical`/`None` — a stock appsettings.json (`"Microsoft.Hosting.Lifetime": "None"`) **crashes the host at startup**. |
| `src/Asm.Testing.Domain/MockDbSet.cs:38` + `MockDbSetFactory.cs:36` | The async-enumerator Moq setup returns one pre-built enumerator instance (eager, unlike the lazy sync setup one line above) — the **second `ToListAsync()`/`await foreach` on a mock silently returns empty**, a false-positive machine for consumers' tests. |

---

## 2. Medium/low-severity bugs

### Asm (core)

- `src/Asm/ByteArray.cs:211` — `GetHashCode` is reference-based while `Equals` is `SequenceEqual`: broken as dictionary/set keys.
- `src/Asm/ExtendedBitArray.cs:34` — `Range` indexer ignores `Index.IsFromEnd`: `bits[0..^1]` treats `^1` as `1`.
- `src/Asm/ExtendedBitArray.cs:92` — the `byte[]`/numeric constructors ignore the requested endianness; the `bool[]` constructors honor it — inconsistent bit layouts for the same value.
- `src/Asm/Nybble.cs:67` — `ToUInt32` on >8 nibbles silently returns just `value[0]`; empty array throws `IndexOutOfRangeException`.
- `src/Asm/UnitConvert.cs:63` — `KilogramsPerPound = 0.4539` is a digit transposition of 0.45359237 (0.07% systematic error).
- `src/Asm/AssemblyVersion.cs:16` — `Lazy<T>` constructed with the *value* ctor, so `FileVersionInfo.GetVersionInfo` runs eagerly in the static ctor; in single-file apps every access throws `TypeInitializationException`.
- `src/Asm/Extensions/System.Linq.IQueryable.cs:45` — `WhereAny` with an empty predicate list throws (null lambda body from `Aggregate` with `null!` seed).
- `src/Asm/Extensions/System.Security.Claims.cs:24` — claim parsing uses `CurrentCulture`; `"1.5"` as a decimal claim in de-DE returns 15 or throws.
- `src/Asm/Extensions/System.DateTime.cs:15,92` (duplicated in `System.DateOnly.cs:35`) — `FirstDayOfWeek` underflows near `DateTime.MinValue`; `DifferenceInMonths` rounds asymmetrically (Jan 1→Feb 28 = 2, Jan 15→Feb 10 = 0).
- `src/Asm/ByteArray.cs:95` — `Copy` bounds guard off by one and misattributes length errors to `start`.
- `src/Asm/ByteArray.cs:269` — `ConvertArray` rejects too-long arrays but not too-short (opaque `ArgumentOutOfRangeException` from `BitConverter`).
- `src/Asm/Extensions/System.String.cs:39` — `Squish` reports the wrong `ParamName` for `fromEnd` violations.
- `src/Asm/Drawing/HexColour.cs:188` — JSON converter throws `FormatException`/`InvalidOperationException` instead of `JsonException`; ctor docs claim wrong exception types.
- `src/Asm/Endian.cs:8` — both enum members' docs describe the opposite endianness.

### Asm.Win32

- `src/Asm.Win32/Hosts.cs:42` — `SystemHostsFile` docs claim the setter resets the singleton; it's a plain auto-property.
- `src/Asm.Win32/Hosts.cs:309` — the 10-second poll timer swaps `Entries` on a ThreadPool thread with no synchronization; exceptions in the timer callback are silently swallowed (dead change notifications); disposing `Current` leaves the stale instance cached.
- `src/Asm.Win32/HostEntry.cs:37` — an entry with only `Address` or only `Alias` degrades to `Blank` and is silently deleted on save.

### Asm.AspNetCore family

- `src/Asm.AspNetCore/Modules/Modules.cs:13` — module registry is a mutable static that each `RegisterModules` call *replaces*: second registration's services are added but the first's endpoints are dropped; cross-host global state breaks parallel `WebApplicationFactory` tests. The no-arg overload also scans every loaded assembly (`ReflectionTypeLoadException` risk, misses not-yet-loaded assemblies).
- `src/Asm.AspNetCore/Authentication/Authentication.cs:47` — the JWT configurator ignores the scheme `name`, overwriting every named JwtBearer scheme a consumer adds.
- `src/Asm.AspNetCore/Authentication/StandardJwtBearerOptions.cs:14` — `required OAuthOptions` is bypassed by the options factory → opaque NRE on first authentication when unconfigured.
- `src/Asm.AspNetCore/Extensions/HttpContextExtensions.cs:20` — `GetUserName` uses `SingleOrDefault` on the `name` claim; duplicate name claims make **every request throw** since this runs per log record/activity/request.
- `src/Asm.AspNetCore/Extensions/IHostApplicationBuilderExtensions.cs:41` — `AddStandardOpenTelemetry` registers processors needing `IHttpContextAccessor` but never registers it → startup failure unless the consumer happens to.
- `src/Asm.AspNetCore/Extensions/IApplicationBuilderExtensions.cs:23` — exception handler serializes as declared `ProblemDetails` (dropping `errors` from validation payloads) with `application/json` instead of `application/problem+json`.
- `src/Asm.AspNetCore/Middleware/CanonicalUrlMiddleware.cs:37` — 301s issued for POST etc. (browsers convert to GET, dropping bodies); restrict to GET/HEAD or use 308.
- `src/Asm.AspNetCore/Authorisation/OneOfAuthorisationRequirementHandler.cs:16,25,31` — passes `null` resource to sub-requirements, uses `context.Fail()` as a veto, and declares an abstract `IsAuthorised` that nothing ever calls.
- `src/Asm.AspNetCore/Reporting/IEndpointRouteBuilderExtensions.cs:37` — CSP report endpoints lack `.AllowAnonymous()`; a fallback auth policy silently 401s all browser reports. `ReadBoundedAsync` (line 78) ignores `ctx.RequestAborted`.
- `src/Asm.AspNetCore.Mvc/TagHelpers/IntegrityTagHelper.cs:125,93` — cache-buster derived from randomized `string.GetHashCode()` (changes per process; differs across load-balanced nodes; `Math.Abs(int.MinValue)` overflow); root-relative (`/css/…`) URLs are silently skipped.
- `src/Asm.AspNetCore.Mvc/TagHelpers/LinkTagHelper.cs:34` — hard cast of the `rel` attribute to `HtmlString` throws when supplied as a Razor string expression.
- `src/Asm.AspNetCore.Mvc/Extensions/HttpRequestExtensions.cs:15` — `OriginHost` trusts client-sent `X-Forwarded-Host` (canonical-link/cache poisoning via `CanonicalTagHelper`).
- `src/Asm.AspNetCore.Mvc/Extensions/RazorPageExtensions.cs:27` — `.Result` sync-over-async inside view rendering.
- `src/Asm.AspNetCore.Modules/Routing/EndpointGroupBase.cs:50` — `.WithName(Name)` on the group stamps the same endpoint name on every endpoint in it → `LinkGenerator`/OpenAPI duplicate-name failures; default `Tags` emits an empty OpenAPI tag; base always calls `RequireAuthorization()` so anonymous groups are impossible.
- `src/Asm.AspNetCore/Http/ValidatorFilter.cs:11` — null body → `ArgumentNullException` → 500 instead of 400; no cancellation token; unregistered validator surfaces at request time not startup.
- `src/Asm.AspNetCore.Mvc/TagHelpers/BodyTagHelper.cs:25` — emits leading-space/blank class strings when route values are null.
- `src/Asm.AspNetCore/Infrastructure/ProblemDetailsFactory.cs:22,149` — static handler registry: `Add` throws on re-registration; no synchronization.
- `src/Asm.AspNetCore/Serilog/SerilogEnrichWithUserMiddleware.cs:23` — resolves `IHttpContextAccessor` instead of using its `context` parameter.
- `src/Asm.AspNetCore/WebApplicationStart.cs:43,37` — relies on `Assembly.GetCallingAssembly()` without `[MethodImpl(NoInlining)]`; `Log.Information($"...")` defeats message templates.
- `src/Asm.AspNetCore.Api/OpenApi/OidcSecuritySchemeTransformer.cs:40` — *replaces* `document.Components.SecuritySchemes`, clobbering schemes added by other transformers.

### CQRS / Domain

- `src/Asm.Domain.Infrastructure/Publisher.cs:28` — domain-event handlers run concurrently via `Task.WhenAll` over a shared scoped `DbContext` → EF "second operation on this context" failures. Sequential publish is both safer and cheaper.
- `src/Asm.Domain.Infrastructure/DomainDbContext.cs:44,105` — events publish **before** `SaveChanges` (side effects happen for state that then fails to persist); events raised by handlers are silently deferred to a future save (no drain loop); the sync path does `.GetAwaiter().GetResult()` (deadlock risk, no cancellation).
- `src/Asm.Cqrs/Dispatcher.cs:32,53,73` — `MethodInfo.Invoke` without `BindingFlags.DoNotWrapExceptions` surfaces synchronous handler throws as `TargetInvocationException`.
- `src/Asm.Cqrs/Dispatcher.cs:58` + `src/Asm.Cqrs/Commands/ICommand.cs` — dispatching an `ICommand<T>` through a variable statically typed `ICommand` resolves the void overload and fails at runtime with an unhelpful message.
- `src/Asm.Cqrs/Dispatcher.cs:17` — the `out` variance on `IQuery<>`/`ICommand<>` advertises covariant dispatch that always fails (handler resolved by static type; `ValueTask<T>` cast is invariant anyway).
- `src/Asm.Cqrs.AspNetCore/IEndpointRouteBuilderExtensions.cs:211` — void `MapPutCommand` declares 201 Created but actually returns 200 OK.
- `src/Asm.Cqrs.AspNetCore/Handlers.cs:36,83` — `MapCommand(pattern)` binds via `[AsParameters]` but `MapCommand(pattern, CommandBinding.None)` binds from the body — same "default", two different wire contracts; the enum docs describe only one.
- `src/Asm.Domain/IdentifiableEqualityComparer.cs:21` — `Equals(null, null)` returns false and `GetHashCode(null)` throws, violating the `IEqualityComparer` contract (the test encodes this as expected).
- `src/Asm.Domain/NamedEntity.cs:23` — `CompareTo(null)` returns −1 (contract: non-null sorts greater than null); `CompareTo` uses `Name` while `Equals` uses `Id`.
- `src/Asm.Domain/IQueryableExtensions.cs:18` — `Specify` null-checks a non-nullable parameter; annotate or throw.

### Reqnroll / Umbraco

- `src/Asm.Reqnroll/ScenarioContextExtensions.cs:14,31` — `AddResult`/`AddException` use `context.Add`, so the second `CatchException` in one scenario throws `ArgumentException`; use `Set`.
- `src/Asm.Reqnroll/ExceptionSteps.cs:25` — `Type.GetType(name, true)` can't resolve non-corlib types, so `Then an exception of type 'Asm.NotFoundException'…` — the library's own exception family — throws `TypeLoadException`.
- `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:94` — path fallback trims only `'\\'`, so `Path.Combine` discards `WebRootPath` on Linux and width/height are never emitted.
- `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:72` — srcset scaling formatted with current culture (`1,5x` in de-DE is invalid srcset syntax).
- `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:104` — decode failures swallowed by empty catch, then `width="0" height="0"` emitted.
- `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:95` — `IndexOf('?') > 0` misses a query string at index 0.
- `src/Asm.Reqnroll/SimpleAssertionSteps.cs:23` — string step doesn't `DecodeWhitespace()` though the exception-message step does — same feature syntax, different behavior.

### Hosting / OAuth / Serilog / MCP

- `src/Asm.OAuth/IServiceCollectionExtensions.cs:29` — `AddAzureOAuthOptions` bridges only `IOptions<OAuthOptions>`; `IOptionsSnapshot`/`IOptionsMonitor` resolve unbound instances with null required members; calling both `AddOAuthOptions` and `AddAzureOAuthOptions` silently shadows the former.
- `src/Asm.ModelContextProtocol/IMcpServerBuilderExtensions.cs:16` — `WithToolsFromAssemblies` only sees assemblies already JIT-loaded — tool registration depends on load order, silently skipping not-yet-loaded assemblies.
- `src/Asm.Hosting/AppStart.cs:78` — `ApplicationName` set too late to affect user-secrets loading; the `appBuilder` parameter is unused.
- `src/Asm.Logging/BoostrapLoggerFactory.cs:30` + `src/Asm.Serilog/LoggingConfigurator.cs:35` — Seq host read via a literal `Seq:Host` environment variable, which POSIX shells can't set (README documents an invalid `export`); the standard `Seq__Host` mapping only applies to the config provider — split-brain between bootstrap and steady-state logging.
- `src/Asm.Serilog/LoggingConfigurator.cs:48,104` — Windows-only `logs\Log.log` path separator.
- `src/Asm.Serilog/LoggingConfigurator.cs:22` — `DOTNET_ENVIRONMENT` preferred over `ASPNETCORE_ENVIRONMENT`, inverting ASP.NET Core precedence.
- `src/Asm.OAuth/AzureOAuthOptions.cs:16` — no guard for `Domain` trailing slash or unset `TenantId` (opaque failure at first metadata fetch).

---

## 3. Dead code

### Ghost directories (delete)

- `src/Asm.Extensions/` — only `bin/Debug/net8.0` leftovers; no `.csproj`, not in `Asm.slnx`.
- `src/Asm.OpenTelemetry/` — only `bin/`; the real code lives in `src/Asm.AspNetCore/OpenTelemetry/`.
- `src/Asm.Testing/` — only stale `bin`/`obj` from before the rename to Asm.Reqnroll. Related residue: `coverlet.runsettings:9` still excludes `[Asm.Testing]*`; `src/Asm.Reqnroll/Asm.Reqnroll.csproj:4` still says "ASM core library unit testing support."

### Excluded / never-compiled

- `src/Asm.Domain.Infrastructure/DbContextQueryHandler.cs` — `<Compile Remove>`d in the csproj; wouldn't compile anyway (wrong return type, no Asm.Cqrs reference). Delete, or finish the migration if the handler is still wanted (maintainer call; default delete).
- `src/Asm.AspNetCore.Api/OpenApi/OidcSecuritySchemeTransformer.cs:53-97` — `#if NET9_0` block never compiles; .NET 9 is no longer supported by the libraries. Delete.

### Zero references repo-wide (public NuGet surface — NOT dead per maintainer rule; keep, fix where broken, add coverage)

- `src/Asm.Logging` — the entire `BootstrapLoggerFactory` (also broken; the package has no tests).
- `src/Asm.AspNetCore.Api/IServiceCollectionExtensions.cs` — entirely empty public static class; `MapMeta`/`MetaModel` unreferenced and already `[Obsolete]`.
- `src/Asm.AspNetCore.Mvc` — `CatchAllAttribute`, `RegexActionConstraint`, `RewriteActionAttribute`, `CookieHelper`, `IsDebug`.
- `src/Asm.AspNetCore/Extensions/IHeaderDictionaryExtensions.cs:14` — `AppendIfNotNull`.
- `src/Asm/ByteArray.cs:249` — private `ToUInt16BE`/`ToUInt16LE`.
- `src/Asm.Domain.Infrastructure/DbSetExtensions.cs:23` — internal `FindAsync` kept alive only by its own test; `IQueryableExtensions.cs:19` — public `FindAsync`, unreferenced and untested.
- `src/Asm.Reqnroll/ScenarioContextExtensions.cs:51` — `GetException<T>()`.

### Unreachable / dead branches and residue

- `src/Asm/Extensions/System.IO.Stream.cs:52,105` — 64-bit chunking branches unreachable with valid arguments today (`byte[]` max length), but retained per maintainer direction — the logic must be fixed so it is correct for every accepted input.
- `src/Asm/Nybble.cs:49` — `value < 0` on a `byte` is always false.
- `src/Asm.Win32/HostEntry.cs:144` — unreachable `default` switch arm; commented-out validation logic at lines 41-43, 68-71.
- `src/Asm/Extensions/System.Security.Claims.cs:23` — never-null check on `Claim.Value`.
- `src/Asm.Serilog/LoggingConfigurator.cs:75,30` — always-true null guard; duplicate `MinimumLevel` call.
- `src/Asm.Logging/BoostrapLoggerFactory.cs:41` — filter duplicating `SetMinimumLevel`.
- `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:25` — `ViewContext` property never read (and `required`, so it burdens every construction).
- `src/Asm.Reqnroll/ExceptionSteps.cs:27` — unreachable `Assert.NotNull` after `throwOnError: true`.
- `src/Asm.Reqnroll/StepArgumentTransformations.cs:38` — two overlapping IPv6 regexes; one is redundant.
- `src/Asm.Testing.Domain/TestAsyncQueryProvider.cs:22` — unread ctor parameter forcing three call sites to build throwaway providers.
- `tests/Asm.Cqrs.Tests/Queries/QueryTests.cs:26-44` — commented-out test for a removed API.
- Orphaned test steps masking untested buggy paths: `tests/Asm.Serilog.Tests/Logging/LoggingConfiguratorSteps.cs:68`, `tests/Asm.AspNetCore.Tests/Infrastructure/ProblemDetailsFactorySteps.cs:85`.
- `src/Asm.Hosting/Asm.Hosting.csproj` — unused `Microsoft.Extensions.Configuration.Binder` reference.
- Test csprojs — `CoverletInclude`/`CoverletExcludeByFile` properties consumed by nothing (CI uses the static runsettings).

---

## 4. Memory & performance

Ordered by impact:

1. `src/Asm.Win32/Hosts.cs:209-276` — six `new Regex(...)` constructions **per input line** of the hosts file. Move to `static readonly` or `[GeneratedRegex]` (the repo already uses these elsewhere).
2. `src/Asm.Cqrs/Dispatcher.cs` + `src/Asm.Domain.Infrastructure/Publisher.cs` — every dispatch does a late-bound `MethodInfo.Invoke` (args array + boxed token + boxed `ValueTask`) and a LazyCache lookup with an interpolated key string. LazyCache's ~20-min default expiry periodically evicts permanent metadata, and the keys live in the app's shared `IMemoryCache`. Replace with a `static ConcurrentDictionary<(Type,Type), CompiledHandler>` of `Expression.Lambda`-compiled delegates; removes the LazyCache dependency.
3. `src/Asm.Domain.Infrastructure/IServiceCollectionExtensions.cs:53` — `AddAggregateRoots` runs `MakeGenericMethod` + two reflection invokes on *every* `IQueryable<T>` resolution; hoist to registration time. Also uses `Add` not `TryAdd` (duplicate registrations).
4. `src/Asm.AspNetCore/OpenTelemetry/HttpContextLogProcessor.cs:13` + `HttpContextExtensions.cs:20` — per log record: attribute-list copy plus three LINQ scans over claims. Compute the username once per request (`HttpContext.Items`).
5. `src/Asm.Umbraco/TagHelpers/ImgSetTagHelper.cs:104,87,66` — full synchronous `Image.Load` on the request path (`Image.Identify` reads only the header; `ProcessAsync` avoids sync I/O); dimension cache written but never read in Development; `image.Url()` called up to 4× per render; `srcset +=` in a loop.
6. `src/Asm/ExtendedBitArray.cs:353-419,517,296` — `Math.Pow(2, j)` per set bit (use `1L << j`); enumeration boxes every bit; `ToString` boxes again.
7. `src/Asm.Domain.Infrastructure/RepositoryBase.cs:40` — `Get(id)` always issues SQL via `SingleOrDefaultAsync` even when the entity is tracked; `FindAsync` short-circuits.
8. `src/Asm.Domain/Entity.cs:11` — `Events` eagerly allocates a `List` per materialized entity; lazy-init.
9. `src/Asm/ByteArray.cs:276` — endian swap allocates a LINQ iterator + array per conversion; use `BinaryPrimitives`.
10. `src/Asm/Drawing/HexColour.cs:67-113` — `TryParse` is exception-based with up to 3 intermediate strings; span-based `uint.TryParse(NumberStyles.HexNumber)` is allocation- and exception-free.
11. `src/Asm.Net/IPAddressExtensions.cs:59` — dotted quad built by string concatenation in a loop.
12. `src/Asm.Serilog/LoggingConfigurator.cs:32,72` — `WriteTo.Trace()` in all environments (formatting overhead per event in production).
13. `src/Asm.OAuth/AzureOAuthOptions.cs:16` — `Authority` re-interpolates per access on an immutable record.
14. `src/Asm.AspNetCore/Reporting/SecurityReportingHeaderBuilder.cs:14` — headers rebuilt per response; cacheable per scheme+host. `IEndpointRouteBuilderExtensions.cs:72` — per-report 8 KB buffer + `MemoryStream` + `ToArray()` double copy.
15. `src/Asm.Win32/Hosts.cs:113,325` — needless finalizer (managed resources only).
16. `src/Asm.AspNetCore/Infrastructure/ProblemDetailsFactory.cs:46` — `ContainsKey` + indexer double lookup; use `TryGetValue`.
17. `src/Asm.AspNetCore.Mvc/TagHelpers/IntegrityTagHelper.cs:84,113` — in Development, hash recomputed with blocking I/O every render yet still written to an unbounded cache.
18. `src/Asm.Testing.Domain/TestAsyncQueryProvider.cs:111` — per-execution `Expression.Lambda(...).Compile().DynamicInvoke()`; prefer `Compile(preferInterpretation: true)`.

---

## 5. Public API design

### Cross-cutting themes

1. **Duplicate fully-qualified type names across packages.** `Asm.AspNetCore.Extensions.IHostApplicationBuilderExtensions` exists in both Asm.AspNetCore and Asm.Domain.Infrastructure — any app referencing both gets CS0433 on direct references. `Microsoft.Extensions.DependencyInjection.IServiceCollectionExtensions` exists in four assemblies (Asm.Cqrs, Asm.AspNetCore, Asm.AspNetCore.Api, Asm.OAuth). Both Umbraco packages define `Umbraco.Cms.Core.DependencyInjection.IUmbracoBuilderExtensions`. Adopt unique names per assembly (the convention Asm.Domain.Infrastructure already started).
2. **`required` properties + the options pattern don't compose.** `StandardJwtBearerOptions`, `OAuthOptions`, `EntraIdOptions`, `FixedMachineInfoFactoryOptions` all rely on `required`, which `OptionsFactory`/config binding bypass. Standardize on `.AddOptions<T>().Bind(...).Validate(...).ValidateOnStart()` in every `Add*` extension.
3. **Secure defaults.** `OAuthOptions.ValidateAudience` defaults to `false` (`src/Asm.OAuth/OAuthOptions.cs:41`, consumed by `AddStandardJwtBearer`) — flip to `true` with explicit opt-out. Entra ID auto-link (`src/Asm.Umbraco.Authentication/EntraId/EntraIdLoginOptions.cs:27`) unconditionally makes any tenant user an approved **Editor** with local login left on — add `AutoLink`/`DefaultUserGroups`/`DenyLocalLogin` options. `EntraIdLoginOptions.SchemeName = "OpenIdConnect"` misnames a MicrosoftAccount OAuth handler and squats on `/signin-oidc`.
4. **Mutable static registries → DI.** `Modules.RegisteredModules`, `ProblemDetailsFactory.AddHandler`, and `Hosts.Current`/`SystemHostsFile` are process-global mutable state: order-dependent, not thread-safe, hostile to parallel test hosts. Move to container-scoped options/registrations.
5. **"I"-prefixed static extension classes** read as interfaces; several class names disagree with their file names (`IdentifiableEqualityComparer.cs` contains `IIdentifiableEqualityComparer`) and there are file-name typos (`CommandQueryContoller.cs`, `BoostrapLoggerFactory.cs`). Batch-fix in the next major.

### Type-level suggestions

- **`Nybble` operators** (`src/Asm/Nybble.cs:92-167`): `+` doesn't add — it concatenates nibbles (`5 + 3 == 0x53 == 83`) with three different return types across overloads, while docs say "sum". Fix `+` to perform arithmetic addition per its documentation (major-version behavioral change), unify return types, and add explicit `Combine`/`Append` methods for the concatenation behavior.
- **Fix the `long` Stream `Read`/`Write` overloads** — the chunking logic is broken (see §1); note they also compete in overload resolution with `Stream.Read`, so correct behavior matters even for callers who reach them accidentally via `long` literals.
- **`ByteArray` aliasing** (`src/Asm/ByteArray.cs:34,239`): `GetBytes()`, both implicit conversions, and the indexer share/mutate the internal array — a "readonly struct" that mutates the caller's array. Copy defensively or document view semantics; fix `GetHashCode` alongside; clean up the `Endian = Endian = …` double assignment (line 71).
- **CQRS**: `ICommand<T> : ICommand` plus overloaded `Dispatch` is a misuse trap — separate the void path by name or add a runtime type check with a clear error; drop the unusable `out` variance; make `ICommandHandler`/`IQueryHandler` variance consistent.
- **Repositories**: `IRepository` requires `TKey : struct` but `KeyedEntity<TKey>` accepts string keys — align constraints (`notnull`). `AddRange` exists on `RepositoryWriteBase` but not `IWritableRepository`. `Get` throws `NotFoundException` undocumented — document it and add `TryGet`/`Find` returning `TEntity?`. `ISpecification` requiring `TEntity : Entity` blocks read models; no `And`/`Or` composition exists.
- **Options-pattern conformance**: `UseCanonicalUrls` ignores `Configure<CanonicalUrlOptions>` (always passes `Options.Create`); `AddStandardSecurityHeaders` returns `HeaderPolicyCollection` (breaks chaining) and its coupling with `AddSecurityReporting` is call-order dependent — use `IPostConfigureOptions`.
- **`WithValidation<T>(int parameterIndex)`** (`src/Asm.AspNetCore/Builder/RouteHandlerBuilderExtensions.cs:18`) is fragile — locate the parameter by type at map time and fail fast.
- **Asm.Reqnroll**: numeric assertion steps cover `byte/ushort/uint/ulong` but not `long`, `decimal`, `double`; `DecodeWhitespace` has contradictory nullability (`string` in, `string?` out); `GetException<T>` throws on wrong-type retrieval instead of returning null.
- **`BannedWordsValidator("damn")`** (`src/Asm.AspNetCore.Mvc/ModelBinding/Validation/BannedWordsValidator.cs:29`) binds to the error-message ctor and bans nothing — remove that overload.
- **Asm.Testing.Domain**: `MockDbSet<T>` (constrained to `Entity`) duplicates `MockDbSetFactory` (constrained to `class`) line for line — delegate one to the other so there is one setup path.
- **Asm.Win32**: `Entries` as `IReadOnlyList<HostEntry>` + explicit mutation methods; parameterless save should write to the instance's own path; `SystemHostsFile` setter should actually call `ResetInstance()`; add `[SupportedOSPlatform("windows")]`; inconsistent entries should throw rather than silently degrade to `Blank`.
- **`IPAddressExtensions.FromUInt32`** is a plain static on an extensions class — add `ToIPAddress(this uint)`; simplify `ToCidrString` mask validation with `BitOperations`.
- **Exceptions family**: `AsmException` is a base used by nothing — either derive the domain exceptions from it or retire it; sealing is inconsistent across the family.
- **`GetEnumerator<T>` extension on `IEnumerator`** makes every non-generic enumerator duck-type `foreach`-able with an unchecked cast — rename to `AsGeneric<T>`.
- **Nullability annotations**: `IsNullOrEmpty<T>(IEnumerable<T>)` lacks `?` and none of the three variants has `[NotNullWhen(false)]`; `GetUserName` returns sentinel `"-"` (consider `string?`).
- **Docs drift**: root README says Asm.Cqrs is "built on Mediatr" (MediatR is gone); `Endian` docs inverted; `Shuffle` docs describe in-place reordering but it copies; `Page` accepts non-positive page numbers silently (negative `Skip` → EF runtime error); `WithToolsFromAssemblies` should enumerate referenced assemblies, add null guards, and take an explicit `StringComparison`; support `Seq__Host`.
- **`Asm.Hosting/AppStart`**: `Run`/`RunAsync` are copy-paste duplicates; `WebApplicationStart` is a non-static class with one static method and no `RunAsync`, inconsistent with `WebStart`/`WebJobStart`.

### Build / packaging

- `src/Directory.Build.props:31` — READMEs are packed but `<PackageReadmeFile>` is never set, so galleries never display them.
- `Directory.Build.props:13` — `© 2011-$([System.DateTime]::Now.Year)` breaks deterministic builds at year boundaries. *Maintainer decision: kept — the impact is limited to rebuilding an old commit in a later year, whereas a static year silently goes stale.*
- `src/Directory.Build.props:9` — deprecated `PackageIconUrl` alongside `PackageIcon` (NU5048). *Maintainer decision: kept — some repositories still consume `PackageIconUrl`.*
- `Directory.Packages.props:8-28` — all Microsoft.* versions conditioned on `'$(TargetFramework)' == 'net10.0'`; a second TFM later yields NU1010 across the board.
- `Directory.Packages.props:57` — SixLabors.ImageSharp pinned to 2.1.13 (3.x current; 2.x has needed repeated security patches) and used for request-path image decoding.

---

## 6. Test-suite weaknesses that let this through

- **Tests encoding bugs as expected**: `Nybble.feature` asserts the truncated `2, 1, 0, 0`; `IdentifiableEqualityComparer.feature` asserts `Equals(null,null) == false`; `OAuthOptions.feature` asserts `ValidateAudience` is false; `CommandQueryControllerSteps` acknowledges the `nameof(T)` bug in a comment and asserts only `NotNull`.
- **Route/endpoint tests inspect builders without executing endpoints**, hiding the 201-vs-200 and binding-mismatch bugs.
- **Serilog tests mock `IConfiguration`** instead of using `AddInMemoryCollection`, hiding the startup crash; the log-level-override step binding is orphaned.
- **No test project at all** for Asm.Logging or Asm.Testing.Domain — the two packages with "silently drops output / silently returns empty" bugs.
- **Happy-path-only data**: `ByteArray` conversions never tested with negatives; `ExtendedBitArray` only 4-bit arrays; `Hosts` never rewritten with shorter content, never fed a single-token line, never saved from a custom-path instance; `ImgSetTagHelper`'s entire non-trivial `Process()` path explicitly skipped.
