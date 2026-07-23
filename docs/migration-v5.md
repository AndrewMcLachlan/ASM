# Migrating to ASM v5.0

v5.0 removes `Asm.Cqrs` and `Asm.Cqrs.AspNetCore`. Both are superseded by
[Postie](https://github.com/AndrewMcLachlan/Postie) (`Postie.Cqrs`, `Postie.Cqrs.AspNetCore` — the
mediator and its endpoint-mapping adapter, published to nuget.org) and, for paged query endpoints,
`Asm.AspNetCore.Postie` (`MapPagedQuery`, added in 4.1). There is no in-repo replacement — the CQRS
packages are gone, not renamed.

MooBank has already migrated (the flagship consumer); MooAuth and MooTrack remain on the 4.x line
until they migrate, since they still consume `Asm.Cqrs` directly.

## Removed

| Package | Replacement |
|---|---|
| `Asm.Cqrs` | `Postie.Cqrs` |
| `Asm.Cqrs.AspNetCore` | `Postie.Cqrs.AspNetCore` (endpoint mapping), `Asm.AspNetCore.Postie` (`MapPagedQuery`) |

## Migration recipe

### 1. Swap package references

```diff
- <PackageReference Include="Asm.Cqrs.AspNetCore" />
+ <PackageReference Include="Postie.Cqrs.AspNetCore" />
+ <PackageReference Include="Postie.AspNetCore" />
```

Add `Asm.AspNetCore.Postie` only to projects that map paged queries with `MapPagedQuery`.

### 2. Swap the CQRS global usings

```diff
- global using Asm.Cqrs.Commands;
- global using Asm.Cqrs.Queries;
+ global using Postie.Cqrs.Commands;
+ global using Postie.Cqrs.Queries;
```

`ICommand`, `ICommandHandler`, `IQuery`, `IQueryHandler` and the dispatchers keep the same shapes —
only the namespace changed.

### 3. `MapDelete` → `MapDeleteCommand`

```diff
- builder.MapDelete<DeleteTagPurpose, LogicalAccount>("/{instrumentId}/tag-purposes/{purpose}")
+ builder.MapDeleteCommand<DeleteTagPurpose, LogicalAccount>("/{instrumentId}/tag-purposes/{purpose}")
```

Every other `Map*Command` method keeps its name (`MapCommand`, `MapPatchCommand`, `MapPutCommand`,
`MapPostCreate`, `MapPutCreate`).

### 4. `CommandBinding` → `RequestBinding`

`CommandBinding` (`None` / `Body` / `Parameters`) is replaced by Postie's `RequestBinding` (`Default` /
`Body` / `Parameters`). `None` becomes `Default` — both mean "let the framework infer the binding".

```diff
- builder.MapPostCreate<Create, LogicalAccount>("/", "Get Account".ToMachine(), a => new { instrumentId = a.Id }, CommandBinding.Body)
+ builder.MapPostCreate<Create, LogicalAccount>("/", "Get Account".ToMachine(), a => new { instrumentId = a.Id }, RequestBinding.Body)
```

**Add an explicit `binding: RequestBinding.Parameters`** wherever Asm's binding-less overload used to
default to `[AsParameters]` (`MapCommand`, `MapPatchCommand`, `MapPutCommand`) — Postie's own default for
these is `RequestBinding.Body`, so the behaviour must be pinned explicitly to avoid a silent binding
change:

```diff
- builder.MapPatchCommand<Update, LogicalAccount>("/{id}")
+ builder.MapPatchCommand<Update, LogicalAccount>("/{id}", binding: RequestBinding.Parameters)
```

Audit every call site that omitted a binding argument — this is a **compiling, silent** behaviour
change if skipped.

### 5. `MapPagedQuery` moves to `Asm.AspNetCore.Postie`

Paged query endpoints keep the same call shape; only the package changes. Add
`using Asm.AspNetCore.Postie;` alongside the `using Postie.AspNetCore;` the rest of the endpoint file
already needs.

### 6. Handler registration

`AddPostie<TMarker>()` / `AddPostie(assemblies)` register both the command/query handlers and the
endpoint dispatcher in one call — most consumers only need this. If handlers are registered
granularly instead (`AddCommandHandlers`/`AddQueryHandlers` per assembly/module), also call
`services.AddPostieEndpointDispatcher()` once — it is not implied by the granular registration
methods, only by `AddPostie`:

```csharp
services.AddPostieEndpointDispatcher();
```

### 7. `CommandQueryController` has no replacement

`CommandQueryController` (the MVC base class binding controller actions to CQRS commands/queries) is
**removed with no direct replacement** — Postie's endpoint mapping is minimal-API-only. Check for
usage before migrating a consumer; a controller-based consumer needs to move the affected actions to
minimal-API endpoints (or keep its own hand-rolled MVC binding) as part of the migration.

## What to change

- Update package references per the table above.
- Swap `Asm.Cqrs.Commands`/`Asm.Cqrs.Queries` global usings for `Postie.Cqrs.Commands`/`Postie.Cqrs.Queries`.
- Rename `MapDelete` calls to `MapDeleteCommand`.
- Replace `CommandBinding` with `RequestBinding` (`None` → `Default`); add explicit
  `RequestBinding.Parameters` wherever the old binding-less overload relied on Asm's `[AsParameters]`
  default.
- Move `MapPagedQuery` usage onto `Asm.AspNetCore.Postie`.
- Add `services.AddPostieEndpointDispatcher()` if registering handlers via `AddCommandHandlers`/
  `AddQueryHandlers` instead of `AddPostie`.
- Search for `CommandQueryController` usage before migrating; there is no replacement.
