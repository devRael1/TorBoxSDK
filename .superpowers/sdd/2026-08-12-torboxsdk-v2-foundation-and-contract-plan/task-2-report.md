# Task 2 — TorBox V2 contract baseline report

## Status

`DONE`

## Scope delivered

- Frozen the official TorBox OpenAPI response in `contracts/torbox/baseline/openapi.json` without generating any C# source.
- Added `manifest.json`, a manual operation inventory (`coverage.json`), and an empty reviewed-divergence register (`divergences.json`).
- Added `tools/UpdateTorBoxContract.ps1`; its only TorBox HTTP call is inside the explicit `-Refresh` branch. `-Validate` only reads local files.
- Added offline contract-test infrastructure resolving the copied `contracts/torbox/` tree from `AppContext.BaseDirectory`, never the working directory.
- Configured the contract-test project to copy the full contract tree to each target's test output.

## Files

- `contracts/torbox/baseline/openapi.json`
- `contracts/torbox/baseline/manifest.json`
- `contracts/torbox/coverage.json`
- `contracts/torbox/divergences.json`
- `tools/UpdateTorBoxContract.ps1`
- `tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs`
- `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractBaseline.cs`
- `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractTestPaths.cs`
- `tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj`

## Frozen snapshot

| Field | Value |
| --- | --- |
| Source | `https://api.torbox.app/openapi.json` |
| Retrieved at UTC | `2026-08-12T21:27:10.4501753Z` |
| OpenAPI `info.version` | `1.0.0` |
| Byte length | `96355` |
| SHA-256 | `ae0504c7ce2b76a0f003a0cc9ead61720800653e1d975a66cd414d2dd6a7eeb9` |
| Snapshot operations | `93` |
| Coverage rows | `93` |
| Snapshot/coverage operation keys | Exact match |
| Initial divergences | `0` (`[]`) |

All coverage rows use `operationKey` in the exact `METHOD path` form. They are deliberately `Planned`, `requires-validation`, and `Unassigned` for `family`/`resource`, with null public/request/result types and empty parameter/divergence arrays. Task 3 owns replacing those transitional assignments; no endpoint mapping was invented here.

## Red-green evidence

1. Before a baseline existed, ran:

   ```text
   dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests
   ```

   Result: exit `1`; the artifact-presence test failed on all five target frameworks because `openapi.json` was absent from `AppContext.BaseDirectory/contracts/torbox/baseline`.

2. Generated the initial baseline through the sole refresh command:

   ```text
   pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Refresh -InitializeCoverage
   ```

   Result: exit `0`. The script used the official source URL in its explicit refresh branch, saved the response bytes to the snapshot, wrote the manifest, and generated 93 coverage rows.

3. The first post-refresh test exposed a real loader issue: camelCase manifest properties were deserialized with case-sensitive defaults, yielding a null hash. The existing hash assertion failed on all five targets. `ContractBaseline` was changed only to deserialize manifest properties case-insensitively, then the same test was rerun.

4. Final targeted result:

   ```text
   dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests
   ```

   Result: exit `0`; 2 tests passed on each of `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`.

## Offline and safety checks

- `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Validate` — exit `0` before and after the final test changes; it recalculated only local SHA-256, byte length, and OpenAPI version.
- `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -InitializeCoverage` — expected exit `1`: `coverage.json` already exists, so the script refused to overwrite the reviewed mapping. No network access occurred.
- The final local inventory check confirmed 93 snapshot operation keys, 93 coverage rows, and exact sorted-key equality.
- No production package or V1 project was changed. The script contains no C# generation path.

## Review

Local review covered `ContractSnapshotFileTests.cs`, `ContractBaseline.cs`, `ContractTestPaths.cs`, and `TorBoxSDK.V2.ContractTests.csproj` against the repository C# conventions for tests. The tests use deterministic local files, `AppContext.BaseDirectory`, xUnit assertions, and explicit Arrange/Act/Assert sections. No critical, major, minor, or nitpick finding remained.

Verdict: `APPROVED`.

## Commit

`test: freeze TorBox V2 contract baseline`

## Risks and follow-up

- The snapshot is intentionally time-bound to the retrieval timestamp above; a future intentional refresh must review the changed bytes, manifest, and mapping separately.
- `family` and `resource` are temporary `Unassigned` values by explicit Task 2 scope. They are not valid for Task 3/release coverage validation and must be assigned there.
- `responseMode` remains `requires-validation` until snapshot or reviewed behavior supports `json`, `stream`, or `redirect`; no live behavior was inferred.

## Fix round 1/5 — contract validation hardening

### Findings closed

- `-Refresh` now downloads into a uniquely named temporary snapshot, derives and writes a temporary manifest, and validates the complete candidate baseline before publication. Existing snapshot and manifest files are replaced through same-volume replacement files with backups and rollback; backups are removed only after both replacements complete. A failed download, invalid JSON, invalid manifest, or coverage mismatch therefore leaves the existing baseline untouched.
- `-Validate` and `ContractBaseline.Load` now require a coverage JSON array with one non-empty, unique `operationKey` per record and exact set equality with the OpenAPI `METHOD path` keys. They do not regenerate or modify coverage.
- Both paths now require the official source URL, an explicitly UTC `retrievedAtUtc` string, non-empty OpenAPI version matching the snapshot, byte length, and SHA-256. The PowerShell loader preserves the raw timestamp string because `ConvertFrom-Json` otherwise converts it to local time before validation.

### Test files

- `tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs`
- `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractBaseline.cs`

The targeted tests include rejection of an untrusted source URL, a non-UTC timestamp, a missing OpenAPI version, a missing snapshot operation in coverage, and a duplicate coverage key. They use temporary copies only and make no HTTP calls.

### Commands and results

1. `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests`
   - Red: after the new refusal tests compiled, five tests failed on each target because the old loader accepted all malformed artifacts.
   - Green/final: exit `0`; 7 tests passed on each of `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`.
2. `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Validate`
   - Exit `0`; offline validation accepted the checked-in snapshot, manifest, and coverage inventory.
3. Isolated temporary-copy checks invoking `-Validate`:
   - Expected rejections observed for an untrusted manifest `sourceUrl` and an incomplete `coverage.json`; the checked-in baseline was not modified and no HTTP call was made.
4. `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -InitializeCoverage`
   - Expected exit `1`; it refused to overwrite the existing reviewed coverage mapping.

### Commit

`test: harden TorBox V2 contract validation` (corrective commit following `1d2c3dedc9bae012a57bb14875fccfc904c1c055`)

### Remaining concern

The snapshot remains intentionally pinned to its recorded retrieval time. Any future approved `-Refresh` must be reviewed with its resulting mapping changes; `-Validate` stays fully offline.

## Fix round 2/5 — durable transaction recovery and explicit UTC

### State protocol

Publication now uses `contracts/torbox/.update-transaction/` as a durable journaled state, which is created only after all three complete candidate artifacts have been copied and validated:

1. The writer stages candidate `baseline/openapi.json`, `baseline/manifest.json`, and `coverage.json` outside the live transaction path, validates them locally, writes `journal.json` (`activeGeneration: candidate`, `publishCoverage`), then atomically renames the complete transaction directory into place.
2. While `journal.json` exists, the C# contract loader and `-Validate` resolve the full candidate generation inside the transaction directory. They never validate the sequentially replaced durable files, so an interruption cannot make a mixed durable state appear valid.
3. A later authorized writer (`-Refresh` or `-InitializeCoverage`) resumes the physical files from that candidate using an atomic replacement per file while retaining the journal. It validates the durable set, then removes the journal. Re-running recovery is idempotent: an interruption leaves the same complete candidate and journal for the next attempt.
4. An interruption before the journal rename leaves the previous durable baseline untouched. A malformed or incomplete journal/candidate is rejected by both readers rather than guessed or repaired from mixed files.

This deliberately does not claim filesystem-level multi-file atomic replacement. Coherence is provided by the durable, complete candidate selected by the journal. `-Validate` remains offline and read-only; it validates that logical candidate. Physical recovery occurs only on an already authorized writing command. Existing coverage is physically rewritten only when the journal records that the interrupted operation was `-InitializeCoverage`.

### UTC rule

PowerShell now requires `retrievedAtUtc` to end with the explicit `Z` UTC marker before invariant parsing and zero-offset verification. A timestamp without a zone is rejected even when the host timezone is UTC.

### Tests and commands

- `tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs`
  - Red evidence: with a journaled candidate, the prior loader failed on a deliberately mixed durable snapshot and accepted an incomplete candidate.
  - Green evidence: the loader now reads the complete candidate and rejects an incomplete candidate. It also rejects a timestamp without `Z`.
- `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests`
  - Final exit `0`; 10 tests passed on each of `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`.
- `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Validate`
  - Final exit `0`; validated the normal durable baseline without a network call.
- Isolated temporary-copy PowerShell transaction test
  - `-Validate` accepted a full candidate while the durable snapshot had been deliberately replaced with `{}` and left that durable file unchanged.
  - The next `-InitializeCoverage` invocation resumed the pending transaction physically, then correctly refused to overwrite the already present coverage mapping; the transaction journal was removed.
  - `-Validate` rejected a manifest timestamp lacking the explicit `Z` marker. No test made an HTTP request.

### Commit

`test: recover TorBox V2 contract transactions`

### Remaining concern

The journal directory is an intentional recovery artifact. If an interruption occurs, read-only validation remains coherent through its candidate; a subsequent authorized writer completes physical publication. A manually damaged journal is rejected rather than silently repaired.

## Fix round 3/5 — safe journal retirement and coverage-write guard

### Revised transaction and interruption protocol

The transaction journal remains the only selector of a candidate generation. Candidate cleanup now has an explicit safe ordering:

1. A transaction that can publish `coverage.json` records `schemaVersion: 2`, `expectedCoverageExists`, and (when it existed) the SHA-256 present when the transaction began.
2. Before publishing any durable artifact, recovery validates the complete candidate and tests that coverage still has that expected state. It tests the same expectation again immediately before replacing coverage. If it changed or appeared, recovery refuses without modifying a durable artifact and leaves `journal.json` plus the full candidate available for inspection/reconciliation.
3. After the three durable artifacts validate as one baseline, recovery renames `journal.json` to a non-selector `journal.retired.<guid>.json`. No candidate file is removed before this rename returns.
4. Only with no `journal.json` visible does cleanup remove the transaction directory recursively. An interruption during cleanup therefore leaves no selector and readers use the already validated durable baseline. A later authorized writer clears that retired residue before publishing a new candidate.

Thus an interruption around journal retirement has only two reader-visible states: `journal.json` with a complete candidate, or no active journal with a complete durable baseline. `-Validate` and the C# loader remain read-only. For a legacy active journal without the new coverage expectation, `-Validate` still reads its complete candidate; an authorized physical recovery refuses rather than risking a coverage overwrite.

The comparison is an integrity guard, not a cross-process lock: an unrelated, non-cooperating writer can still theoretically modify `coverage.json` in the tiny interval after the final hash comparison and before the filesystem replacement. The script detects all changes present at recovery/publication checks, but cannot provide compare-and-swap semantics across arbitrary external writers without a shared locking protocol.

### Test files and scenarios

- `tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs`
  - Adds `ContractTransaction_WhenJournalIsRetired_LoadsDurableBaselineWithoutTheCandidate`, proving that after the selector is retired a missing candidate cannot affect the readable durable generation.
  - The existing incomplete-active-candidate test remains the complementary refusal proof: a visible `journal.json` without all candidate artifacts is rejected.
- Isolated temporary-copy PowerShell transaction checks (no HTTP):
  - Red before the fix: a pending `publishCoverage` journal overwrote an externally modified valid coverage mapping and removed its journal.
  - Green after the fix: the same mutation returns a non-zero recovery result, preserves the external coverage hash, and leaves both journal and candidate intact.
  - A matching expectation completes recovery, removes the transaction directory only after journal retirement, and a following `-Validate` accepts the durable baseline.
  - A legacy journal with a full candidate is accepted by `-Validate` without changing the deliberately damaged durable snapshot.

### Commands and results

1. Isolated temporary-copy PowerShell red scenario (pre-fix)
   - Expected safe-refusal assertion failed as intended: `exit=1 hashPreserved=False journalExists=False`, proving the old recovery overwrote coverage and removed the active journal.
2. Isolated temporary-copy PowerShell recovery scenarios (post-fix)
   - Exit `0`; `round3-transaction-scenarios=passed`. The changed-coverage path refused and retained journal/candidate; the matching path retired then removed the journaled transaction and `-Validate` passed.
3. Isolated legacy-journal read-only check
   - Exit `0`; `legacy-journal-read-only-validation=passed` while the durable snapshot remained `{}`.
4. `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --no-restore --filter FullyQualifiedName~ContractSnapshotFileTests`
   - Exit `0`; 11 tests passed on each of `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`.
5. `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Validate`
   - Exit `0`; normal baseline validation remained offline and read-only.
6. `git diff --check`
   - Exit `0`.

### Commit

`test: guard TorBox V2 contract transaction cleanup` (corrective commit following `e059f689c61da010c7f6748925b632aa4991f92b`)

### Remaining concern

The hash guard safely rejects a transaction if coverage has changed before either publication check, including a mapping that appeared after an initialization transaction started. It cannot impose a lock on arbitrary external writers between the final check and the file replacement; that would require a wider shared-locking decision outside this task's offline contract scope.
