# Schema Validation Testing

## Overview

Schema validation tests verify bidirectional consistency between SDK model types
and the versioned contract baseline. `contracts/baseline/manifest.json` is the
authority for captured raw sources, provenance, availability and SHA-256
hashes. Sources that could not be verified are recorded as unavailable rather
than replaced with inferred data.

## Test Categories

### Static Schema Tests (`OpenApi/`)

Load the recorded Main OpenAPI baseline and compare it against SDK types
without calling the API:

- **`OpenApiFieldCoverageTests`** — every OpenAPI field must have a `[JsonPropertyName]` in the SDK, and vice-versa.
- **`OpenApiTypeMappingTests`** — C# property types must be compatible with OpenAPI type declarations.

These deterministic tests are marked `Category=Contract`. They run offline,
without `TORBOX_API_KEY`, through `ContractBaselineReader.Load()` and
`OpenApiSchemaReader.ReadFromBaselineAsync()`.

### Live Schema Tests (`Live/`)

Call real TorBox API endpoints and detect unmapped fields in the JSON response:

- Require `TORBOX_API_KEY` environment variable.
- Skip gracefully when the key is absent.
- Use `SchemaAssert.FindUnmappedFieldsAsync<T>()` for field detection.
- Run only under the authorization and execution policy eventually defined by
  DEC-017; this guide does not define a CI schedule or gate.

## Adding a New Model

When adding or updating an SDK model:

1. **Register the mapping** in `SchemaModelMapping.SchemaToType` — the key is the OpenAPI schema name, the value is the SDK model type.
2. **Run deterministic contract tests** with `dotnet test tests/TorBoxSDK.SchemaValidationTests/TorBoxSDK.SchemaValidationTests.csproj -c Release -f net10.0 --filter "Category=Contract"` to verify field and type coverage offline.
3. **Add known exclusions** if the SDK intentionally diverges:
   - `KnownOpenApiFieldsNotInSdk` — OpenAPI fields the SDK does not map (e.g., deprecated or internal-only fields).
   - `KnownSdkFieldsNotInOpenApi` — SDK extensions not in the spec (e.g., computed properties).
   - `KnownTypeMismatches` — fields where the OpenAPI type differs from the SDK serialization (e.g., integer enums serialized as strings).
   - `KnownSchemaMappingsNotInMainBaseline` — named DEC-004 / DIV-001
     differences absent from the retained Main baseline; never use it to infer
     an OpenAPI union.
4. **Add a live test** only when it is justified and authorized, following the
   pattern in existing `Live/` test files.

## Infrastructure

| Class | Location | Responsibility |
|-------|----------|----------------|
| `ContractBaselineReader` | `Infrastructure/` | Validates and loads the versioned manifest and captured artifacts |
| `OpenApiSchemaReader` | `Infrastructure/` | Parses the retained Main OpenAPI baseline (cached per process via `Lazy<T>`) |
| `SchemaModelMapping` | `Infrastructure/` | Schema-to-type map, known exclusions, type mapping |
| `ModelReflector` | `Infrastructure/` | `[JsonPropertyName]` reflection (delegates to `TestUtilities`) |
| `UnmappedFieldDetector` | `Infrastructure/` | Recursive unmapped field detection (delegates to `TestUtilities`) |
| `SchemaLiveTestFixture` | `Infrastructure/` | Shared `HttpClient` with API key for live tests |
| `SchemaTestCollection` | `Infrastructure/` | xUnit collection definition for live test fixture sharing |

## Key Rules

- Version the approved raw artifacts with `manifest.json`; keep their
  provenance, content length and SHA-256 in the same reviewed change.
- Use `ContractBaselineReader.Load()` and
  `OpenApiSchemaReader.ReadFromBaselineAsync()` for deterministic schema
  tests. Do not add a remote fetch to that path.
- Run static tests with `Category=Contract`; they must work offline and
  without `TORBOX_API_KEY`.
- Keep remote monitoring manual and opt-in:
  `pwsh ./eng/Invoke-ContractMonitor.ps1 -AllowNetwork -OutputPath <path-outside-contracts/baseline> [-FailOnDrift]`.
  Its report never replaces an artifact or `manifest.json`.
- A remote drift result opens a review; it does not alter the SDK, establish
  the effective contract or plan CI.
- The baseline is not a response-fixture store. DEC-011 remains open.
- Live tests must skip gracefully without `TORBOX_API_KEY`.
- All `HttpResponseMessage` in live tests must use `using` and
  `EnsureSuccessStatusCode()`.
