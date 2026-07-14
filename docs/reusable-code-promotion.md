# Promoting reusable code from MooBank

Tracking the promotion of genuinely reusable infrastructure that the MooBank application had to
build because ASM lacked it. Driven by MooBank issue
[#886 "Promote Reusable code to ASM library"](https://github.com/AndrewMcLachlan/MooBank/issues/886).

Each candidate below was cross-checked against ASM to confirm it is a real gap (not already provided).
Anything coupled to MooBank's domain (accounts, transactions, budgets, instruments, families, tags) is
deliberately excluded and stays in MooBank.

## Promoted in this branch (Tier 1)

Self-contained, low-risk extractions with no design decisions. All ship with tests.

| Item | New API | Project |
|------|---------|---------|
| Nullable→`required` schema transformer | `OpenApiOptions.AddRequiredForNonNullableProperties()` (`RequiredForNonNullableSchemaTransformer`) | `Asm.AspNetCore.Api` |
| `[DisplayName]` schema reference IDs | `OpenApiOptions.UseDisplayNameSchemaReferenceIds()` (`DisplayNameSchemaReferenceIds`) | `Asm.AspNetCore.Api` |
| Relocate `/api` prefix to a relative server | `OpenApiOptions.RelocatePathPrefixToServer(prefix)` (`ServerPathPrefixDocumentTransformer`) | `Asm.AspNetCore.Api` |
| Document title + version-from-assembly | `OpenApiOptions.AddDocumentInfo(title, assembly?)` (`DocumentInfoTransformer`) | `Asm.AspNetCore.Api` |
| Generic background work queue | `IBackgroundWorkQueue<T>` / `BackgroundWorkQueue<T>` + `IServiceCollection.AddBackgroundWorkQueue<T>()` | `Asm.Hosting` |
| Generic queued hosted worker | `QueuedHostedService<T>` (abstract; override `ProcessAsync`) | `Asm.Hosting` |
| Month-boundary date extensions | `DateOnly`/`DateTime` `ToStartOfMonth()` / `ToEndOfMonth()` | `Asm` |
| Sequential async projection | `IEnumerable<T>.SelectAsync(selector)` | `Asm` |

New extension methods use the C# `extension { }` member syntax, matching the repo's existing style
(`System.DateOnly.cs`).

**Dropped from Tier 1 with cause:** MooBank's OpenAPI "sort tags" transformer was a **no-op** — it
re-added already-present tags to a `HashSet`, which never reorders anything. Rather than promote a
no-op (or a fragile sort over an unordered set), it was omitted. If tag ordering is wanted later, it
needs the document's tag collection to be genuinely ordered first.

### MooBank follow-up (issue #886 step 3)

The MooBank-side duplicates (`src/MooBank.Api/Program.cs` OpenAPI block, `src/MooBank/Queues/*` +
`Services/Background/*`, `src/MooBank/Extensions/DateExtensions.cs`,
`src/MooBank/EnumerableAsyncExtensions.cs`) can be deleted and replaced with these APIs **once a new
ASM version carrying this branch is published**. That bump + deletion is a separate MooBank PR.

## Backlog — Tier 2 (high value, needs a small design decision)

### Integration-test auth harness → new `Asm.Testing.AspNetCore` package
- **From**: `tests/MooBank.Api.Tests/Infrastructure/{TestAuthHandler,MooBankWebApplicationFactory}.cs`
- **What**: A header-driven fake `AuthenticationHandler` that mints a `ClaimsPrincipal` from `X-Test-*`
  headers, plus a `WebApplicationFactory` that (a) swaps the SqlServer `DbContext` for in-memory —
  crucially removing the `IDbContextOptionsConfiguration<T>` descriptor as well as `DbContextOptions`
  — and (b) rebinds the JwtBearer scheme's `HandlerType` via `PostConfigure<AuthenticationOptions>`
  so the **real** authorisation requirement handlers still run under test.
- **Gap**: No `AuthenticationHandler`/`WebApplicationFactory` type anywhere in ASM. `Asm.Testing.Domain`
  only mocks `DbSet`; `Asm.Testing` is an empty shell.
- **To promote**: genericise the concrete `MooBankContext` to `TContext`; make the header→claim map and
  claim-type names injectable; keep MooBank's concrete `TestUser` factories in MooBank.
- **Effort/risk**: Medium; test-only code, low risk.

### Auth cluster (all hinge on one small ASM addition)
- **Denial-audit seam (do first)**: a minimal `IAuthorisationAuditor` (or similar) with a no-op default,
  in `Asm.AspNetCore.Authorisation`. "Audit every authorisation denial once, in one shape" is the
  cross-cutting concern; the only MooBank-specific part of the signature is the `User`, reducible to
  `ClaimsPrincipal`/user-id. This seam is what lets the next two promote without dragging MooBank's
  full `IAuditLogger` along. Note: MooBank's `IAuditLogger` login/import/mutation methods are
  app-specific and must **not** move — only the denial hook is generic.
- **Tolerant (non-vetoing) route handler** → `Asm.AspNetCore.Authorisation`. From
  `src/MooBank.Security/Authorisation/TolerantRouteAuthorisationHandler.cs`. MooBank *forked* ASM's
  `RouteParamAuthorisationHandler` precisely because it always calls `context.Fail()` on a missing
  route value; the tolerant variant fails open on absence (letting resource-based handlers decide) and
  closed on a present-but-invalid value — the same philosophy as ASM's already-generic
  `OneOfAuthorisationRequirementHandler`. Break two couplings: `User?` → `IPrincipalProvider`/bool, and
  `IAuditLogger` → the audit seam above.
- **Auditable `RoleRequirement` + handler** → `Asm.AspNetCore.Authorisation`. From
  `src/MooBank.Security/Authorisation/{RoleRequirement,RoleAuthorisationHandler}.cs`. "Like
  `RequireRole` but denials are logged." Gated on the audit seam; without it, it degenerates into plain
  `RequireRole` and isn't worth promoting. `AdminRequirement` (the concrete subclass) stays in MooBank.

### `IUserIdProvider` + generic settable user provider → `Asm.Security` / `Asm.AspNetCore.Security`
- **From**: `src/MooBank.Domain/Security/IUserIdProvider.cs`, `src/MooBank/Security/IUserDataProvider.cs`,
  `src/MooBank.Security/SettableUserDataProvider.cs`.
- **What**: The "explicitly-set value for background/job processing, else resolve from claims" provider
  pattern, building on ASM's existing `IPrincipalProvider`. `IUserIdProvider` (`Guid CurrentUserId`) is
  trivially generic; promote a generic `ISettableUserDataProvider<TUser>` interface + an abstract base
  with an overridable `MapFromClaims`. The concrete claim URIs and `User` projection stay in MooBank.
- **Effort/risk**: Medium; independently promotable (no dependency on the audit seam).

## Backlog — Tier 3 (low priority / optional)

- **MCP bearer-forwarding + resource-metadata helper** → `Asm.ModelContextProtocol`. From the
  `.AddMcp(...)` block in `Program.cs`. A thin wrapper (validate via existing JwtBearer, keep the
  MCP-owned 401 challenge, advertise protected-resource-metadata); every value is app-specific and
  becomes a parameter. Main value is capturing the hard-won Entra v2 knowledge in the comments
  (AADSTS9010010, resource-must-match-URL).
- **`RequireScope(scope)` authorization helper** → `Asm.AspNetCore` / `Asm.OAuth`. The `scp` /
  long-form scope-claim splitting from the `MapMcp` policy; only the scope string is app-specific.
- **Audit-scope logging helper** → `Asm.Logging` / `Asm.Serilog`. A `logger.BeginAuditScope(category)`
  idiom extracted from `src/MooBank/Audit/AuditLogger.cs`. Low value (one `BeginScope` call); the
  surrounding `IAuditLogger` is MooBank-domain and stays.
- **`HexColourConverter` (EF value converter)** → beside `Asm.Drawing.HexColour` (which already lives in
  ASM), if/when ASM grows an EF value-converter surface. From
  `src/MooBank.Infrastructure/ValueConverters/HexColourConverter.cs`. Niche.

## Considered and rejected

Domain-bound or already-covered: MooBank's `Security`/`ISecurity` assert-and-audit wrapper, `Policies`
and policy-builder helpers, `ClaimTypes`, all Instrument/Group/Family/Budget/Tag/Forecast requirements
and handlers (they correctly derive from ASM's existing bases), `BindHelper.BindWithInstrumentIdAsync`,
`ExistingTagByIdInterceptor`, `CacheKeys`, hand-rolled entity↔DTO `ToModel()` mappers, `MooBankContext`
/ repositories / `AuditingUnitOfWork` (built on `Asm.Domain.Infrastructure` bases), the forced-`https`
middleware (covered by `ForwardedHeaders`), and the `JsonOptions` two-liner.
