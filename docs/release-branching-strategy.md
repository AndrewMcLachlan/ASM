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

- **`main` is the trunk.** PRs and Dependabot land here; every merge publishes `X.Y.<count>`.
- **`feature/*` / PRs** build and test but never publish.
- **A parallel line is just a branch** with its own `VersionPrefix`/`VersionSuffix`:
  - *Breaking-major beta:* branch `release/5.0-beta`, set `VersionPrefix 5.0` + `VersionSuffix beta`
    → publishes `5.0.0-beta.N` while `main` stays on `4.x`. **Graduate** by merging to `main` and
    setting `VersionPrefix 5.0` with no suffix.
  - *Maintaining an old line* (only if ever needed): branch `release/4.x` off the last 4.x point;
    it keeps `VersionPrefix 4.0` and publishes `4.0.<count>` independently.

Publishing is gated in CI to `main` and `release/**`. There are **no tags, no anchor commits, no
release workflow, no default-branch juggling, and no admin tokens** involved.

## Cutting a new major/minor — the whole procedure

1. (Optional, for a breaking major you want to preview) branch `release/5.0-beta`.
2. Edit `VersionPrefix` (and `VersionSuffix` for a beta) in `Directory.Build.props`, commit, push.
3. Done — CI publishes the new line from there.
