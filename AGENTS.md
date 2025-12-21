@README.md

## About ASM

This solution is a collection of .NET libraries that are packaged as NuGet packages and deployed to the AndrewMcLachlan GitHub package repository.
The libraries are deliberately opinionated, allowing for consuming apps to avoid boilerplate code while creating homogeneity between different apps.

## Core Principles

- Keep the public surface area stable. Breaking changes require explicit approval and semantic versioning updates.
- Prefer expressive, well-tested code over clever abstractions that obscure intent.
- Every new feature or bug fix should include accompanying documentation or samples when discoverability would otherwise suffer.

## Workflow for AI Agents

1. **Review repo instructions first.** Read this file, `README.md`, and any instructions referenced in the global instructions table (e.g., Azure or Postman guidance) before editing.
2. **Understand the target project.** Many libraries sit under `src/`; confirm which project you are changing and inspect its `Directory.Build.*` settings for shared props.
3. **Plan before editing.** Summarize the intended change, confirm with the user when unclear, then prefer `apply_patch` for focused edits.
4. **Respect existing changes.** Never revert or overwrite user edits already present in the worktree unless they explicitly ask.
5. **Keep diffs tight.** Avoid mechanical rewrites or formatting-only changes unless requested.

## Coding Standards

- Follow `.editorconfig` and the C# conventions used throughout the repo (PascalCase types, `_camelCase` private fields, `var` for obvious types, expression-bodied members when they aid clarity).
- Use existing helpers (e.g., under `Asm/Extensions`, `Asm.Testing`, `Asm.Hosting`) before introducing new utilities.
- Validate null arguments and guard application boundaries using the established exception types in `Asm` (e.g., `NotFoundException`, `NotAuthorisedException`).
- Keep logging consistent with `Asm.Serilog` patterns; wire telemetry through the OpenTelemetry helpers when instrumenting new code paths.
- Use framework types (e.g. `Int32`) when calling a static method (e.g. `Int32.Parse` )
- Use built-in types (e.g. `int`) when declaring variables (e.g. `int count = 0;`)
- Fix compiler warnings unless explicitly instructed otherwise
- Use the latest C# language features where possible (e.g., collection initialisers `[]`)

## Testing Guidance

- Add BDD coverage with **Reqnroll** for user-facing behaviors (step definitions live under `tests/**`).
- Add focused unit tests for algorithmic or serialization logic; prefer the shared fixtures in `Asm.Testing` and `Asm.Testing.Domain`.
- Run the relevant test project via `dotnet test tests/<Project>.csproj`; large sweeping changes should execute the full solution tests.
- When editing multi-package features, update both the implementation tests and any dependent package smoke tests.

## Repository Tips

- Shared build metadata lives in `Directory.Build.props`/`.targets`; inspect them before adjusting project files.
- NuGet dependencies are centrally managed through `Directory.Packages.props`; add or update packages there.
- Keep documentation close to the code (e.g., project-level `README.md`). Update those files whenever behavior or configuration changes.
