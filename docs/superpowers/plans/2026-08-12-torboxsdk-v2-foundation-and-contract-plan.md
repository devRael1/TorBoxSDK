# TorBoxSDK V2 Foundation and Contract Baseline Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a side-by-side, manually authored V2 foundation that can be verified offline and that freezes the TorBox contract before resource endpoints are implemented.

**Architecture:** V2 is built in dedicated projects with the final `TorBoxSDK` and `TorBoxSDK.DependencyInjection` assembly and package identities, while V1 remains unchanged until final cutover. The core owns options, response envelopes, protocol errors, streaming, authentication, and transport; the DI project owns Microsoft.Extensions integration. A checked-in OpenAPI snapshot plus manually maintained coverage manifest controls endpoint work without generating SDK code.

**Tech Stack:** C# latest, `netstandard2.0`, .NET 6–10, `System.Net.Http`, `System.Text.Json`, xUnit, PowerShell, GitHub Actions, and the existing DocFX toolchain.

## Global Constraints

- Target the V2 core and optional DI package at `netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0`.
- Keep the core free of third-party production dependencies; use `System.Text.Json` only as a conditional Microsoft compatibility package for `netstandard2.0`.
- Keep `TorBoxSDK.DependencyInjection` separate; only it may depend on Microsoft.Extensions packages for DI, options, configuration, and `IHttpClientFactory`.
- Handwrite every public model, interface, and endpoint. Do not generate or ship client/model source.
- Preserve the approved public hierarchy: `ITorBoxClient → Main/Search/Relay`, with all eleven Main resources.
- JSON endpoints return `Task<TorBoxResponse<T>>` or `Task<TorBoxResponse>`; structured TorBox API failures remain response values, including structured 4xx/5xx envelopes.
- Reserve exceptions for cancellation, network failures, and contract-invalid responses. Use `TorBoxProtocolException` for the latter.
- Return file/binary/redirect-oriented results through disposable `TorBoxStreamResponse`; it is the stream-operation response envelope and carries structured failure metadata when the server returns JSON instead of a stream. Never buffer a successful stream implicitly.
- Do not add automatic retry, backoff, or endpoint replay. The caller owns retry policy; every request is sent at most once by the SDK transport.
- Do not fetch OpenAPI during builds or deterministic tests. Store a reviewed snapshot, source metadata, retrieval timestamp, and SHA-256 hash in Git.
- Use explicit `CancellationToken` propagation, file-scoped namespaces, nullable references, one public type per file, and complete XML documentation.
- Never log or commit `TORBOX_API_KEY`; live tests are separate from deterministic tests.
- Keep the existing V1 solution buildable until the final V2 cutover plan; do not publish an interim package.

---

## File structure

| Path | Responsibility |
| --- | --- |
| `TorBoxSDK.V2.slnx` | Isolated solution containing only V2 projects. |
| `src/TorBoxSDK.V2/` | Portable core source. Its assembly and package identities are `TorBoxSDK`. |
| `src/TorBoxSDK.DependencyInjection.V2/` | Optional DI source. Its assembly and package identities are `TorBoxSDK.DependencyInjection`. |
| `src/TorBoxSDK.V2.Examples/` | V2-only example executable; no V1 project reference. |
| `tests/TorBoxSDK.V2.UnitTests/` | Deterministic V2 unit tests using a recording HTTP handler. |
| `tests/TorBoxSDK.V2.ContractTests/` | Offline snapshot and coverage-manifest tests. |
| `tests/TorBoxSDK.V2.IntegrationTests/` | Secret-protected, separately invoked V2 live tests. |
| `contracts/torbox/baseline/openapi.json` | Checked-in official contract snapshot. |
| `contracts/torbox/baseline/manifest.json` | Snapshot source, retrieval time, OpenAPI version, byte length, and SHA-256 hash. |
| `contracts/torbox/coverage.json` | Manual operation-to-public-surface mapping; code generation never reads it. |
| `contracts/torbox/divergences.json` | Reviewed differences between the snapshot and observed TorBox behavior. |
| `tools/UpdateTorBoxContract.ps1` | Explicit human-invoked refresh and validation command; not called from build/test. |
| `eng/Invoke-V2DeterministicChecks.ps1` | Single local/CI entry point for V2 restore, build, tests, and package inspection. |

Each V2 project that has NuGet dependencies owns a checked-in `packages.lock.json`. V1 projects keep their current restore behavior while V2 is side-by-side.

### V2 internal interfaces defined by this plan

```csharp
internal interface ITorBoxApiTransport
{
    Task<TorBoxResponse<T>> SendAsync<T>(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken);

    Task<TorBoxResponse> SendAsync(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken);

    Task<TorBoxStreamResponse> SendStreamAsync(
        HttpClient httpClient,
        HttpRequestMessage request,
        CancellationToken cancellationToken);
}
```

`ITorBoxApiTransport` is internal. Resource clients receive it together with the correct family-specific `HttpClient`; consumers never receive any HTTP transport object.

For a stream operation, `TorBoxStreamResponse` is the visible response envelope: a successful binary or redirect response has `Success == true` and owns its still-open content stream when one exists; a structured JSON API failure has `Success == false`, `Stream == null`, bounded `Error`/`Detail`, and the original status code. This keeps the response-as-value rule true for streams as well as JSON endpoints.

### Contract manifest shapes

```json
{
  "operationKey": "GET /v1/api/torrents/mylist",
  "family": "Main",
  "resource": "Torrents",
  "publicInterface": null,
  "publicMethod": null,
  "parameterTypes": [],
  "requestType": null,
  "resultType": null,
  "responseMode": "requires-validation",
  "implementationState": "Planned",
  "divergenceIds": []
}
```

```json
{
  "id": "DIV-001",
  "operationKey": "GET /v1/api/example",
  "source": "Controlled live validation",
  "observedAtUtc": "2026-08-12T00:00:00Z",
  "contractBehavior": "Describe the checked-in baseline behavior.",
  "v2Behavior": "Describe the explicitly chosen V2 behavior.",
  "evidence": "Sanitized fixture path or issue URL."
}
```

The second snippet documents the required fields only. The initial `divergences.json` is an empty JSON array (`[]`) until a real, reviewed difference exists; it must not contain invented entries.

### V2 response primitives

```csharp
public sealed record TorBoxResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; init; }
}

public sealed record TorBoxResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; init; }
}
```

`TorBoxResponse` and `TorBoxResponse<T>` are deliberately two standalone sealed records. They expose the same envelope metadata without an inheritance hierarchy, so every target has one unambiguous immutable public shape and `System.Text.Json` can populate the public `init` metadata. `StatusCode` is non-wire metadata; callers may set it only while constructing a test or a custom value.

## Task 1: Create the isolated V2 project topology

**Files:**

- Create: `TorBoxSDK.V2.slnx`
- Create: `src/TorBoxSDK.V2/TorBoxSDK.V2.csproj`
- Create: `src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj`
- Create: `src/TorBoxSDK.V2.Examples/TorBoxSDK.V2.Examples.csproj`
- Create: `tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj`
- Create: `tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj`
- Create: `tests/TorBoxSDK.V2.IntegrationTests/TorBoxSDK.V2.IntegrationTests.csproj`
- Create: `tests/TorBoxSDK.V2.Testing/TorBoxSDK.V2.Testing.csproj`
- Modify: `Directory.Packages.props`

**Interfaces:**

- Consumes: no V1 project reference.
- Produces: a V2-only solution whose test projects reference `src/TorBoxSDK.V2/` and whose DI/example projects reference the V2 core explicitly.

- [ ] **Step 1: Capture the V1 baseline without changing it.**

Run: `dotnet build TorBoxSDK.slnx --configuration Release`

Expected: record the result in the implementation notes; do not alter a V1 source file in this task.

- [ ] **Step 2: Create the core project with final public identities and the full target matrix.**

Create `src/TorBoxSDK.V2/TorBoxSDK.V2.csproj` with this essential structure:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0</TargetFrameworks>
    <AssemblyName>TorBoxSDK</AssemblyName>
    <PackageId>TorBoxSDK</PackageId>
    <Version>2.0.0</Version>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <IsPackable>true</IsPackable>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
  </PropertyGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0'">
    <PackageReference Include="System.Text.Json" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Create the optional DI project without adding Microsoft.Extensions dependencies to the core.**

Create `src/TorBoxSDK.DependencyInjection.V2/TorBoxSDK.DependencyInjection.V2.csproj` with the same target frameworks, `<AssemblyName>TorBoxSDK.DependencyInjection</AssemblyName>`, `<PackageId>TorBoxSDK.DependencyInjection</PackageId>`, `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>`, and a project reference to `../TorBoxSDK.V2/TorBoxSDK.V2.csproj`. Put `Microsoft.Extensions.*` package references only in this project and V2 test/example projects that need them.

- [ ] **Step 4: Create V2 test, test-support, integration, and example project references.**

Use this project-reference direction and no reverse references:

```text
TorBoxSDK.V2.UnitTests       -> TorBoxSDK.V2, TorBoxSDK.V2.Testing
TorBoxSDK.V2.ContractTests   -> TorBoxSDK.V2
TorBoxSDK.V2.IntegrationTests-> TorBoxSDK.V2, TorBoxSDK.DependencyInjection.V2
TorBoxSDK.V2.Examples        -> TorBoxSDK.V2, TorBoxSDK.DependencyInjection.V2
```

Target the V2 unit, contract, integration, and test-support projects at `net6.0;net7.0;net8.0;net9.0;net10.0`; they exercise the corresponding core/DI asset. Keep the example executable on one supported current runtime, initially `net8.0`. Add `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>` to every V2 project that restores packages. Do not add this property to the shared `Directory.Build.props`, which would change V1 restore behavior.

- [ ] **Step 5: Add all V2 projects to `TorBoxSDK.V2.slnx` and restore it.**

Run: `dotnet restore TorBoxSDK.V2.slnx --use-lock-file`

Expected: restore succeeds without building or packing the V1 projects and creates the reviewed V2 `packages.lock.json` files. Commit those lock files with their owning V2 projects; subsequent deterministic checks use `--locked-mode`.

- [ ] **Step 6: Add only the compatibility package version required by `netstandard2.0`.**

Add `System.Text.Json` to `Directory.Packages.props` with a centrally managed, reviewed Microsoft version that carries a `netstandard2.0` asset. Add no third-party production package and no Microsoft.Extensions reference to `TorBoxSDK.V2.csproj`.

- [ ] **Step 7: Build the empty V2 topology across every V2 target.**

Run: `dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore`

Expected: all V2 projects compile with zero warnings; failures caused by missing V2 source types are fixed by adding only minimal internal placeholders that are replaced in later tasks.

- [ ] **Step 8: Commit the project topology.**

```bash
git add TorBoxSDK.V2.slnx Directory.Packages.props src/TorBoxSDK.V2 src/TorBoxSDK.DependencyInjection.V2 src/TorBoxSDK.V2.Examples tests/TorBoxSDK.V2.UnitTests tests/TorBoxSDK.V2.ContractTests tests/TorBoxSDK.V2.IntegrationTests tests/TorBoxSDK.V2.Testing
git commit -m "build: scaffold isolated TorBoxSDK V2 projects"
```

## Task 2: Freeze the TorBox contract without generating SDK source

**Files:**

- Create: `contracts/torbox/baseline/openapi.json`
- Create: `contracts/torbox/baseline/manifest.json`
- Create: `contracts/torbox/coverage.json`
- Create: `contracts/torbox/divergences.json`
- Create: `tools/UpdateTorBoxContract.ps1`
- Test: `tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractBaseline.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractTestPaths.cs`
- Modify: `tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj`

**Interfaces:**

- Consumes: the official source URL `https://api.torbox.app/openapi.json` only when an explicit maintainer invokes the refresh script.
- Produces: an immutable-in-build baseline and manual operation inventory keyed by `METHOD path`, not by OpenAPI `operationId` alone.

- [ ] **Step 1: Write the failing snapshot-file tests.**

Create tests that require `openapi.json`, `manifest.json`, `coverage.json`, and `divergences.json` to exist and reject a manifest whose SHA-256 or byte count does not match the checked-in snapshot.

```csharp
[Fact]
public void BaselineManifest_WhenSnapshotChanges_RejectsTheHashMismatch()
{
    ContractBaseline baseline = ContractBaseline.Load(ContractTestPaths.BaselineDirectory);

    Assert.Equal(baseline.Manifest.Sha256, baseline.CalculateSha256());
    Assert.Equal(baseline.Manifest.ByteLength, baseline.SnapshotBytes.Length);
}
```

- [ ] **Step 2: Run the new test and confirm it fails before a baseline exists.**

Run: `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests`

Expected: FAIL because the baseline files are absent.

- [ ] **Step 3: Implement `UpdateTorBoxContract.ps1` as the only refresh path.**

The script must download the official URL only after an explicit `-Refresh` argument, write the exact response bytes to `contracts/torbox/baseline/openapi.json`, calculate SHA-256, parse `info.version`, and write a manifest. `-InitializeCoverage` may create rows only when `coverage.json` does not exist; it must fail rather than overwrite a reviewed mapping. `-Validate` recalculates local facts only. No script mode writes C# source.

```powershell
param([switch] $Refresh, [switch] $InitializeCoverage, [switch] $Validate)

if ($Refresh) {
    Invoke-WebRequest -Uri 'https://api.torbox.app/openapi.json' -OutFile $snapshotPath
    $hash = (Get-FileHash -LiteralPath $snapshotPath -Algorithm SHA256).Hash.ToLowerInvariant()
    # Write source URL, UTC retrieval time, OpenAPI info.version, byte length, and hash.
}

if ($InitializeCoverage) {
    # Fail if coverage.json already exists; otherwise create one Planned row per METHOD path.
}

if ($Validate) {
    # Recalculate the checked-in file hash and throw on mismatch. No network call is allowed here.
}
```

- [ ] **Step 4: Generate the baseline and initial manual coverage rows.**

Run: `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Refresh -InitializeCoverage`

Create one `coverage.json` row for each path/method in the snapshot. Use `operationKey` as `GET /path` or `POST /path`, because the live document contains duplicate operation IDs. Set `implementationState` to `Planned`; leave `publicInterface`, `publicMethod`, `requestType`, and `resultType` null; use an empty `parameterTypes` array; and leave `divergenceIds` empty unless supported by evidence. Before Task 3 can pass, manually assign every row to one approved `family` and `resource`; do not keep an `Unassigned` placeholder. Set `responseMode` to `json`, `stream`, or `redirect` only when the snapshot or reviewed documentation establishes it. Otherwise use `requires-validation`; that state is allowed before endpoint work but forbidden for an `Implemented` operation and at release.

- [ ] **Step 5: Create an empty divergence register rather than copying old assumptions.**

Set `contracts/torbox/divergences.json` to:

```json
[]
```

Only a fixture, controlled live test, or documented TorBox behavior may add a divergence later.

- [ ] **Step 6: Implement the contract-test file loader, then run the snapshot tests and offline script validation.**

Add `ContractTestPaths` so tests resolve a copied `contracts/torbox/` tree from `AppContext.BaseDirectory`, never from the process working directory. Add `ContractBaseline` to parse the snapshot, calculate its SHA-256 and byte count, and expose its operation keys. Update the contract test project so the entire checked-in contract tree is copied to test output.

Run: `pwsh -NoProfile -File tools/UpdateTorBoxContract.ps1 -Validate`

Run: `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter FullyQualifiedName~ContractSnapshotFileTests`

Expected: both commands pass without downloading the contract.

- [ ] **Step 7: Commit the contract baseline and refresh utility.**

```bash
git add contracts/torbox tools/UpdateTorBoxContract.ps1 tests/TorBoxSDK.V2.ContractTests/ContractSnapshotFileTests.cs
git commit -m "test: freeze TorBox V2 contract baseline"
```

## Task 3: Enforce manual coverage inventory in offline contract tests

**Files:**

- Create: `tests/TorBoxSDK.V2.ContractTests/Infrastructure/ContractBaseline.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/Infrastructure/CoverageManifest.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/Infrastructure/DivergenceRegister.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/OperationCoverageTests.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/DivergenceRegisterTests.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs`
- Create: `tests/TorBoxSDK.V2.ContractTests/V2ReleaseContractTests.cs`
- Modify: `tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj`

**Interfaces:**

- Consumes: checked-in baseline JSON and manual `coverage.json`.
- Produces: `ContractBaseline.Load(string directory)`, `CoverageManifest.Load(string path)`, and deterministic tests that know the exact operation set without accessing the network.

- [ ] **Step 1: Write the failing operation-key coverage test.**

```csharp
[Fact]
public void CoverageManifest_ContainsExactlyOneRecordForEverySnapshotOperation()
{
    IReadOnlySet<string> snapshotKeys = ContractBaseline.Load(ContractTestPaths.BaselineDirectory).OperationKeys;
    IReadOnlyList<CoverageRecord> coverage = CoverageManifest.Load(CoveragePath).Records;

    Assert.Equal(snapshotKeys.Count, coverage.Count);
    Assert.Equal(snapshotKeys.OrderBy(static key => key), coverage.Select(static record => record.OperationKey).OrderBy(static key => key));
}
```

- [ ] **Step 2: Write the failing record-shape test.**

Copy the checked-in `contracts/torbox/` tree to the contract-test output directory and resolve it from `AppContext.BaseDirectory`, never from the process working directory. Require each record to use a non-empty approved `family`, `resource`, and `responseMode`. Allow `publicInterface`, `publicMethod`, and `resultType` to remain null only while `implementationState` is `Planned`. `requestType` remains null for an operation without a request body; otherwise an implemented row must name its request type and include it in the ordered `parameterTypes`. Require an `Implemented` record to provide its fully qualified public interface, method name, ordered parameter type names, and result type. Require its response mode to be `json`, `stream`, or `redirect`, never `requires-validation`.

- [ ] **Step 3: Implement parsers that use `METHOD path` keys.**

Parse every HTTP verb object beneath `paths`. Do not trust `operationId` uniqueness. Reject duplicate coverage records, missing snapshot operations, stale coverage operations, and divergence IDs that do not exist in `divergences.json`. Add a default-on `ContractSurfaceTests.ImplementedMappingsResolveToPublicMethods` test: load the core by its final assembly name (`TorBoxSDK`), then for each implemented row reflect the named public interface and method with the declared ordered parameters and exact `Task` result type from the manifest. It passes vacuously while all foundation rows are `Planned`, then protects every resource plan as rows become `Implemented`.

- [ ] **Step 4: Run the contract suite and repair every initial coverage row.**

Run: `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release`

Expected: PASS with all snapshot operations represented, even though their implementation state is initially `Planned`.

- [ ] **Step 5: Add the final-completeness test in a disabled-by-default category.**

Create `V2ReleaseContractTests.AllOperationsAreImplemented` with `[Trait("Category", "Release")]`. It must fail whenever a record is not `Implemented`, has an unresolved response mode, lacks its public mapping, or fails the surface-mapping validation; it is not invoked until the final release plan enables it in the deterministic release gate.

```csharp
[Fact]
[Trait("Category", "Release")]
public void AllOperationsAreImplemented()
{
    IReadOnlyList<CoverageRecord> records = CoverageManifest.Load(CoveragePath).Records;

    Assert.DoesNotContain(records, static record => record.ImplementationState != CoverageImplementationState.Implemented);
    Assert.DoesNotContain(records, static record => record.ResponseMode == CoverageResponseMode.RequiresValidation);
    Assert.DoesNotContain(records, static record => string.IsNullOrWhiteSpace(record.PublicInterface) || string.IsNullOrWhiteSpace(record.PublicMethod) || string.IsNullOrWhiteSpace(record.ResultType));
}
```

- [ ] **Step 6: Commit the offline coverage gate.**

```bash
git add tests/TorBoxSDK.V2.ContractTests contracts/torbox/coverage.json contracts/torbox/divergences.json
git commit -m "test: add offline V2 contract coverage gate"
```

## Task 4: Define V2 response, protocol, serialization, and stream primitives

**Files:**

- Create: `src/TorBoxSDK.V2/Compatibility/IsExternalInit.cs`
- Create: `src/TorBoxSDK.V2/Models/Common/TorBoxResponse.cs`
- Create: `src/TorBoxSDK.V2/Models/Common/TorBoxResponseOfT.cs`
- Create: `src/TorBoxSDK.V2/Models/Common/TorBoxProtocolException.cs`
- Create: `src/TorBoxSDK.V2/Models/Common/TorBoxStreamResponse.cs`
- Create: `src/TorBoxSDK.V2/Serialization/TorBoxJsonOptions.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Models/Common/TorBoxResponseTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Models/Common/TorBoxStreamResponseTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Models/Common/TorBoxProtocolExceptionTests.cs`

**Interfaces:**

- Consumes: no V1 `TorBoxException` or `TorBoxErrorCode` type.
- Produces: immutable JSON envelope metadata, an owning stream-response envelope, and the sole V2 protocol exception used by later transport and resource clients.

- [ ] **Step 1: Write response tests before creating the response types.**

```csharp
[Fact]
public void TorBoxResponse_WithStructuredApiFailure_PreservesTheEnvelopeAndStatus()
{
    TorBoxResponse response = new()
    {
        Success = false,
        Error = "BAD_TOKEN",
        Detail = "Invalid token.",
        StatusCode = HttpStatusCode.Unauthorized
    };

    Assert.False(response.Success);
    Assert.Equal("BAD_TOKEN", response.Error);
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

- [ ] **Step 2: Run the response test and confirm the missing V2 type causes failure.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter FullyQualifiedName~TorBoxResponseTests`

Expected: FAIL because `TorBoxResponse` does not exist in the V2 core.

- [ ] **Step 3: Implement immutable response types with non-wire HTTP metadata.**

Implement the two standalone sealed records shown above. Use explicit `[JsonPropertyName]` attributes for `success`, `error`, `detail`, and `data`. Mark `HttpStatusCode` `[JsonIgnore]` and keep its setter public `init`, so the serializer and external tests can create immutable values without a special factory. Add the `IsExternalInit` compatibility shim only for `NETSTANDARD2_0` if records or `init` accessors require it.

- [ ] **Step 4: Write and implement stream ownership behavior.**

```csharp
[Fact]
public void Dispose_ClosesTheOwnedContentStream()
{
    MemoryStream stream = new();
    TorBoxStreamResponse response = TorBoxStreamResponse.CreateForTesting(stream, HttpStatusCode.OK);

    response.Dispose();

    Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
}
```

`TorBoxStreamResponse` must expose `Success`, bounded `Error`/`Detail`, nullable `Stream`, media type, optional length, optional file name, optional `RedirectUri`, and status code. A successful non-redirect stream has a non-null `Stream`; a structured API failure has `Stream == null`; a redirect may have no stream but exposes its parsed `Location` as `RedirectUri`. It internally owns the successful `HttpResponseMessage` and disposes it together with its content stream, without publicly exposing either. Its constructor is internal; only the testing factory is internal and visible to the V2 unit-test assembly through `InternalsVisibleTo`. Add tests for success disposal, a structured stream failure, and redirect metadata.

- [ ] **Step 5: Write and implement protocol-exception behavior.**

`TorBoxProtocolException` must include the request URI, HTTP status when known, and a bounded diagnostic detail. It must not retain or expose an `HttpResponseMessage`.

- [ ] **Step 6: Create the shared serializer options.**

Create one reusable `JsonSerializerOptions` instance. It must not be created per request, must preserve explicit property mappings, and must compile on every V2 target.

- [ ] **Step 7: Run the primitive tests across all executable targets.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~Models.Common"`

Expected: PASS on .NET 6 through .NET 10; the core `netstandard2.0` target compiles during the solution build.

- [ ] **Step 8: Commit the V2 public primitives.**

```bash
git add src/TorBoxSDK.V2/Compatibility src/TorBoxSDK.V2/Models/Common src/TorBoxSDK.V2/Serialization tests/TorBoxSDK.V2.UnitTests/Models/Common
git commit -m "feat: add V2 response and stream primitives"
```

## Task 5: Implement bounded, envelope-preserving HTTP transport

**Files:**

- Create: `src/TorBoxSDK.V2/Http/ITorBoxApiTransport.cs`
- Create: `src/TorBoxSDK.V2/Http/TorBoxApiTransport.cs`
- Create: `src/TorBoxSDK.V2/Http/TorBoxEnvelopeJsonConverterFactory.cs`
- Create: `src/TorBoxSDK.V2/Http/BoundedDiagnosticReader.cs`
- Create: `src/TorBoxSDK.V2/Http/HttpContentStreamReader.cs`
- Create: `src/TorBoxSDK.V2/Http/Handlers/AuthHandler.cs`
- Create: `src/TorBoxSDK.V2/Http/TorBoxHttpClientHandlerFactory.cs`
- Create: `src/TorBoxSDK.V2/Http/QueryStringBuilder.cs`
- Create: `tests/TorBoxSDK.V2.Testing/RecordingHttpMessageHandler.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Http/TorBoxApiTransportTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Http/HttpContentStreamReaderTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Http/AuthHandlerTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Http/QueryStringBuilderTests.cs`

**Interfaces:**

- Consumes: `TorBoxResponse`, `TorBoxProtocolException`, `TorBoxStreamResponse`, and `TorBoxJsonOptions` from Task 4.
- Produces: `ITorBoxApiTransport` for internal resource clients; a 64 KiB maximum retained protocol diagnostic; response-as-value behavior for structured JSON failures (including stream operations); and no public HTTP-message exposure.

- [ ] **Step 1: Write the failing known-error envelope tests.**

Cover a JSON `success:false` body returned with 200, 401, and 500 status codes. Each assertion must expect a `TorBoxResponse<T>` or `TorBoxResponse` with `Success == false`, preserved `Error`/`Detail`, and the original status code; none may expect `TorBoxProtocolException`.

```csharp
[Theory]
[InlineData(HttpStatusCode.OK)]
[InlineData(HttpStatusCode.Unauthorized)]
[InlineData(HttpStatusCode.InternalServerError)]
public async Task SendAsync_WithStructuredTorBoxFailure_ReturnsFailureEnvelope(HttpStatusCode statusCode)
{
    RecordingHttpMessageHandler handler = RecordingHttpMessageHandler.Json(statusCode, """{\"success\":false,\"error\":\"BAD_TOKEN\",\"detail\":\"Invalid token.\"}""");
    using HttpClient client = new(handler) { BaseAddress = new Uri("https://api.torbox.app/v1/api/") };

    TorBoxResponse<string> response = await _transport.SendAsync<string>(client, new HttpRequestMessage(HttpMethod.Get, "user/me"), CancellationToken.None);

    Assert.False(response.Success);
    Assert.Equal(statusCode, response.StatusCode);
}
```

Add the equivalent `SendStreamAsync` cases: when a stream endpoint returns one of those structured JSON envelopes, it returns a disposable `TorBoxStreamResponse` with `Success == false`, `Stream == null`, and the original status rather than throwing.

- [ ] **Step 2: Write the failing bounded-diagnostic and property-order tests.**

Use bodies where `detail` appears before `success`, after `success`, and after a large ignored property. Assert that retained diagnostic text is at most 65,536 UTF-8 bytes and that parsing does not depend on property order. Use `RecordingHttpMessageHandler` to expose how many bytes were read when the body is a protocol failure.

- [ ] **Step 3: Implement streaming JSON envelope decoding.**

Send JSON requests with `HttpCompletionOption.ResponseHeadersRead`. Implement an envelope converter that reads top-level properties in any order, streams values from `HttpContent`, and bounds retained `error`/`detail` diagnostics to 64 KiB. Do not call `ReadAsStringAsync` for response handling. `HttpContentStreamReader.ReadAsync(HttpContent, CancellationToken)` is the only compatibility point: it checks cancellation first, calls `ReadAsStreamAsync()` for `NETSTANDARD2_0`, and calls the cancellation-aware overload on newer targets. A malformed envelope, unsupported content type, or unreadable JSON produces `TorBoxProtocolException` with bounded diagnostic data.

```csharp
using HttpResponseMessage response = await httpClient.SendAsync(
    request,
    HttpCompletionOption.ResponseHeadersRead,
    cancellationToken).ConfigureAwait(false);

using Stream body = await HttpContentStreamReader.ReadAsync(response.Content, cancellationToken).ConfigureAwait(false);
TorBoxResponse<T> envelope = await _serializer.DeserializeEnvelopeAsync<T>(body, response.StatusCode, cancellationToken).ConfigureAwait(false);
```

- [ ] **Step 4: Write and implement stream-response tests.**

Test that `SendStreamAsync` returns a still-open successful `TorBoxStreamResponse`, preserves its mapped content metadata and status, disables automatic redirect following through `TorBoxHttpClientHandlerFactory`, and transfers disposal ownership to the returned object. Verify that the transport disposes its `HttpResponseMessage` after a JSON-envelope result but leaves it owned by `TorBoxStreamResponse` after a successful stream or redirect.

- [ ] **Step 5: Write and implement authentication/query tests.**

Test that `AuthHandler` adds exactly one Bearer header from options, rejects a blank key before sending, does not write the key to diagnostics, and that `QueryStringBuilder` URI-escapes names/values while omitting null values. Add a cancellation test proving the supplied token reaches the final `HttpClient.SendAsync` call, and a recording-handler test proving that one SDK call produces exactly one HTTP send with no hidden retry or backoff.

- [ ] **Step 6: Run transport tests.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~Http"`

Expected: PASS with no request to TorBox and no test relying on field order in a JSON envelope.

- [ ] **Step 7: Commit the transport foundation.**

```bash
git add src/TorBoxSDK.V2/Http tests/TorBoxSDK.V2.Testing tests/TorBoxSDK.V2.UnitTests/Http
git commit -m "feat: add V2 envelope-preserving HTTP transport"
```

## Task 6: Implement options and the approved root hierarchy

**Files:**

- Create: `src/TorBoxSDK.V2/ITorBoxClient.cs`
- Create: `src/TorBoxSDK.V2/TorBoxClient.cs`
- Create: `src/TorBoxSDK.V2/TorBoxClientOptions.cs`
- Create: `src/TorBoxSDK.V2/Internal/TorBoxClientFactory.cs`
- Create: `src/TorBoxSDK.V2/Properties/AssemblyInfo.cs`
- Create: `src/TorBoxSDK.V2/Main/IMainApiClient.cs`
- Create: `src/TorBoxSDK.V2/Main/MainApiClient.cs`
- Create: `src/TorBoxSDK.V2/Search/ISearchApiClient.cs`
- Create: `src/TorBoxSDK.V2/Search/SearchApiClient.cs`
- Create: `src/TorBoxSDK.V2/Relay/IRelayApiClient.cs`
- Create: `src/TorBoxSDK.V2/Relay/RelayApiClient.cs`
- Create: one empty public interface and one internal implementation file for each Main resource: `General`, `Torrents`, `Usenet`, `WebDownloads`, `User`, `Notifications`, `Rss`, `Stream`, `Integrations`, `Vendors`, and `Queued`.
- Create: `tests/TorBoxSDK.V2.UnitTests/TorBoxClientTests.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/Configuration/TorBoxClientOptionsTests.cs`

**Interfaces:**

- Consumes: internal transport/authentication from Task 5.
- Produces: a disposable `ITorBoxClient` with `.Main`, `.Search`, and `.Relay`; all approved Main resource properties; direct construction with `TorBoxClientOptions`; an internal DI construction bridge; and no endpoint methods until the resource plans add contract-backed methods.

- [ ] **Step 1: Write failing hierarchy tests.**

```csharp
[Fact]
public void Constructor_WithValidOptions_ExposesAllApprovedApiFamilies()
{
    using TorBoxClient client = new(CreateValidOptions());

    Assert.NotNull(client.Main);
    Assert.NotNull(client.Search);
    Assert.NotNull(client.Relay);
    Assert.NotNull(client.Main.Torrents);
    Assert.NotNull(client.Main.WebDownloads);
}
```

- [ ] **Step 2: Write failing option-validation tests.**

Test null/empty API key, non-absolute URLs, base URLs missing a trailing slash, and non-positive timeout. Keep configuration-bindable option properties mutable; validate and normalize them once before client construction. Freeze the three default fully resolved family base URLs in tests only after checking them against the reviewed baseline and official API documentation; do not copy V1 path composition without that evidence. Resource methods receive resolved family clients and never concatenate version or token data.

- [ ] **Step 3: Implement `TorBoxClientOptions` and direct client ownership.**

`TorBoxClient` creates three internal family clients, each with a separately configured `HttpClient` and the internal `AuthHandler`. It owns and disposes these clients only in direct-construction mode. `ITorBoxClient` implements `IDisposable`. Add the internal `TorBoxClientFactory` overload that accepts the three factory-managed clients and an ownership flag; expose it only to `TorBoxSDK.DependencyInjection` and `TorBoxSDK.V2.UnitTests` through explicit `InternalsVisibleTo` entries in the core assembly information. It must not expose those `HttpClient` instances or set authorization headers in resource methods.

- [ ] **Step 4: Implement empty V2 family/resource facades.**

Create the exact public property tree approved in the V2 design. Resource interfaces contain no endpoint methods until their coverage records are assigned in the Main/Search/Relay plans. This task provides the final navigation shape without falsely claiming endpoint coverage.

- [ ] **Step 5: Run hierarchy and configuration tests.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~TorBoxClient|FullyQualifiedName~TorBoxClientOptions"`

Expected: PASS without network access.

- [ ] **Step 6: Commit the V2 root client.**

```bash
git add src/TorBoxSDK.V2/ITorBoxClient.cs src/TorBoxSDK.V2/TorBoxClient.cs src/TorBoxSDK.V2/TorBoxClientOptions.cs src/TorBoxSDK.V2/Internal src/TorBoxSDK.V2/Properties src/TorBoxSDK.V2/Main src/TorBoxSDK.V2/Search src/TorBoxSDK.V2/Relay tests/TorBoxSDK.V2.UnitTests/TorBoxClientTests.cs tests/TorBoxSDK.V2.UnitTests/Configuration
git commit -m "feat: add V2 client hierarchy and options"
```

## Task 7: Implement the optional DI package

**Files:**

- Create: `src/TorBoxSDK.DependencyInjection.V2/TorBoxServiceCollectionExtensions.cs`
- Create: `tests/TorBoxSDK.V2.UnitTests/DependencyInjection/TorBoxServiceCollectionExtensionsTests.cs`
- Modify: `tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj`

**Interfaces:**

- Consumes: `ITorBoxClient`, `TorBoxClientOptions`, and the core's internal DI construction hook.
- Produces: `IServiceCollection AddTorBox(this IServiceCollection, Action<TorBoxClientOptions>)` and `IServiceCollection AddTorBox(this IServiceCollection, IConfiguration)` from the separate DI assembly.

- [ ] **Step 1: Write failing registration tests.**

```csharp
[Fact]
public void AddTorBox_RegistersOnlyTheRootClient()
{
    ServiceCollection services = new();
    services.AddTorBox(static options => options.ApiKey = "test-key");

    using ServiceProvider provider = services.BuildServiceProvider();

    Assert.NotNull(provider.GetRequiredService<ITorBoxClient>());
    Assert.Null(provider.GetService<IMainApiClient>());
    Assert.Null(provider.GetService<ISearchApiClient>());
    Assert.Null(provider.GetService<IRelayApiClient>());
}
```

- [ ] **Step 2: Run the registration test and confirm it fails before the extension exists.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter FullyQualifiedName~TorBoxServiceCollectionExtensionsTests`

Expected: FAIL because `AddTorBox` is absent from the V2 DI package.

- [ ] **Step 3: Implement delegate and configuration-section registration.**

Register named Main/Search/Relay HTTP clients, their private authentication handlers, V2 options validation, and a transient `ITorBoxClient` created through the internal core factory. Bind the configuration overload from the `TorBox` section. Do not register family/resource interfaces individually.

- [ ] **Step 4: Add test cases for option errors and HTTP-client separation.**

Assert that invalid options fail before resolution/sending and that the three named client configurations receive their own base URL. Use an injected fake primary handler; do not call the network.

- [ ] **Step 5: Run all DI tests.**

Run: `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter FullyQualifiedName~DependencyInjection`

Expected: PASS.

- [ ] **Step 6: Commit the separate DI package.**

```bash
git add src/TorBoxSDK.DependencyInjection.V2 tests/TorBoxSDK.V2.UnitTests/DependencyInjection tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj
git commit -m "feat: add V2 dependency injection package"
```

## Task 8: Add V2 deterministic checks and align repository instructions

**Files:**

- Create: `eng/Invoke-V2DeterministicChecks.ps1`
- Create: `.github/instructions/torboxsdk-v2.instructions.md`
- Modify: `.agents/skills/tests/SKILL.md`
- Modify: `.agents/skills/dev/references/dev-jobs.md`
- Modify: `.agents/skills/dev/references/development-playbooks.md`
- Modify: `.github/workflows/ci.yml`
- Modify: `.github/workflows/integration-tests.yml`

**Interfaces:**

- Consumes: `TorBoxSDK.V2.slnx`, V2 test projects, versioned baseline, and V2 package projects.
- Produces: an offline V2 gate that never downloads OpenAPI, an opt-in live workflow, and instructions that distinguish V1 legacy behavior from the approved V2 rules.

- [ ] **Step 1: Write the failing deterministic-check smoke test.**

Create a PowerShell test invocation that fails immediately when `contracts/torbox/baseline/manifest.json` is absent, when the V2 lock files are missing or stale, when the V2 solution does not build, or when unit/non-release-contract tests fail.

```powershell
$ErrorActionPreference = 'Stop'

& dotnet restore TorBoxSDK.V2.slnx --locked-mode
if ($LASTEXITCODE -ne 0) { throw 'V2 restore failed.' }
& dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw 'V2 build failed.' }
& dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-build
if ($LASTEXITCODE -ne 0) { throw 'V2 unit tests failed.' }
& dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-build --filter "Category!=Release"
if ($LASTEXITCODE -ne 0) { throw 'V2 contract tests failed.' }
```

- [ ] **Step 2: Implement `Invoke-V2DeterministicChecks.ps1`.**

Add `-SkipPack` and `-IncludeReleaseContract` switches. Default behavior runs restore, build, V2 unit tests, and non-release V2 contract tests. `-IncludeReleaseContract` runs the release-trait test only after the final cutover plan declares it eligible. The script must not invoke `UpdateTorBoxContract.ps1 -Refresh`, integration tests, live tests, or any TorBox HTTP call. Normal locked NuGet restore remains permitted; after restore, every remaining step is local.

- [ ] **Step 3: Add exact-package inspection.**

When packing is not skipped, pack both V2 packages to the exact repository-relative `artifacts/v2-packages/` directory after proving that path is below the repository root. Clear only that resolved directory, then inspect the `.nupkg` contents and nuspec metadata. Assert that the core package has no Microsoft.Extensions dependency and that the DI package has the required core dependency and only its reviewed Microsoft.Extensions dependencies; reject unexpected production dependencies. Do not publish either package.

- [ ] **Step 4: Create V2-specific engineering rules and correct stale test guidance.**

`torboxsdk-v2.instructions.md` must state the V2 target matrix, response-as-value error policy, separate DI package, streaming ownership, and checked-in contract baseline. Update the `tests` skill and dev references so V2 static tests read the baseline rather than downloading `openapi.json`; leave V1 legacy instructions explicitly labeled until cutover.

- [ ] **Step 5: Add CI jobs without changing V1 publishing behavior.**

Add a V2 deterministic job that invokes `eng/Invoke-V2DeterministicChecks.ps1`. Keep live validation in a manually dispatched V2 job requiring the protected `TORBOX_API_KEY` environment. Do not add NuGet publish steps for the side-by-side V2 projects.

- [ ] **Step 6: Run the deterministic script locally.**

Run: `pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1`

Expected: PASS with no API key and no TorBox/OpenAPI network call. The initial locked restore may contact configured NuGet sources; all later steps are local.

- [ ] **Step 7: Commit the foundation gate and V2 guidance.**

```bash
git add eng/Invoke-V2DeterministicChecks.ps1 .github/instructions/torboxsdk-v2.instructions.md .agents/skills/tests/SKILL.md .agents/skills/dev/references/dev-jobs.md .agents/skills/dev/references/development-playbooks.md .github/workflows/ci.yml .github/workflows/integration-tests.yml
git commit -m "ci: add deterministic V2 foundation gate"
```

## Task 9: Verify and review the V2 foundation before resource implementation

**Files:**

- Verify: all files created by Tasks 1–8
- Review: `src/TorBoxSDK.V2/`, `src/TorBoxSDK.DependencyInjection.V2/`, `tests/TorBoxSDK.V2.UnitTests/`, `tests/TorBoxSDK.V2.ContractTests/`, and `eng/Invoke-V2DeterministicChecks.ps1`

**Interfaces:**

- Consumes: every foundational deliverable.
- Produces: a review-approved V2 foundation and an exact, checked-in operation inventory for the Main/Search/Relay plans.

- [ ] **Step 1: Run the complete offline foundation gate.**

Run: `pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1`

Expected: exit code 0; V2 core compiles for `netstandard2.0` and .NET 6–10; unit and contract tests pass without a TorBox credential.

- [ ] **Step 2: Verify no deterministic V2 path fetches OpenAPI.**

Run: `rg -n "api\.torbox\.app/openapi\.json|Invoke-WebRequest|Invoke-RestMethod|UpdateTorBoxContract.*-Refresh" tests/TorBoxSDK.V2.ContractTests eng/Invoke-V2DeterministicChecks.ps1`

Expected: no match (exit code 1) in deterministic test/build paths. Verify separately that the only intended source URL is in `tools/UpdateTorBoxContract.ps1`.

- [ ] **Step 3: Review the V2 C# changes.**

Run the `code-review` skill on every changed V2 `.cs` file. Resolve every CRITICAL or MAJOR finding before continuing. Confirm that known TorBox API errors return envelopes rather than `TorBoxException`, and that no V2 public surface exposes raw HTTP messages.

- [ ] **Step 4: Confirm the contract inventory is ready for resource planning.**

Run: `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --filter "Category!=Release"`

Expected: every snapshot operation has one coverage record with a family/resource and no stale record.

- [ ] **Step 5: Commit only scoped review fixes, if any.**

```bash
git add TorBoxSDK.V2.slnx src/TorBoxSDK.V2 src/TorBoxSDK.DependencyInjection.V2 src/TorBoxSDK.V2.Examples tests/TorBoxSDK.V2.UnitTests tests/TorBoxSDK.V2.ContractTests tests/TorBoxSDK.V2.IntegrationTests tests/TorBoxSDK.V2.Testing contracts/torbox tools/UpdateTorBoxContract.ps1 eng/Invoke-V2DeterministicChecks.ps1 .github/instructions/torboxsdk-v2.instructions.md .agents/skills/tests/SKILL.md .agents/skills/dev/references/dev-jobs.md .agents/skills/dev/references/development-playbooks.md .github/workflows/ci.yml .github/workflows/integration-tests.yml
git commit -m "chore: verify V2 foundation checkpoint"
```

Run the command only if the review produced fixes. Do not create a tag, publish a NuGet package, or publish a public prerelease at this checkpoint.

## Downstream handoff

After Task 9, use `contracts/torbox/coverage.json` as the exact input for the next plans. Each Main resource plan must change its records from `Planned` to `Implemented` only after the handwritten public interface, model mappings, implementation, deterministic unit tests, and resource-level review all pass. The Search/Relay plan applies the same rule. The final cutover plan alone enables the `Category=Release` contract test and moves V2 projects to the final V1 paths.
