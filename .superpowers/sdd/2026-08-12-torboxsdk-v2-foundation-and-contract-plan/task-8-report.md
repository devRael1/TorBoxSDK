# Task 8 report — V2 deterministic foundation gate

## Handoff scope

This record describes the Task 8 implementation handoff on
`codex/v2-redesign-foundation`. It does not provide an independent review
verdict.

The change is limited to the deterministic V2 gate, its V2-specific
instructions and delivery guidance, the existing test/dev skill references,
the two requested workflows, and this handoff record:

- `eng/Invoke-V2DeterministicChecks.ps1`
- `.github/instructions/torboxsdk-v2.instructions.md`
- `.agents/skills/tests/SKILL.md`
- `.agents/skills/dev/references/dev-jobs.md`
- `.agents/skills/dev/references/development-playbooks.md`
- `.github/workflows/ci.yml`
- `.github/workflows/integration-tests.yml`
- this handoff record

No V1 code, V2 production code, contract-baseline data, package version, or
NuGet publishing workflow was changed.

## RED evidence and safety finding

Before this task, invoking the missing deterministic script failed immediately
with exit code 64. That RED check did not mutate the repository.

The first minimal recovery script was intentionally run with `-SkipPack`, but
its dynamic project-name selector was incorrect: it classified the V1
integration project as a unit-test project. The run attempted real V1 API
requests and failed with authentication and DNS errors. No API key was
provided, printed, or persisted. This is a safety finding, not a successful
validation result.

The root cause was the broad `(unit|tests)` project-name rule. The mitigation
replaces all dynamic project discovery with the exact V2 solution, unit-test,
contract-test, core-package, and DI-package paths. The gate itself contains no
integration, refresh, or HTTP-client command references. Before rerunning it,
a static scan recorded zero matches for:

```text
IntegrationTests|Invoke-WebRequest|Invoke-RestMethod|UpdateTorBoxContract.*Refresh|curl|live
```

The scan also verified that the only project/solution inputs are
`TorBoxSDK.V2.slnx`, the V2 unit and contract projects, and the V2 core and DI
projects. The only `openapi.json` strings are checked-in Main and Relay
artifact paths used for local presence validation.

## Deterministic gate behavior

`Invoke-V2DeterministicChecks.ps1` validates the checked-in multi-source
baseline before a locked restore. Its required files include Main, Relay, and
Search snapshots/manifests; `sources.json`, `divergences.json`, and
`coverage.json`; and all seven V2 lockfiles.

It then performs only these build/test operations:

1. `dotnet restore TorBoxSDK.V2.slnx --locked-mode`
2. Release build of `TorBoxSDK.V2.slnx` with no restore
3. V2 unit tests with no build or restore
4. V2 contract tests filtered to `Category!=Release`

The default path excludes the release trait. `-IncludeReleaseContract` adds a
separate `Category=Release` invocation. With no final-cutover eligibility
flag, that one selected test skips on each target; it does not claim that the
release contract has passed.

The package path is fixed to `artifacts/v2-packages`. Before clearing it, the
gate proves it is a descendant of the repository root, inspects every existing
path segment for a reparse point, creates and resolves the parent, proves the
resolved candidate remains a descendant, and repeats the reparse-point check.
Only then can `Remove-Item -Recurse` clear that exact directory.

The gate packs only the V2 core and DI projects locally. It verifies both
package identities, all six library target assets, nuspec presence and
metadata, and the production-dependency boundary. It never publishes a
package.

## Documentation and workflow delivery

The new V2 instruction file records the six-target production matrix,
response-as-value policy, separate DI package boundary, stream-response
ownership, and checked-in baseline rules. It explicitly leaves V1 as legacy
until cutover.

The `tests` skill and dev references now distinguish V1's legacy downloaded
OpenAPI guidance from V2 static tests, which read the versioned multi-source
baseline. They direct default V2 validation through the deterministic gate and
keep protected V2 validation opt-in.

`ci.yml` adds a `V2 Deterministic Gate` job that invokes the PowerShell gate.
`integration-tests.yml` adds a false-by-default `run_v2_live` manual input and
a `V2 Live Validation` job in the protected `testing` environment, passing
only the protected `TORBOX_API_KEY` secret to the V2 integration-test command.
Existing V1 jobs and all publishing behavior remain unchanged.

## Final deterministic evidence

All checks below ran from the V2 worktree with `TORBOX_API_KEY` removed from
the process environment. No test or script step sent a TorBox request.

```powershell
pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1
```

Result:

- locked V2 restore passed;
- V2 Release solution build passed with **0 warnings and 0 errors**;
- V2 unit tests passed **92/92** on each of net6.0 through net10.0;
- V2 non-release contract tests passed **39/39** on each of net6.0 through
  net10.0;
- local core and DI packages were created and inspected successfully.

The inspected nuspec dependency sets were:

| Package | Version | Verified dependencies |
| --- | --- | --- |
| `TorBoxSDK` | `2.0.0` | `System.Text.Json` only |
| `TorBoxSDK.DependencyInjection` | `2.0.0` | `TorBoxSDK` plus Configuration.Abstractions, DependencyInjection.Abstractions, Http, Options, and Options.ConfigurationExtensions |

`pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1 -SkipPack -IncludeReleaseContract` also completed successfully. It selected exactly the release trait and reported **1 skip** per executable target because `TORBOXSDK_V2_RELEASE_CONTRACT` was not set; this is the expected current cutover guard.

`git diff --check` completed with no whitespace errors. The `actionlint`
binary was not installed in this worktree, so that dedicated workflow-linter
check could not be run locally. NuGet emitted a README advisory for each local
package; it did not produce a build warning/error and was not changed within
this task's authorized file scope.
