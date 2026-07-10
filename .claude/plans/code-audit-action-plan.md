# Code Audit Action Plan

Derived from [docs/code-audit-2026-07-08.md](../../docs/code-audit-2026-07-08.md). Work through phases in order; each phase is independently shippable. Phases 1–4 are non-breaking (patch/minor); phases 5–6 contain breaking changes and must batch into the next major version with explicit approval per repo policy.

**Maintainer direction (2026-07-08):** These are published libraries, so **nothing `public` is ever considered dead** — zero in-repo references does not justify removal, because external consumers are invisible to the repo. Public members are kept, fixed where broken, and given test coverage. "Dead code" removal applies only to non-public code: private/internal members with no callers, never-compiled blocks, .NET 9-specific code (net9 is no longer supported), ghost artifacts, dead branches, commented-out code, orphaned test steps. Broken functionality is always fixed, never removed — the `long` Stream `Read`/`Write` overloads are the canonical example.

**Ground rules for every task** (per AGENTS.md):
- Write the failing test first (Reqnroll BDD for user-facing behavior, focused unit tests for algorithmic logic), then fix.
- Where an existing test *asserts the buggy behavior*, update the test in the same commit and call it out in the commit message.
- Run `dotnet test tests/<Project>.csproj` for the affected project; run full solution tests before merging each phase.
- Keep diffs tight; no formatting-only churn.

---

## Phase 0 — Repo hygiene (no code behavior change, zero risk)

- [ ] Delete ghost directories: `src/Asm.Extensions/`, `src/Asm.OpenTelemetry/`, `src/Asm.Testing/` (verified untracked — bin/obj leftovers only; delete manually, e.g. `rm -rf src/Asm.Extensions src/Asm.OpenTelemetry src/Asm.Testing`).
- [x] Remove `[Asm.Testing]*` exclusion from `coverlet.runsettings`. **Revised:** the `CoverletInclude`/`CoverletExcludeByFile` properties were initially removed as unconsumed, but the maintainer's intent is per-project coverage scoping (a test project's report should not include referenced ASM assemblies at 0%). They are restored, and CI now wires them up: the "Scope coverage" step in `build.yml` reads `CoverletInclude` via `dotnet msbuild -getProperty` and injects it as `<Include>` into the runsettings per matrix project. Verified locally: Asm.Domain.Infrastructure.Tests coverage shrinks from {Asm, Asm.Domain, Asm.Domain.Infrastructure} to {Asm.Domain.Infrastructure}. `CoverletExcludeByFile` remains unwired (its `**/*` value would blank the report).
- [x] Fix `Asm.Reqnroll.csproj` package description (still says "ASM core library unit testing support").
- [x] `src/Asm.Domain.Infrastructure/DbContextQueryHandler.cs`: deleted along with its `<Compile Remove>` line (was excluded from compilation and referenced a removed API shape).
- [x] Delete the `#if NET9_0` block in `OidcSecuritySchemeTransformer.cs` — also removed the then-pointless `#if NET10_0` wrapper.
- [x] Delete the dead private `ByteArray.ToUInt16BE`/`ToUInt16LE` methods (unused duplicates of `ConvertArray` logic).
- [x] Clean up dead branches, expressions, and commented-out code: `Nybble.cs` dead guard, `HostEntry` commented-out logic + unreachable default arm, `Claims.cs` never-null branch, `LoggingConfigurator` duplicate `MinimumLevel` + always-true guard, `BoostrapLoggerFactory` redundant filter, redundant IPv6 regex in `StepArgumentTransformations` (**note:** the multi-capture-group regex was the dead one — the non-capturing-group variant is required by Reqnroll argument binding; the audit had it backwards, and Asm.Net.Tests caught it), commented-out block in `tests/Asm.Cqrs.Tests/Queries/QueryTests.cs`.
- [x] Remove unused `Microsoft.Extensions.Configuration.Binder` reference from Asm.Hosting.
- [x] Docs drift: root README (Asm.Cqrs "built on Mediatr"), `Endian` enum docs (inverted), `Shuffle` docs, `OAuthOptions` doc/mutability mismatches. (`Hosts.SystemHostsFile` docs deferred — Phase 1a fixes the behavior to match the docs instead.)
- [x] Packaging: added `<PackageReadmeFile>` (conditional on README existing). Two audit recommendations overridden by maintainer decision: `PackageIconUrl` is kept alongside `PackageIcon` (some repos still consume it; accept NU5048 on pack), and the rolling `$([System.DateTime]::Now.Year)` copyright is kept (a static year goes stale; the determinism cost only affects rebuilds of old commits across a year boundary).

**Acceptance:** solution builds clean, full test suite green, `dotnet pack` on one project shows README + icon metadata correct.

## Phase 1 — Critical bug fixes: data corruption & silent failure (patch release)

Highest-value fixes; all non-breaking. Test-first for each.

### 1a. Asm.Win32 `Hosts` — DONE
- [x] `WriteHostsToFile(string)`: `File.OpenWrite` → `File.Create` (truncate). Tested: remove entry, save, reload — no trailing garbage.
- [x] Parameterless `WriteHostsToFile()`: writes to `_hostsFile ?? SystemHostsFile`. Tested: custom-path instance saves to its own file, fake system file untouched.
- [x] Single-token line parsing: entry regexes rewritten (`^([^\s#]+)(?:\s+([^#]+))?$` etc.); a bare valid IP loads as an alias-less entry, non-IP tokens still throw `FormatException`.
- [x] `SystemHostsFile` setter now calls `ResetInstance()` (matches docs); `Dispose` clears `_instance`; `ResetInstance` disposes outside the lock.

### 1b. Asm.Testing.Domain — DONE
- [x] Async-enumerator setup made lazy in both `MockDbSet` and `MockDbSetFactory`.
- [x] New `tests/Asm.Testing.Domain.Tests` project (added to slnx + CI matrix): double `ToListAsync`, double `await foreach`, double sync enumeration, projection. Verified failing before fix (3/5), passing after (5/5). Note: CI's `FullyQualifiedName!~Testing` filter clause was removed — it would have excluded this project entirely and no longer filtered anything else.

### 1c. Asm.Logging / Asm.Serilog — DONE
- [x] `BootstrapLoggerFactory.Create` now returns a `BootstrapLogger` (ILogger + IDisposable) that owns the factory and the App/Env scope; disposing flushes providers. README updated to match (was documenting a factory return that never compiled). File renamed `BoostrapLoggerFactory.cs` → `BootstrapLoggerFactory.cs`.
- [x] `MapLogLevel` maps Microsoft names (`Trace`→`Verbose`, `Critical`→`Fatal`, `None`→ above-Fatal silence, unknown → ignored; Serilog names still accepted). Tested with real in-memory configuration across all 9 names (old code crashed on 5 of them).
- [x] `Seq__Host`/`Seq__APIKey` supported alongside `Seq:Host`/`Seq:APIKey` in both libraries; README examples fixed.
- [x] `Path.Combine("logs", "Log.log")` in both overloads.

**Design decisions (settled 2026-07-09):** The disposable-`BootstrapLogger` shape is confirmed — returning a bare `ILoggerFactory` was rejected because the caller always immediately calls `CreateLogger`, so it would only add ceremony for the single realistic consumer. Making the factory the disposable (returning the framework `ILoggerFactory`, or an instantiable factory type) was considered and dropped for the same reason. Because the wrapper flushes on dispose, Seq stays in the bootstrap path (no console-only cut needed).

**Out of scope — separate project:** `Asm.Logging` exists to eventually move the hosting story off Serilog and onto Microsoft.Extensions.Logging. Wiring `WebApplicationStart` to consume `BootstrapLoggerFactory`, and building a MEL equivalent of `LoggingConfigurator` (steady-state enrichment/levels/Seq), is a **separate project**, not part of this audit plan. `BootstrapLoggerFactory` is intentionally left unwired for now. Open question for that project: enrichment parity vs Serilog's `LogContext`. (Note: MEL reads `Logging:LogLevel` natively, so the `MapLogLevel` translation above becomes unnecessary once the steady-state path moves to MEL.)

**Loose end:** `Asm.Logging` still has no test project, so the `BootstrapLogger` fix is currently unverified by a test. A minimal test (Create returns a usable disposable logger; an in-memory provider proves output is not dropped; Dispose flushes) could be added here, or deferred to the separate project above. **Maintainer to decide.**

### 1d. Asm.AspNetCore error pipeline — DONE
- [x] Validation errors dictionary now `property → messages[]` with duplicate-message grouping. Unit + integration tests cover two failures sharing a message (old code crashed in `ToDictionary`).
- [x] Response status set from the computed status in the validation and custom-handler branches. Integration test: `/validation-error` under `UseExceptionHandler` returns wire status 400 (was 500).
- [x] Handler serializes as `object` (preserves `HttpValidationProblemDetails.errors`) with `application/problem+json`.
- [x] Orphaned ValidationException step wired into real scenarios (unit + integration).

### 1e. Asm.AspNetCore misc high/medium — DONE
- [x] `RouteGroupBuilderExtensions`: `GetExecutingAssembly()` → `GetCallingAssembly()` + `[MethodImpl(NoInlining)]`. New scenario tests the parameterless overload maps groups from the calling assembly.
- [x] `CanonicalUrlMiddleware`: redirects now include `PathBase`; the `Directory.Exists`/`File.Exists` URL-path probe is removed (trailing-slash decided from options only); only GET/HEAD are redirected (others pass through). Tests: PathBase case, POST pass-through.
- [x] `GetUserName`: `SingleOrDefault` → `ClaimsIdentity.FindFirst` (single scan, tolerates duplicates). Test: duplicate `name` claims.
- [x] `AddStandardOpenTelemetry` calls `AddHttpContextAccessor()`.
- [x] `ConfigureAzureOptions.Configure` no-ops unless `name == JwtBearerDefaults.AuthenticationScheme`.
- [x] `MapSecurityReporting`: `.AllowAnonymous()` on the group; `ctx.RequestAborted` threaded through `ReadBoundedAsync`.
- [x] `ValidatorFilter`: null argument → 400 `ProblemHttpResult` (was `ArgumentNullException`→500); passes `RequestAborted`. Test: null request → 400, next not called.
- [x] `DataTypeValidator` email regex anchored (`^…$`), classes include `A-Z`, `RegexOptions.IgnoreCase | CultureInvariant`, `IsMatch`. **New `Asm.AspNetCore.Mvc.Tests` project** (slnx + CI matrix); tests: uppercase accepted, embedded substring rejected (old regex failed 4/8).

### 1f. CQRS / Domain high/medium — DONE
- [x] `AddReadOnlyDbContext<TContextService, TContextImplementation>(lifetime)` now forwards `TContextService` (was `IReadOnlyDbContext`). New scenario resolves a custom context interface via the lifetime overload.
- [x] `ControllerName<T>()`: `nameof(T)` → `typeof(T).Name`; test now asserts `"Test"` (fails on old code). File renamed `CommandQueryContoller.cs` → `CommandQueryController.cs` (type name unchanged).
- [x] `IIdentifiable`/`KeyedEntity` equality: `ReferenceEquals` shortcut (reflexive for default IDs), runtime-type check (no cross-type equality), transient entities equal only by reference. New xunit `KeyedEntityEqualityTests` covers default-Id self-equality, cross-type inequality, HashSet round-trip.
- [x] `IIdentifiableEqualityComparer`: `Equals(null, null)` → true; `GetHashCode(null)` → 0. Feature file updated (was encoding the violation).
- [x] `Publisher` publishes sequentially (`foreach await` + `DoNotWrapExceptions`) instead of `Task.WhenAll`.
- [x] `DomainDbContext` drains events until empty (re-snapshot each round) in both sync and async paths; async threads the cancellation token. Publish-before-save ordering retained (full move is Phase 5).
- [x] `Dispatcher` + `Publisher`: `BindingFlags.DoNotWrapExceptions` on every `Invoke`. Test: synchronous handler throw surfaces `InvalidOperationException`, not `TargetInvocationException`.
- [x] Void `Dispatch(ICommand)` detects an argument that also implements `ICommand<T>` and throws a clear `InvalidOperationException` naming `Dispatch<TResponse>`. Test covers the `ICommand`-typed-variable trap.
- [x] Void `MapPutCommand<TRequest>` no longer declares a response status at all (was a mismatched 201). A PUT that isn't a create has an implementer-driven status (200/201/204), so the caller declares it on the returned `RouteHandlerBuilder` via `.Produces(...)`.

### 1g. Asm core (non-breaking subset) — DONE
- [x] `AssemblyVersion`: `Lazy<T>` now takes factory lambdas (was eager); `FileVersion` guards an empty `Assembly.Location` so single-file apps don't throw `TypeInitializationException`. (No test — class is `[ExcludeFromCodeCoverage]` and entry-assembly dependent.)
- [x] `UnitConvert.KilogramsPerPound`: 0.4539 → 0.45359237. UnitConvert feature examples corrected (old values fail at 3 dp).
- [x] `WhereAny`: empty predicates return the source unchanged (documented); materialises once to avoid multiple enumeration. New scenario.
- [x] `ByteArray.GetHashCode`: content + `Endian` hash consistent with `Equals` (was reference-based `base.GetHashCode()`). Tests: equal instances share a hash, different endian differ.
- [x] Claims parsing uses `CultureInfo.InvariantCulture` (int/Guid/ChangeType). No dedicated test (mutating `CurrentCulture` is unsafe under parallel xunit); fix is a clear correctness change.
- [x] `HexColourJsonConverter.Read` throws `JsonException` for a bad string or a non-string token (was `FormatException`/`InvalidOperationException`). Tests cover both.
- [x] Guards: `ByteArray.Copy` bounds fixed (`start + length`, correct ParamName); `ConvertArray` rejects too-short arrays with a clear message; `Squish` reports `fromEnd` (was `fromStart`); `Nybble.ToUInt32` throws for empty and >8 nybbles. Tests cover Squish ParamName and Nybble validation.

### 1h. Reqnroll / Umbraco medium — DONE
- [x] `ScenarioContextExtensions`: `AddResult`/`AddException` use `context.Set` (overwrite) instead of `context.Add` (threw on the second `CatchException` in a scenario). New scenario calls `CatchException` twice.
- [x] `ExceptionSteps`: type resolution falls back to scanning loaded assemblies, so `Asm.NotFoundException` (and other library types) resolve by full name instead of throwing `TypeLoadException`. New scenario.
- [x] `SimpleAssertionSteps`: the string step applies `DecodeWhitespace()` to the expected value (consistent with the exception-message step).
- [x] `ImgSetTagHelper`: path logic extracted to `internal static ToPhysicalPath` (trims both separators — Linux fix — and `IndexOf('?') >= 0`); scaling extracted to `internal static FormatSrcsetEntry` (invariant culture); a decode failure now returns instead of emitting `width="0" height="0"`. `InternalsVisibleTo` added; 4 helper unit tests.

**Acceptance:** every fix has a test that fails before / passes after; full solution green (875 tests across 15 projects); patch-version release notes list each behavioral fix.

**Phase 1 status:** 1a–1h all complete. 1a–1d on branch `audit/phase-1-critical-fixes` (PR #405); 1e–1h on branch `audit/phase-1-remaining` (branched from it).

## Phase 2 — Security hardening (needs a deliberate release; some are behavior changes)

- [ ] `OAuthOptions.ValidateAudience` default `false` → `true`. **Behavioral/breaking for misconfigured consumers — flag in release notes; requires approval.** Update the feature test that enshrines `false`.
- [ ] Entra ID auto-link (`EntraIdLoginOptions`): add `AutoLink`, `DefaultUserGroups` (default Editor for compat), `DenyLocalLogin` options; document the current permissive behavior prominently in the README immediately, even before the options land.
- [ ] `OriginHost` / `CanonicalTagHelper`: stop trusting raw `X-Forwarded-Host`; rely on `Request.Host` + Forwarded Headers middleware.
- [ ] Options validation everywhere: `.AddOptions<T>().Bind(...).Validate(...).ValidateOnStart()` for `OAuthOptions`, `AzureOAuthOptions` (non-empty TenantId, no trailing-slash Domain), `StandardJwtBearerOptions`, `EntraIdOptions`, `FixedMachineInfoFactoryOptions`. Fix `AddAzureOAuthOptions` to register a real named binding (snapshot/monitor safe) instead of a closed `IOptions` singleton.
- [ ] **Do NOT upgrade SixLabors.ImageSharp to 3.x** — v3+ moved to the Six Labors Split License (commercial use requires a paid licence), which is unacceptable for a freely-distributed NuGet library. Stay on the Apache-2.0 2.x line. Instead: keep 2.1.x patched (track 2.x security releases) and, since ImageSharp is only used for header/dimension reads in `ImgSetTagHelper`, consider whether a lighter/again-permissive dependency could replace it long-term.

**Acceptance:** new BDD scenarios: missing OAuth section fails at startup with a clear message; audience validation on by default; forged `X-Forwarded-Host` does not appear in canonical links.

## Phase 3 — The bit/byte family rewrite (Asm core)

`ExtendedBitArray`, `ByteArray`, `Nybble` are wrong for most non-trivial inputs and the tests are happy-path-only. Treat as a rewrite with a test matrix, not spot fixes.

- [ ] Build the test matrix FIRST (Reqnroll scenario outlines): both endiannesses × {1, 8, 9, 16, 32, 64}-bit sizes × {positive, negative, boundary} values; `ICollection.CopyTo`; `Range` indexer incl. `^` indices; round-trips (`bytes → type → bytes`).
- [ ] Fix against the matrix: `ExtendedBitArray.GetBytes` (byte-loop stride), `ToSigned` BE off-by-one, `ToUnsigned` LE truncation, `CopyTo` `NotImplementedException`, `Range`/`IsFromEnd`, constructor endianness consistency; `ByteArray.ToInt16/32/64` sign handling (`unchecked((short)ToUInt16())`); `Nybble.ToNybbles(int)` full 32-bit conversion + consistent nibble ordering.
- [ ] Performance while in there: `1L << j` instead of `Math.Pow`; `BinaryPrimitives`/`ReverseEndianness` instead of LINQ `Reverse()`; avoid boxing in enumeration/`ToString`.
- [ ] Fix the `long` Stream `Read`/`Write` overloads (keep them): track cumulative progress across chunks, size each chunk from the remaining count, use the actual `bytesRead` when copying (never the requested segment size), and terminate correctly for all `offset`/`count` combinations. Tests: `offset > int.MaxValue` with small count; partial reads from a throttled stream; `count` at the boundary. Even though `byte[]` can't exceed `int.MaxValue` today, the methods must be correct for every input they accept.
- [ ] Decide `ByteArray` aliasing semantics (defensive copy vs documented view) — document now, change behavior in Phase 5 if copying.

**Acceptance:** full matrix green; mutation-check a few cases by hand (e.g. `{0xFF,0xFF}.ToInt16() == -1`, 32-bit `GetBytes` round-trip).

## Phase 4 — Performance (non-breaking)

- [ ] `Hosts` parsing: hoist all six regexes to `[GeneratedRegex]` partials.
- [ ] `Dispatcher`/`Publisher`: replace LazyCache+`MethodInfo.Invoke` with `static ConcurrentDictionary<(Type,Type), …>` of compiled delegates; drop the LazyCache package dependency (verify nothing else uses it).
- [ ] `AddAggregateRoots`: hoist `MakeGenericMethod` out of the factory lambda; `Add` → `TryAdd`.
- [ ] `GetUserName` hot path: cache per request in `HttpContext.Items`; single claims scan; avoid the attribute-list copy in `HttpContextLogProcessor` where possible.
- [ ] `ImgSetTagHelper`: `Image.Identify` instead of `Image.Load`; `ProcessAsync`; cache `image.Url()`; read cache in all environments or don't write it in dev; `string.Join` for srcset.
- [ ] `RepositoryBase.Get`: use `FindAsync` (tracked-entity short-circuit) — verify `NotFoundException` semantics preserved.
- [ ] `Entity.Events`: lazy-init the list.
- [ ] Smaller: `HexColour.TryParse` span-based; `IPAddressExtensions` string building; `WriteTo.Trace()` dev-only; cache `AzureOAuthOptions.Authority`; cache security-reporting headers; remove `Hosts` finalizer; `TryGetValue` in `ProblemDetailsFactory`; `IntegrityTagHelper` dev-mode cache churn + derive `$v` from the content hash (also fixes the restart-busting bug).

**Acceptance:** full suite green; optional micro-benchmark for dispatcher before/after.

## Phase 5 — Public API redesign (next MAJOR, requires explicit approval)

Batch all breaking changes into one major release. Draft the list as a GitHub issue for sign-off before starting.

Where a rename is listed, prefer keeping the old name alive as an `[Obsolete]` forwarding shim (old static class delegating to the new one) for one major cycle rather than deleting it outright, except where the duplicate fully-qualified names themselves are the problem (CS0433 collisions), where the colliding name must move.

- [ ] **Unique extension-class names per assembly** — resolve all CS0433 collisions: `IHostApplicationBuilderExtensions` (Asm.AspNetCore vs Asm.Domain.Infrastructure), `IServiceCollectionExtensions` (×4 assemblies), `IUmbracoBuilderExtensions` (×2 Umbraco packages). Convention: `Asm<Package><Target>Extensions`.
- [ ] Drop "I" prefix from static classes; fix `IIdentifiableEqualityComparer` → `IdentifiableEqualityComparer`; file-name typos (`CommandQueryContoller.cs`, `BoostrapLoggerFactory.cs`).
- [ ] **Modules**: static registry → DI (`services.AddSingleton<IModule>`, `MapModuleEndpoints` reads from `app.Services`); additive registration.
- [ ] **ProblemDetailsFactory.AddHandler** → options-based per-container registration.
- [ ] **Hosts**: `Entries` → `IReadOnlyList` + mutation methods; thread-safe refresh; `[SupportedOSPlatform("windows")]`; inconsistent `HostEntry` throws instead of degrading to `Blank`.
- [ ] **CQRS**: distinct method name for void-command dispatch; remove unusable `out` variance; align handler variance; unify `MapCommand` binding semantics (make `CommandBinding` explicit everywhere); decide 200 vs 201 convention for void commands.
- [ ] **Domain events**: dispatch after successful `SaveChanges` (or make ordering configurable); remove sync-over-async path or document it hard.
- [ ] **Repositories**: `TKey` constraints aligned (`notnull`); `AddRange` on `IWritableRepository`; add `Find`/`TryGet` returning `TEntity?`; document `NotFoundException`; relax `ISpecification` to `class`; add `And`/`Or` composition.
- [ ] **Nybble**: make the `+` operators perform arithmetic addition as their documentation promises (behavioral fix, hence major), unify the return types, and add explicit `Combine`/`Append` methods for the current concatenation behavior so it remains available.
- [ ] **Fix and cover the zero-in-repo-reference public surface** — nothing public is dead (library rule); every item below stays, gets fixed where broken, and gets tests:
  - Stream `long` `Read`/`Write` overloads — fixed in Phase 3; keep.
  - `BootstrapLoggerFactory` — fixed in Phase 1c; add tests and keep.
  - `IsDebug` (`IHtmlHelperExtensions`) — replace the pack-time `#if DEBUG` with a runtime check of the consuming app (entry assembly `DebuggableAttribute` or `IHostEnvironment.IsDevelopment()`), so it reflects the consumer's build.
  - `CookieHelper` — fix: `IsEssential = true` (a consent cookie must survive the consent policy), `DateTimeOffset.UtcNow`, explicit `SameSite`; drop the redundant null check; write the missing class XML summary.
  - `RegexActionConstraint` — cache the compiled `Regex` in a field instead of constructing per `Accept` call; `CatchAllAttribute`/`RewriteActionAttribute` — complete or correct their `Accept` logic (only the commented-out leftovers inside them are removable).
  - `AppendIfNotNull`, `FindAsync` (Domain.Infrastructure, both variants) — keep; add test coverage.
  - `GetException<T>` — fix the contract: return null on wrong-type retrieval (`GetException() as T`) instead of a cast failure.
  - `MapMeta`/`MetaModel` — keep; `[Obsolete]` remains the signal, no removal.
  - `BannedWordsValidator(string errorMessage)` ctor — keep; make the misuse trap visible via XML docs and add tests distinguishing the two ctors.
  - Empty `IServiceCollectionExtensions` (Asm.AspNetCore.Api) — keep the type; populate it if/when the package grows registration helpers.
- [ ] Options-pattern conformance: `AddCanonicalUrls(Action<>)` + DI-resolving `UseCanonicalUrls()`; order-independent security headers/reporting via `IPostConfigureOptions`; `AddStandardSecurityHeaders` returns `IServiceCollection`.
- [ ] Consistency: `WebApplicationStart` static + `RunAsync`; dedupe `AppStart.Run`/`RunAsync`; unify `ConfigureLogging` overload semantics on `IConfiguration` (consider Serilog `ReadFrom.Configuration`); `IPAddress` `ToIPAddress(this uint)`; exceptions family (`AsmException` base — use or retire); `GetEnumerator<T>` → `AsGeneric<T>`; nullability annotations (`IsNullOrEmpty` + `[NotNullWhen(false)]`, `DecodeWhitespace`, `GetUserName`); `Page` argument validation; missing Reqnroll numeric steps (`long`, `decimal`, `double`); `MockDbSet<T>` delegates to `MockDbSetFactory`; `WithValidation<T>` locates parameter by type; `WithToolsFromAssemblies` loads referenced assemblies + guards; `EndpointGroupBase` `string?` instead of `String.Empty` sentinels, per-endpoint names, multi-tag support, anonymous-group opt-out; `EntraIdLoginOptions.SchemeName`/callback path rename; `OneOfAuthorisationRequirementHandler` pass `context.Resource`, stop using `context.Fail()` as a veto, and wire the never-called abstract `IsAuthorised` into the evaluation (it was copy-pasted but never invoked — make it functional rather than deleting it); `RouteParamAuthorisationHandler` typed hook.

**Acceptance:** PublicApiAnalyzers (or a manual diff of the `.nupkg` API surface) documenting every rename and behavioral change — **no public-member removals** (renames ship with `[Obsolete]` forwarders; the only old names that disappear are the CS0433-colliding duplicates, which cannot coexist); everything broken-but-kept demonstrably fixed with tests; migration notes in each package README; major version bump via GitVersion.

## Phase 6 — Test-suite hardening (parallel with any phase)

- [ ] Add test projects: Asm.Logging, Asm.Testing.Domain.
- [ ] Convert builder-inspection endpoint tests to executed-endpoint tests (in-memory `WebApplication` or `WebApplicationFactory`) for Asm.Cqrs.AspNetCore and module mapping.
- [ ] Replace Moq'd `IConfiguration` with `ConfigurationBuilder.AddInMemoryCollection` in Asm.Serilog tests.
- [ ] Audit feature files for orphaned step bindings (two known: LoggingConfigurator, ProblemDetailsFactory) and wire or delete them.
- [ ] Add the adversarial cases listed in the audit §6: negative `ByteArray` values, >8-bit `ExtendedBitArray`, `Hosts` shorter rewrite / single-token / custom-path save, duplicate name claims, double `CatchException`, PathBase redirects, `RegisterModules` twice.

---

## Suggested sequencing

1. Phase 0 + 1a–1d in the first PR wave (hygiene + the data-corruption/silent-failure fixes) → patch release.
2. Phase 1e–1h → second patch/minor release.
3. Phase 2 (security) → minor release with prominent notes (audience-validation default flip may warrant coordinating with consuming apps).
4. Phase 3 + 4 → minor release.
5. Phase 5 (+ remaining Phase 6) → next major.
