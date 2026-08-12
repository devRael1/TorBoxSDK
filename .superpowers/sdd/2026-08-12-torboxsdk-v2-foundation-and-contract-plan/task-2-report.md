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
