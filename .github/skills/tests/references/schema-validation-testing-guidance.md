# Schema Validation Testing

## Overview

Schema validation tests verify bidirectional consistency between the TorBox OpenAPI specification and SDK model types. The OpenAPI spec is fetched at test time from `https://api.torbox.app/openapi.json` — no local copy is versioned in the repository.

## Test Categories

### Static Schema Tests (`OpenApi/`)

Compare the downloaded OpenAPI specification against SDK types without calling the API:

- **`OpenApiFieldCoverageTests`** — every OpenAPI field must have a `[JsonPropertyName]` in the SDK, and vice-versa.
- **`OpenApiTypeMappingTests`** — C# property types must be compatible with OpenAPI type declarations.

Static tests run in CI with `--filter "Category!=Live"` and require network access to download the spec.

### Live Schema Tests (`Live/`)

Call real TorBox API endpoints and detect unmapped fields in the JSON response:

- Require `TORBOX_API_KEY` environment variable.
- Skip gracefully when the key is absent.
- Use `SchemaAssert.FindUnmappedFieldsAsync<T>()` for field detection.
- Run in CI via the `integration-tests.yml` workflow with the `testing` environment.

## Adding a New Model

When adding or updating an SDK model:

1. **Register the mapping** in `SchemaModelMapping.SchemaToType` — the key is the OpenAPI schema name, the value is the SDK model type.
2. **Run static tests** with `dotnet test tests/TorBoxSDK.SchemaValidationTests/ --filter "Category!=Live"` to verify field and type coverage.
3. **Add known exclusions** if the SDK intentionally diverges:
   - `KnownOpenApiFieldsNotInSdk` — OpenAPI fields the SDK does not map (e.g., deprecated or internal-only fields).
   - `KnownSdkFieldsNotInOpenApi` — SDK extensions not in the spec (e.g., computed properties).
   - `KnownTypeMismatches` — fields where the OpenAPI type differs from the SDK serialization (e.g., integer enums serialized as strings).
4. **Add a live test** when a new endpoint is mapped, following the pattern in existing `Live/` test files.

## Infrastructure

| Class | Location | Responsibility |
|-------|----------|----------------|
| `OpenApiSchemaReader` | `Infrastructure/` | Downloads and parses the OpenAPI spec (cached per process via `Lazy<T>`) |
| `SchemaModelMapping` | `Infrastructure/` | Schema-to-type map, known exclusions, type mapping |
| `ModelReflector` | `Infrastructure/` | `[JsonPropertyName]` reflection (delegates to `TestUtilities`) |
| `UnmappedFieldDetector` | `Infrastructure/` | Recursive unmapped field detection (delegates to `TestUtilities`) |
| `SchemaLiveTestFixture` | `Infrastructure/` | Shared `HttpClient` with API key for live tests |
| `SchemaTestCollection` | `Infrastructure/` | xUnit collection definition for live test fixture sharing |

## Key Rules

- Never version `openapi.json` locally — always fetch from `https://api.torbox.app/openapi.json`.
- Use `OpenApiSchemaReader.ReadFromApi()` for all spec access.
- Static tests must work without `TORBOX_API_KEY`.
- Live tests must skip gracefully without `TORBOX_API_KEY`.
- All `HttpResponseMessage` in live tests must use `using` and `EnsureSuccessStatusCode()`.
