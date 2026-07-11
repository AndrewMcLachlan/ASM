# Release Branching & Versioning Strategy (Proposal)

> **Status:** agreed design for **ASM**, not yet implemented. Intended to also form the
> basis of the equivalent overhaul for **MooApp**, so the two share one strategy.

## Context

ASM publishes NuGet packages to GitHub Packages from CI. Today the workflow **publishes on
every push to `main`** (`build.yml` publish job `if: … github.ref == 'refs/heads/main'`), and
the version is *inferred from timing* — `major.minor.commitsSinceVersionSource`, with
`major.minor` floored by a hardcoded `next-version` in `GitVersion.yml`. GitVersion has **no
`release` branch type** configured.

Problems this causes:

- A routine merge to `main` can publish a **mislabelled** version (e.g. a breaking change
  shipping as `3.7.x`), because nothing ties the version to intent.
- Cutting a new major/minor requires fragile manual tag/bump choreography (a tag must exist on
  `main`'s HEAD *before* the triggering push, or a wrong version ships).
- `AssemblyVersion` is hardcoded and stale (`3.0.0.0`), independent of the package version.

### Goals

- Semantic Versioning (`Major.Minor.Patch`).
- **Major/Minor chosen deliberately** by the developer (by creating a release branch).
- **Patch increments automatically** for every change.
- No ever-increasing build numbers; no "empty" version-bump publishes.
- The correct major/minor exists **before** new functionality is published.
- **One supported version at a time** (older lines are not maintained) — which lets the model
  collapse to a single trunk.
- Routine flow (patches, Dependabot) is **fully automatic with zero config to maintain**.

## Target model — the release branch *is* the trunk

- The **current release branch `release/X.Y`** (no `v` prefix) is the **repository default
  branch** and the trunk: PRs and Dependabot land there (default → zero Dependabot config), CI
  runs there, and each push publishes stable `X.Y.<patch>`. **There is no separate long-lived
  `main`.**
- **Each release line is anchored by a tag `X.Y.0`** on an empty "Start X.Y" commit *unique to its
  branch* (created by the Release workflow). GitVersion reads **Major.Minor from that anchor tag**
  and the **patch/beta counter from the commit count since it** (`CommitsSinceVersionSource`), which
  resets per line. Because the anchor commit exists only on its own branch, **lines are isolated** —
  a `5.0` beta never affects the live `4.x` line, even while both receive commits. (GitVersion cannot
  derive a rolling version from a two-part `release/X.Y` name; only a tag / `next-version` / full
  `X.Y.Z` name seeds it — verified against the GitVersion docs and locally.)
- **Only `release/**` branches publish.** PRs run CI (build + test) but never publish.

| Branch | Version produced | Publishes | Default branch |
|---|---|---|---|
| `release/4.0` | `4.0.x` stable | ✅ | ✅ (current) |
| `release/5.0-beta` | `5.0.0-beta.N` (prerelease) | ✅ (prerelease) | ❌ |
| `release/5.0` (after graduation) | `5.0.x` stable | ✅ | ✅ (new) |
| `feature/*`, PRs | CI prerelease (unpublished) | ❌ | ❌ |

**Cutting a new minor** (non-breaking, e.g. 5.1): create `release/5.1`, flip the default branch
→ publishes `5.1.x`.

**Cutting a new major** (breaking, e.g. 5.0): create `release/5.0-beta` → publishes
`5.0.0-beta.N` while you iterate (stable `4.0` consumers unaffected). When ready, a **"Release"
workflow** renames `release/5.0-beta` → `release/5.0` (GitHub rename-branch API, which
auto-retargets open PRs) **and sets it as the default branch** → publishes stable `5.0.0`;
`release/4.0` retired.

The only deliberate acts are **creating the next branch and flipping the default** — i.e. the
"choose major/minor deliberately" step, which is rare. Everything routine is automatic.

## Changes

### 1. `GitVersion.yml` — anchor-tag versioning

- A single `release` branch type matching `release/X.Y` and `release/X.Y-beta`
  (`^releases?[/-]v?\d+\.\d+(-beta)?$`), `mode: ContinuousDeployment`, `is-release-branch: true`,
  `increment: Patch`, `label: ''`. Major.Minor comes from the per-line anchor tag, not the name.
- `assembly-versioning-scheme: Major` → `AssemblyVersion` = `X.0.0.0`.
- **No `next-version`** (anchor tags supersede it).
- Verified locally with `dotnet-gitversion` across `release/4.0`, `release/5.0-beta`, `feature/*`.

### 2. `.github/workflows/build.yml`

- **Triggers:** `push: branches: ['release/**']` (the publish path) + `pull_request` (CI only) +
  `workflow_dispatch`. Drop the `main`-only push trigger.
- **Version step:** derive from GitVersion (`majorMinorPatch`, or `semVer` for prerelease
  branches) instead of the `main`-conditional formatting.
- **Publish job `if:`** → `startsWith(github.ref, 'refs/heads/release/')`. Publishes stable from
  `release/X.Y` and prereleases from `release/X.Y-beta`; PRs never publish.
- **Pack step:** add `--p:AssemblyVersion=<major>.0.0.0` (from the GitVersion `major` output) —
  pins the binding version per major line.

### 3. `.github/dependabot.yml`

- **No change.** Leave `target-branch` unset so it follows the repo default branch — which,
  after the transition, is `release/4.0`, and moves automatically whenever the "Release"
  workflow flips the default.

### 4. `Directory.Build.props`

- Remove the hardcoded `<AssemblyVersion>3.0.0.0</AssemblyVersion>` (now supplied at pack time
  as `<major>.0.0.0`); optionally keep a local-dev fallback.

### 5. New `.github/workflows/release.yml` (workflow_dispatch)

A one-click "start/graduate a version" workflow so there is **no manual git**. Inputs: `version`
(e.g. `5.0`) and `action`:

- **`start-beta`:** branch `release/X.Y-beta` from the current default, add an empty `Start X.Y`
  commit, tag it `X.Y.0`, push both → publishes `X.Y.0-beta.N`. Default **unchanged** (the beta runs
  alongside the current stable line).
- **`start-stable`:** same, on `release/X.Y`, and set it as the default branch (a new minor line).
- **`graduate`:** rename `release/X.Y-beta` → `release/X.Y` (GitHub rename-branch API) and set it as
  the default branch. The anchor tag is already on the branch, so versioning continues as `X.Y.N`.
- **Auth:** rename + default-branch change need a **PAT or GitHub App token** in a `RELEASE_TOKEN`
  secret (the stock `GITHUB_TOKEN` cannot do either).

## Transition (current ASM state → new model)

Current state: `main` is behind; all v4 work **and the open v4 PRs** live on `release/v4.0`.

1. Merge the open v4 PRs into `release/v4.0` (done — #417/#420/#421 merged).
2. Land the config changes above on `release/v4.0` (this PR).
3. **Rename `release/v4.0` → `release/4.0`** (drop the `v`; GitHub rename-branch API retargets any
   PRs).
4. **Anchor the 4.0 line:** add an empty `Start 4.0` commit at the tip of `release/4.0` and tag it
   `4.0.0`, so the line versions from `4.0.0` (the first push publishes `4.0.1`, etc.). This is the
   one-off manual equivalent of what `release.yml`'s `start-*` does for future lines.
5. **Set `release/4.0` as the repository default branch.**
6. Add the `RELEASE_TOKEN` secret (PAT/App token) used by `release.yml`.
7. **Retire `main`** — delete it (recommended; `release/4.0` is a strict superset), or keep it as a
   non-default, unused branch.
8. Thereafter, pushes to `release/4.0` publish the **`4.0.x` stable line**; new majors/minors use
   the `release.yml` workflow.

## Verification

- Run `dotnet-gitversion` locally on `release/4.0` and a throwaway `release/5.0-beta`; confirm
  `4.0.x` stable and `5.0.0-beta.N`, and that `feature/*` produces an unpublished prerelease.
- On a scratch branch, dry-run `build.yml`: a **PR builds+tests but does not publish**; a **push
  to `release/**` publishes**; a push to `release/5.0-beta` publishes a **prerelease**.
- Confirm Dependabot raises PRs against the default branch after the flip (no `target-branch`).
- Confirm the `release.yml` token can change `default_branch` and rename a branch.
- Confirm packed `AssemblyVersion` = `<major>.0.0.0`.

## Open decisions

- **`main`:** delete it (recommended) vs keep a vestigial, unused branch.
- **Prerelease label wording:** `-beta` (recommended) vs `-preview` / `-rc`.
- **Graduation mechanism:** GitHub rename-branch API (recommended — atomic, retargets PRs) vs
  create-new + delete-old.

## Applicability to MooApp

MooApp is a separate repo but shares the same publish-from-CI shape. The same strategy applies
directly; the MooApp-specific work is: the transition steps against its current branches, its
`GitVersion.yml`/workflow files, and confirming its Dependabot/default-branch setup. Design this
ASM version first as the proving ground, then mirror it.
