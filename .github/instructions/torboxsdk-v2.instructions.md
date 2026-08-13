---
applyTo: "src/TorBoxSDK.V2/**/*.cs,src/TorBoxSDK.DependencyInjection.V2/**/*.cs,tests/TorBoxSDK.V2.*/**/*.cs,contracts/torbox/**,eng/Invoke-V2DeterministicChecks.ps1"
description: "Use for the side-by-side TorBoxSDK V2 foundation, its contract baseline, deterministic validation, and separate dependency-injection package."
---

# TorBoxSDK V2 Engineering Rules

These rules apply to the side-by-side V2 foundation. They do not change the V1 public surface or its legacy validation behavior before cutover.

## Instruction precedence on V2 paths

For `src/TorBoxSDK.V2/**/*.cs` and
`src/TorBoxSDK.DependencyInjection.V2/**/*.cs`, this instruction supersedes
only the two legacy response/exception requirements in **Part 2 → Response
Handling** of `csharp-conventions.instructions.md`: converting `success:
false` into `TorBoxException`, and translating HTTP 4xx/5xx responses into
typed exceptions. Apply every generic rule and every other Part 2 rule as
written. V1 paths retain the legacy response/exception requirements.

For V2 test files under `tests/TorBoxSDK.V2.*/**/*.cs`, keep every **Part 4**
test rule in force. The response and transport policy below defines the V2
behavior those tests must assert; Part 2 does not apply to test files and its
legacy exception requirements are not test expectations.

## Target matrix

- `TorBoxSDK.V2` and `TorBoxSDK.DependencyInjection.V2` must support `netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0`.
- V2 test projects run on `net6.0;net7.0;net8.0;net9.0;net10.0`.
- Do not introduce a production API that is unavailable on any supported V2 target.

## Response and transport policy

- A valid TorBox JSON envelope is a response value. Preserve `Success`, `Error`, `Detail`, and `StatusCode` in `TorBoxResponse` or `TorBoxResponse<T>`, including `success: false` and JSON error status responses.
- Do not translate an API-declared failure envelope into the V1 exception model.
- Throw `TorBoxProtocolException` only when the response violates the V2 transport contract, such as unsupported content, malformed JSON, or an invalid stream response shape.

## Package boundaries

- The core `TorBoxSDK` package remains free of `Microsoft.Extensions.*` dependencies. `System.Text.Json` compatibility support for `netstandard2.0` is allowed.
- Dependency-injection registration belongs only in the separate `TorBoxSDK.DependencyInjection` package. It references the core package and owns the reviewed Microsoft.Extensions dependencies.
- Do not publish V2 packages from a deterministic or CI validation job.

## Streaming ownership

- Streaming endpoints return `TorBoxStreamResponse`; a successful response owns its HTTP response and exactly one stream or redirect URI.
- The consumer disposes `TorBoxStreamResponse` after consuming the stream. Do not buffer a successful content stream into memory.
- Structured failures remain response values and must not own a stream, redirect URI, or HTTP response.

## Versioned contract baseline and validation

- `contracts/torbox/sources.json`, checked-in source snapshots and manifests, `coverage.json`, and `divergences.json` are the V2 multi-source contract baseline.
- Static V2 contract tests read those checked-in files. They must not refresh or download an OpenAPI document.
- Run the offline foundation gate with `pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1`. It performs a locked restore, Release build, V2 unit tests, non-release V2 contract tests, and local package inspection.
- `-IncludeReleaseContract` is reserved for an eligible final cutover. It is not a substitute for the separately protected manual validation workflow.

## V1 boundary until cutover

V1 remains legacy. Its existing direct OpenAPI and live-validation guidance must not be copied into V2 static checks, and V2 response-as-value semantics must not be retrofitted onto V1 without an approved migration.
