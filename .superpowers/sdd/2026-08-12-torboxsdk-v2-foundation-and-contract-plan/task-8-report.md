# Task 8 report — V2 deterministic foundation gate

## Handoff scope

This record describes the Task 8 implementation handoff on
`codex/v2-redesign-foundation`. It does not provide an independent review
verdict.

The change is limited to the deterministic V2 gate, its V2-specific
instructions and delivery guidance, the existing test/dev skill references,
the two requested workflows, and this handoff record:

- `eng/Invoke-V2DeterministicChecks.ps1`
- `TorBoxSDK.V2.slnx`
- `src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj`
- `.github/instructions/torboxsdk-v2.instructions.md`
- `.github/instructions/csharp-conventions.instructions.md`
- `.agents/skills/tests/SKILL.md`
- `.agents/skills/dev/references/dev-jobs.md`
- `.agents/skills/dev/references/development-playbooks.md`
- `.agents/skills/code-review/SKILL.md`
- `.agents/skills/code-review/references/instruction-map.md`
- `.agents/skills/code-review/references/review-workflow.md`
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

The root cause was the broad `(unit|tests)` project-name rule. The first
corrective RED check also proved that `TorBoxSDK.V2.slnx` still contained the
V2 integration project and that the DI package emitted a bare `2.0.0` core
dependency version. The mitigation replaces dynamic project discovery with the
exact V2 solution, unit-test, contract-test, core-package, and DI-package
paths. The solution now contains exactly six approved offline projects; before
restore, build, or test, the gate checks that exact set and walks its
`ProjectReference` graph to reject a direct or indirect
`TorBoxSDK.V2.IntegrationTests.csproj` reference.

The integration project name is intentionally present once as that explicit
safety guard; it is not hidden by concatenation and it is not an executable
command input. The static safety scan recorded no network or live-validation
commands, including:

```text
Invoke-WebRequest|Invoke-RestMethod|UpdateTorBoxContract.*Refresh|curl|live
```

The scan also verified that the gate's executable project inputs are only
`TorBoxSDK.V2.slnx`, the V2 unit and contract projects, and the V2 core and DI
projects. It does not infer a web call from a string: local `openapi.json`
artifact paths are validated as files, while source-provenance URLs in the
checked-in V2 contract baseline are data, not executable requests.

## Deterministic gate behavior

`Invoke-V2DeterministicChecks.ps1` validates the checked-in multi-source
baseline before a locked restore. Its required files include Main, Relay, and
Search snapshots/manifests; `sources.json`, `divergences.json`, and
`coverage.json`; and the six lockfiles for projects in the deterministic
solution. The V2 integration-test lockfile is deliberately outside this gate.

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
metadata, and every normalized TFM dependency group separately. The core may
have `System.Text.Json` only in its `netstandard2.0` group and never a
`Microsoft.Extensions.*` dependency. Every DI group must contain exactly the
core package plus the five reviewed Microsoft.Extensions packages; the core
range is derived from the packed core version and must be exact. A targeted
post-`_GetProjectReferenceVersions` MSBuild mutation encloses only the core
project-reference version in brackets before `GenerateNuspec`. It never
publishes a package.

## Documentation and workflow delivery

The new V2 instruction file records the six-target production matrix,
response-as-value policy, separate DI package boundary, stream-response
ownership, and checked-in baseline rules. Its precedence is explicit: on V2
source paths it replaces only Part 2's legacy response/exception mapping
rules, while generic and unrelated Part 2 rules remain mandatory. V1 remains
legacy until cutover. The code-review skill, instruction map, and review
workflow require reviewers to read and apply that narrow override for V2
targets.

The `tests` skill and dev references now distinguish V1's legacy downloaded
OpenAPI guidance from V2 static tests, which read the versioned multi-source
baseline. They direct default V2 validation through the deterministic gate and
keep protected V2 validation opt-in.

`ci.yml` adds a secretless `V2 Deterministic Gate` job that invokes the
PowerShell gate. `integration-tests.yml` adds a false-by-default `run_v2_live`
manual input and a `V2 Live Validation` job in the protected `testing`
environment. That job restores the V2 integration-test `.csproj` directly in
locked mode before its test command, and passes only the protected
`TORBOX_API_KEY` secret to that command. Existing V1 jobs and all publishing
behavior remain unchanged.

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
- local core and DI packages were created and inspected successfully, with all
  six dependency groups checked independently.

The inspected nuspec dependency sets were:

| Package | Version | Verified dependencies |
| --- | --- | --- |
| `TorBoxSDK` | `2.0.0` | Empty non-`netstandard2.0` groups; `System.Text.Json` only for `netstandard2.0`; no `Microsoft.Extensions.*` |
| `TorBoxSDK.DependencyInjection` | `2.0.0` | Every group: `TorBoxSDK=[2.0.0]` plus Configuration.Abstractions, DependencyInjection.Abstractions, Http, Options, and Options.ConfigurationExtensions; 8.x policy for net6/net7 and 10.x policy for netstandard2.0/net8/net9/net10 |

Corrective TDD evidence was retained separately from the full gate:

1. A pre-change policy probe failed because the deterministic solution still
   contained `TorBoxSDK.V2.IntegrationTests` and the DI package declared the
   core dependency as bare `2.0.0` without group-level validation.
2. After the solution, MSBuild target, and group-level validator changes, the
   real generated packages were accepted.
3. Temporary local nuspec fixtures using a bare core version, a missing target
   group, and a missing dependency in one group were each rejected by the
   production validator. The fixtures and probe were then removed.
4. The first two GREEN attempts exposed and corrected two gate-local PowerShell
   defects before the successful rerun: `$Matches['major']` is already a
   string (so accessing `.Value` failed while normalizing a TFM), and mandatory
   empty-array binding rejected the core package's intentionally empty
   dependency groups. Neither defect invoked an integration test or a web/API
   command; both were fixed before the final full gate.

A final static/offline check passed: PowerShell syntax and no network/live
commands in the gate; the exact six-project solution and graph guard; no
integration-test lockfile in the gate; direct locked restore of the V2
integration `.csproj` in the manual workflow; a secretless deterministic CI
job; and successful YAML parsing for both workflows.

`pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1 -SkipPack -IncludeReleaseContract` also completed successfully after the correction. It selected exactly the release trait and reported **1 skip** per executable target because `TORBOXSDK_V2_RELEASE_CONTRACT` was removed from the process environment; this is the expected current cutover guard and did not make a live request.

## Corrective cycle 2 — fail-closed deterministic project graph

The exact six-root solution assertion did not previously make the reachable
`ProjectReference` graph fail closed: it rejected only the specifically named
integration project and missing files, so another existing project could be
walked and implicitly admitted. This was reproduced before the production
change with an isolated Temp shadow of only the gate prefix through
`Assert-DeterministicSolutionExcludesIntegrationTests`. The shadow contained
the unchanged six-root solution plus a real, persisted `ProjectReference` from
the core project to an existing temporary
`src/TorBoxSDK.V2.Unapproved/TorBoxSDK.V2.Unapproved.csproj`. It retained no
`dotnet` invocation, API key, network command, or external API operation. The
pre-change static gate returned success, and the probe therefore failed RED
with:

```text
RED: the pre-dotnet graph guard accepted reachable unapproved project
'src/TorBoxSDK.V2.Unapproved/TorBoxSDK.V2.Unapproved.csproj'.
```

The graph guard now normalizes every `ProjectReference` declared by each
reachable project file relative to the repository root before it is enqueued.
It rejects rooted references,
paths that traverse outside the repository, and every normalized path not in
the exact six approved repository-relative projects. The existing explicit
integration-project rejection remains before traversal, so a renamed or V1
integration project is also rejected by the allowlist.

The same Temp shadow then passed GREEN by rejecting the added reachable project
with the expected `not in the approved deterministic V2 project graph`
diagnostic. With `TORBOX_API_KEY` removed, the real
`pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1 -SkipPack` also
passed: locked restore, Release build with 0 warnings and 0 errors, 92/92 V2
unit tests and 39/39 non-release contract tests on each executable target. No
integration test or live API call was invoked. The temporary probe was removed
after the evidence was captured.

Final cycle-2 verification also ran the full gate with the API key and release
eligibility variable removed. It passed the same restore, build, and test
matrix, created and validated the local core and DI packages, and did not make
an external request. `-SkipPack -IncludeReleaseContract` then passed with the
release trait selected and one expected skip on each executable target.

`git diff --check` completed with no whitespace errors. The `actionlint`
binary was not installed in this worktree, so that dedicated workflow-linter
check could not be run locally. NuGet emitted a README advisory for each local
package; it did not produce a build warning/error and was not changed within
this task's authorized file scope.

## Corrective cycle 3 — explicit V2 test-review policy

The first documentation rereview found that the V2 instruction described the
response/exception precedence for V2 source paths, while the code-review skill
and review workflow also told reviewers to apply it to V2 tests. That wording
was internally ambiguous because Part 2 is a source-only convention and Part 4
governs tests.

A local consistency probe failed RED before the documentation change because
the V2 instruction, generic convention, instruction map, review skill, and
review workflow did not all state an explicit V2 test policy. The corrected
rules now make the boundary precise:

- V2 source keeps generic conventions and every unrelated Part 2 rule, while
  the V2 instruction supersedes only the two legacy Part 2 failure-mapping
  requirements;
- V2 tests keep Part 1 and every Part 4 rule; they use the V2
  response-as-value and transport policy to determine the behavior to assert,
  not the source-only legacy Part 2 exception mapping;
- V1 source and tests retain the legacy rules unchanged.

The same consistency probe passed GREEN after the change. This was an
instruction-only correction: it made no API request, changed no V1 code or
workflow behavior, and introduced no package or runtime dependency.

## Deferred hardening at the stop point

A later review identified one further, unimplemented hardening: the static
preflight walks `ProjectReference` elements declared in the reachable project
files, but it does not yet traverse local MSBuild import closures such as
`Directory.Build.props`, `Directory.Build.targets`, or explicit local imports.
No such imported `ProjectReference` exists in the current V2 graph. An initial
implementation of that traversal was deliberately removed when work stopped,
because it had not reached a complete, independently reviewed state. The next
gate-hardening task must cover that import closure before claiming absolute
project-graph isolation.
