# Release Branching & Versioning

How ASM packages are versioned and published. **The version is *stated*, never inferred** — no
GitVersion, no tags, no version-bump ceremony.

## The version

The current `Major.Minor` is declared in `Directory.Build.props`:

```xml
<VersionPrefix>4.0</VersionPrefix>
<!-- optional, for a prerelease line: -->
<VersionSuffix>beta</VersionSuffix>
```

CI (`.github/workflows/build.yml`) turns that into the published version:

- **Patch = commits since the version line last changed.** Specifically
  `git rev-list --count "$(git log -1 --format=%H -G'<Version' -- Directory.Build.props)"..HEAD`.
  The `-G'<Version'` scopes the counter to commits that actually changed a `<VersionPrefix>` /
  `<VersionSuffix>` line, so **editing the props file for any other reason does not reset the patch**.
- Stable → `Major.Minor.<count>` (e.g. `4.0.17`). Prerelease → `Major.Minor.0-<suffix>.<count>`
  (e.g. `5.0.0-beta.4`).
- `AssemblyVersion` = `Major.0.0.0` (no binding redirects within a major); `FileVersion` =
  `Major.Minor.<count>.0`.

**To bump the minor or major:** edit `VersionPrefix` (`4.0` → `4.1` / `5.0`) and commit. That commit
resets the patch, so the next publish is `X.Y.0`. That single, reviewable edit *is* the deliberate
version decision — nothing else to do.

## Branching

Because the version lives in the props file *on the branch*, every branch is self-describing and
isolated — no shared tags, no cross-talk.

- **`main` is the trunk.** PRs and Dependabot land here; every merge publishes `X.Y.<count>` (stable).
- **`feature/*` / PRs** build and test but never publish.
- **A preview line is a `release/**` branch with a `VersionSuffix`** — e.g. branch `release/5.0-beta`,
  set `VersionPrefix 5.0` + `VersionSuffix beta` → publishes `5.0.0-beta.N` while `main` stays on `4.x`.

**Publishing is gated in CI to `main` (the stable trunk) and prerelease builds** — i.e. `main`, or any
branch whose version carries a `VersionSuffix`. **A `release/**` branch with no suffix publishes
nothing** (that's what lets you graduate a beta in place — see below). There are no tags, anchor
commits, release workflow, default-branch juggling, or admin tokens.

> This deliberately means a *stable* `release/**` branch is not auto-published, so there is no
> parallel "maintain an old major (`4.0.x`) after main moves to `5.0`" flow — consistent with
> supporting one line at a time. If that need ever arises, widen the publish gate.

## Graduating a preview to GA

Because a suffix-less `release/**` branch publishes nothing, you graduate **in place** — no promotion
or integration branch:

1. On `release/5.0-beta`, delete the `<VersionSuffix>beta</VersionSuffix>` line and commit. The build
   runs but **publishes nothing** (stable, but not `main`).
2. Merge `release/5.0-beta` → `main`. `main` publishes **`5.0.0`** (removing the suffix line reset the patch).
3. Delete `release/5.0-beta`.

## Cutting a new major/minor
- **Minor / non-breaking:** edit `VersionPrefix` on `main` (`4.0` → `4.1`) in a normal PR; the next merge publishes `4.1.0`.
- **Breaking major (previewed):** branch `release/5.0-beta`, set `VersionPrefix 5.0` + `VersionSuffix beta`, then graduate as above.
