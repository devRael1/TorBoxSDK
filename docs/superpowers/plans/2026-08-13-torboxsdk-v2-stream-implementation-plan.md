# TorBoxSDK V2 Stream Resource Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (- [ ]) syntax for tracking.

**Goal:** Implement the two JSON Main Stream operations with typed requests,
typed responses, reviewed contract mappings, and deterministic multi-target
verification.

**Architecture:** Stream stays behind ITorBoxClient.Main.Stream. Immutable
request records make the two stream-access tokens named rather than positional.
The internal StreamClient maps them to relative GET queries and delegates to
the existing envelope-preserving JSON transport. Before the first route becomes
Implemented, contract validation moves from runtime-specific
AssemblyQualifiedName values to a recursive canonical identity stable from
net6.0 through net10.0.

**Tech Stack:** C#, .NET Standard 2.0 and .NET 6 through .NET 10,
System.Text.Json, HttpClient, xUnit, checked-in OpenAPI contracts, and the
PowerShell deterministic gate.

## Global Constraints

- Work only on codex/v2-redesign-foundation in the existing PR worktree. Never
  modify or push master.
- Core targets netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0. Tests run
  on net6.0 through net10.0.
- Add no production dependency to the core package.
- A valid V2 JSON envelope is a response value, including success:false and
  structured JSON HTTP error statuses.
- All tests are offline and use an in-memory handler. Do not refresh
  contracts, call TorBox, execute integration tests, access an API key, or log
  tokens.
- Public types have XML documentation, matching file names, file-scoped
  namespaces, and correct nullability. Library awaits use ConfigureAwait(false).
- CreateStreamRequest omits every null option so TorBox supplies its defaults.
- Both methods use ITorBoxApiTransport.SendAsync<T>; neither uses
  TorBoxStreamResponse or SendStreamAsync.
- StreamMetadata.Video and StreamData.SearchMetadata are JsonElement?. Do not
  invent schemas for them.

---

## File Structure

| File | Responsibility |
| --- | --- |
| tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentity.cs | Stable reflection identity for coverage result types. |
| tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentityTests.cs | Tests the canonical generic-name grammar. |
| tests/TorBoxSDK.V2.ContractTests/Infrastructure/CoverageManifest.cs | Validates canonical Task result types. |
| tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs | Compares method return types using canonical identities. |
| src/TorBoxSDK.V2/Models/Stream/*.cs | Stream requests and response models. |
| src/TorBoxSDK.V2/Main/Stream/IStreamClient.cs | Public Stream signatures. |
| src/TorBoxSDK.V2/Main/Stream/StreamClient.cs | Validation, query mapping, and transport delegation. |
| tests/TorBoxSDK.V2.UnitTests/Main/Stream/StreamClientTests.cs | Offline Stream behavior tests. |
| contracts/torbox/coverage.json | Implemented mappings for both Stream routes. |
| contracts/torbox/divergences.json | Evidence for the empty response schemas. |

### Task 1: Stabilize contract result-type identities

**Files:**

- Create: tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentity.cs
- Create: tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentityTests.cs
- Modify: tests/TorBoxSDK.V2.ContractTests/Infrastructure/CoverageManifest.cs
- Modify: tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs
- Modify: tests/TorBoxSDK.V2.ContractTests/OperationCoverageTests.cs

**Interfaces:**

- Consumes a runtime System.Type from a public interface method.
- Produces CanonicalTypeIdentity.Format(Type type).
- Generic identities retain generic arity but exclude assembly metadata. Example:
  System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[System.String]].

- [ ] **Step 1: Write failing canonical identity tests.**

Create CanonicalTypeIdentityTests:

~~~csharp
[Fact]
public void Format_WithGenericTaskEnvelope_ReturnsVersionIndependentIdentity()
{
    // Arrange
    Type type = typeof(Task<TorBoxResponse<string>>);

    // Act
    string identity = CanonicalTypeIdentity.Format(type);

    // Assert
    Assert.Equal(
        "System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[System.String]]",
        identity);
}

[Fact]
public void Format_WithNonGenericType_ReturnsTheFullName()
{
    // Arrange
    Type type = typeof(CancellationToken);

    // Act
    string identity = CanonicalTypeIdentity.Format(type);

    // Assert
    Assert.Equal("System.Threading.CancellationToken", identity);
}
~~~

- [ ] **Step 2: Run the focused test and capture RED.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CanonicalTypeIdentityTests"
~~~

Expected: compilation failure because CanonicalTypeIdentity is absent.

- [ ] **Step 3: Implement the formatter.**

Implement this internal static method:

~~~csharp
internal static string Format(Type type)
{
    if (type is null)
    {
        throw new ArgumentNullException(nameof(type));
    }

    if (!type.IsGenericType)
    {
        return type.FullName
            ?? throw new InvalidOperationException("The type does not have a full name.");
    }

    Type definition = type.GetGenericTypeDefinition();
    string definitionName = definition.FullName
        ?? throw new InvalidOperationException("The generic type definition does not have a full name.");
    string arguments = string.Join(",", type.GetGenericArguments().Select(Format));
    return string.Concat(definitionName, "[", arguments, "]");
}
~~~

- [ ] **Step 4: Add result-type validator RED cases.**

In OperationCoverageTests, add an implemented record with
System.Threading.Tasks.Task`1[System.String] that loads successfully, and
an otherwise valid record with System.Threading.Tasks.TaskFake that throws
InvalidDataException. Keep the normal temporary-file cleanup behavior.

- [ ] **Step 5: Update contract validation and reflection comparison.**

In CoverageManifest.ValidateState, accept only:

~~~csharp
resultType == "System.Threading.Tasks.Task"
    || resultType.StartsWith("System.Threading.Tasks.Task`1[", StringComparison.Ordinal)
~~~

In ContractSurfaceTests compare:

~~~csharp
Assert.Equal(resultType, CanonicalTypeIdentity.Format(method.ReturnType));
~~~

Keep exact ordered parameter type resolution and public interface validation.

- [ ] **Step 6: Run the contract subset and verify GREEN.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CanonicalTypeIdentityTests|FullyQualifiedName~OperationCoverageTests|FullyQualifiedName~ContractSurfaceTests"
~~~

Expected: green on net6.0 through net10.0; the TaskFake regression remains a
passing test because it proves the manifest rejects the bad record.

- [ ] **Step 7: Commit Task 1.**

~~~powershell
git add tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentity.cs tests/TorBoxSDK.V2.ContractTests/Infrastructure/CanonicalTypeIdentityTests.cs tests/TorBoxSDK.V2.ContractTests/Infrastructure/CoverageManifest.cs tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs tests/TorBoxSDK.V2.ContractTests/OperationCoverageTests.cs
git commit -m "test: stabilize V2 contract result type identities"
~~~

### Task 2: Implement typed Stream requests, responses, and methods

**Files:**

- Create: src/TorBoxSDK.V2/Models/Stream/CreateStreamRequest.cs
- Create: src/TorBoxSDK.V2/Models/Stream/GetStreamDataRequest.cs
- Create: src/TorBoxSDK.V2/Models/Stream/StreamData.cs
- Create: src/TorBoxSDK.V2/Models/Stream/StreamMetadata.cs
- Create: src/TorBoxSDK.V2/Models/Stream/AudioTrackInfo.cs
- Create: src/TorBoxSDK.V2/Models/Stream/SubtitleTrackInfo.cs
- Modify: src/TorBoxSDK.V2/Main/Stream/IStreamClient.cs
- Modify: src/TorBoxSDK.V2/Main/Stream/StreamClient.cs
- Create: tests/TorBoxSDK.V2.UnitTests/Main/Stream/StreamClientTests.cs

**Interfaces:**

- Consumes ITorBoxApiTransport.SendAsync<T>, QueryStringBuilder, and the Main
  HttpClient.
- Produces:

~~~csharp
Task<TorBoxResponse<string>> CreateStreamAsync(
    CreateStreamRequest request,
    CancellationToken cancellationToken = default);

Task<TorBoxResponse<StreamData>> GetStreamDataAsync(
    GetStreamDataRequest request,
    CancellationToken cancellationToken = default);
~~~

- [ ] **Step 1: Write validation and route RED tests.**

Create StreamClientTests under Main/Stream using RecordingHttpMessageHandler,
an HttpClient with BaseAddress https://api.torbox.app/v1/api/, and internal
StreamClient plus TorBoxApiTransport. Add tests for null request to each
method, whitespace Type, whitespace PresignedToken, whitespace Token, and
handler.SendCount equal to zero for every rejected request.

- [ ] **Step 2: Run Stream tests and capture RED.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~StreamClientTests"
~~~

Expected: compilation failure because the models and public methods are absent.

- [ ] **Step 3: Create immutable models.**

Create CreateStreamRequest:

~~~csharp
public sealed record CreateStreamRequest
{
    public long DownloadId { get; init; }
    public long? FileId { get; init; }
    public string? Type { get; init; }
    public int? ChosenSubtitleIndex { get; init; }
    public int? ChosenAudioIndex { get; init; }
    public int? ChosenResolutionIndex { get; init; }
    public bool? ScrobblingEnabled { get; init; }
}
~~~

Create GetStreamDataRequest:

~~~csharp
public sealed record GetStreamDataRequest
{
    public string PresignedToken { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public int? ChosenSubtitleIndex { get; init; }
    public int? ChosenAudioIndex { get; init; }
    public int? ChosenResolutionIndex { get; init; }
}
~~~

Port known V1 Stream data fields into sealed records. Every response property
uses JsonPropertyName. Collections use IReadOnlyList<T> with empty defaults.
Use JsonElement? only for StreamMetadata.Video and
StreamData.SearchMetadata.

- [ ] **Step 4: Add the interface methods and implement StreamClient.**

Add the two signatures above with full XML docs. Persist constructor inputs in
readonly _httpClient and _transport fields after null checks.

Validate before creating HttpRequestMessage. Use the netstandard2.0-compatible
manual null check:

~~~csharp
if (request is null)
{
    throw new ArgumentNullException(nameof(request));
}

if (request.Type is not null && string.IsNullOrWhiteSpace(request.Type))
{
    throw new ArgumentException("The stream type cannot be whitespace.", nameof(request));
}
~~~

For GetStreamDataRequest, reject whitespace PresignedToken and Token with
ArgumentException using nameof(request).

Build CreateStream query in exact order:

~~~csharp
string query = QueryStringBuilder.Build(
    ("id", request.DownloadId.ToString(CultureInfo.InvariantCulture)),
    ("file_id", request.FileId is long fileId ? fileId.ToString(CultureInfo.InvariantCulture) : null),
    ("type", request.Type),
    ("chosen_subtitle_index", request.ChosenSubtitleIndex is int subtitleIndex ? subtitleIndex.ToString(CultureInfo.InvariantCulture) : null),
    ("chosen_audio_index", request.ChosenAudioIndex is int audioIndex ? audioIndex.ToString(CultureInfo.InvariantCulture) : null),
    ("chosen_resolution_index", request.ChosenResolutionIndex is int resolutionIndex ? resolutionIndex.ToString(CultureInfo.InvariantCulture) : null),
    ("scrobbling_enabled", request.ScrobblingEnabled is bool enabled ? (enabled ? "true" : "false") : null));
~~~

Use stream/createstream, a using HttpRequestMessage, and
_transport.SendAsync<string>. Build GetStreamData in order presigned_token,
token, chosen_subtitle_index, chosen_audio_index, chosen_resolution_index;
use stream/getstreamdata and _transport.SendAsync<StreamData>. Do not set an
authorization header, compose a host, or use SendStreamAsync.

- [ ] **Step 5: Add request/response behavior tests.**

Add these named cases:

1. CreateStreamAsync_WithOnlyDownloadId_OmitsAllTorBoxOwnedDefaults asserts GET
   and a URI ending stream/createstream?id=42.
2. CreateStreamAsync_WithExplicitOptions_UsesInvariantEncodedQueryValues passes
   FileId = -7, Type = "usenet & test", all selected indices, and false; it
   asserts percent encoding and lowercase false.
3. GetStreamDataAsync_WithTokensAndOptions_UsesNamedEncodedQueryValues uses
   token data containing plus, slash, question mark, and ampersand; it asserts
   the documented named parameter order and escaped values.
4. GetStreamDataAsync_WithNestedJson_ReturnsTypedDataAndRawJsonElements
   asserts typed tracks plus Video.ValueKind and SearchMetadata.ValueKind.
5. CreateStreamAsync_WithStructuredFailure_ReturnsFailureEnvelope and
   GetStreamDataAsync_WithStructuredFailure_ReturnsFailureEnvelope return JSON
   success:false with HTTP 401, preserving envelope values without exceptions.
6. GetStreamDataAsync_WithCancellationToken_ForwardsItToTheHandler asserts the
   recording handler observed the supplied token.

- [ ] **Step 6: Run the complete Stream unit suite and verify GREEN.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~StreamClientTests"
~~~

Expected: all Stream tests pass on net6.0 through net10.0 without network
access.

- [ ] **Step 7: Commit Task 2.**

~~~powershell
git add src/TorBoxSDK.V2/Models/Stream src/TorBoxSDK.V2/Main/Stream/IStreamClient.cs src/TorBoxSDK.V2/Main/Stream/StreamClient.cs tests/TorBoxSDK.V2.UnitTests/Main/Stream/StreamClientTests.cs
git commit -m "feat: implement V2 Stream resource"
~~~

### Task 3: Map Stream coverage and local response-shape evidence

**Files:**

- Modify: contracts/torbox/coverage.json
- Modify: contracts/torbox/divergences.json
- Modify: tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs

**Interfaces:**

- Consumes Task 1 canonical identities and Task 2 public methods.
- Produces two Implemented Main/Stream records with responseMode json,
  requestType null, and one divergence record per route.

- [ ] **Step 1: Add a Stream contract-surface regression.**

Add a ContractSurfaceTests test that asserts:

~~~csharp
Assert.Equal(
    "System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[System.String]]",
    CanonicalTypeIdentity.Format(
        typeof(IStreamClient).GetMethod(nameof(IStreamClient.CreateStreamAsync))!.ReturnType));

Assert.Equal(
    "System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[TorBoxSDK.Models.Stream.StreamData]]",
    CanonicalTypeIdentity.Format(
        typeof(IStreamClient).GetMethod(nameof(IStreamClient.GetStreamDataAsync))!.ReturnType));
~~~

- [ ] **Step 2: Run the focused contract test.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~ContractSurfaceTests"
~~~

Expected after Task 2: green on net6.0 through net10.0. Before Task 2, the
missing methods are the expected RED.

- [ ] **Step 3: Update both coverage records.**

For GET /v1/api/stream/createstream, set:

~~~json
{
  "publicInterface": "TorBoxSDK.Main.Stream.IStreamClient",
  "publicMethod": "CreateStreamAsync",
  "parameterTypes": [
    "TorBoxSDK.Models.Stream.CreateStreamRequest",
    "System.Threading.CancellationToken"
  ],
  "requestType": null,
  "resultType": "System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[System.String]]",
  "responseMode": "json",
  "implementationState": "Implemented",
  "divergenceIds": ["DIV-STREAM-001"]
}
~~~

For GET /v1/api/stream/getstreamdata, set GetStreamDataAsync,
GetStreamDataRequest then System.Threading.CancellationToken, and:

~~~text
System.Threading.Tasks.Task`1[TorBoxSDK.Models.Common.TorBoxResponse`1[TorBoxSDK.Models.Stream.StreamData]]
~~~

Set divergenceIds to DIV-STREAM-002 and preserve each existing route identity,
sourceId, family, and resource.

- [ ] **Step 4: Create both divergence records from local evidence.**

Capture observedAtUtc through:

~~~powershell
(Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", [System.Globalization.CultureInfo]::InvariantCulture)
~~~

Both records contain non-empty id, operationKey, source, observedAtUtc,
contractBehavior, v2Behavior, and evidence. State that the checked-in Main
OpenAPI snapshot declares application/json with an empty 200 schema, typed
data is informed by local V1 source, and no live response was used. The second
record also records the two JsonElement? fields.

- [ ] **Step 5: Run all offline contract tests and verify GREEN.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-restore --filter "Category!=Release"
~~~

Expected: both mappings resolve on every target, divergence IDs are registered,
and no network request occurs.

- [ ] **Step 6: Commit Task 3.**

~~~powershell
git add contracts/torbox/coverage.json contracts/torbox/divergences.json tests/TorBoxSDK.V2.ContractTests/ContractSurfaceTests.cs
git commit -m "test: map V2 Stream contract coverage"
~~~

### Task 4: Validate, review, and publish the Stream slice

**Files:**

- Verify: every file changed by Tasks 1 through 3.
- Fix only: files implicated by an independent review finding.

**Interfaces:**

- Consumes the completed Stream API, coverage, divergences, and deterministic
  gate.
- Produces a review-approved, pushed Stream slice on the existing PR.

- [ ] **Step 1: Run unit and core-build validation.**

Run:

~~~powershell
dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore
dotnet build src/TorBoxSDK.V2/TorBoxSDK.V2.csproj --configuration Release --no-restore
~~~

Expected: all V2 unit tests pass on net6.0 through net10.0 and the core is
warning-clean across netstandard2.0 through net10.0.

- [ ] **Step 2: Run the secretless deterministic gate.**

Run:

~~~powershell
Remove-Item Env:TORBOX_API_KEY -ErrorAction SilentlyContinue
Remove-Item Env:TORBOXSDK_V2_RELEASE_CONTRACT -ErrorAction SilentlyContinue
pwsh -NoProfile -File eng/Invoke-V2DeterministicChecks.ps1
~~~

Expected: locked restore, Release build, V2 unit tests, non-release contract
tests, and package inspection pass without a TorBox request, refresh,
integration test, publish, or secret.

- [ ] **Step 3: Request independent C# review.**

Use code-review for changed V2 C# and C# project files. Supply the Stream
design, this plan, and the exact changed-file list. Require review of public
API, XML docs, nullability, query encoding, default omission, tokens,
cancellation, response-as-value behavior, JsonElement lifetime, canonical
type identity, and the absence of live calls, retries, raw HTTP exposure, or
secret leakage.

- [ ] **Step 4: Resolve every Critical or Major finding with fresh RED/GREEN.**

For each finding, create the smallest deterministic failing test, run it red,
apply the smallest scoped correction, run it green, then rerun its affected
suite. Do not bundle unrelated cleanup.

- [ ] **Step 5: Commit review fixes only when needed.**

~~~powershell
git add <review-fix-files>
git commit -m "fix: address V2 Stream review findings"
~~~

Skip this commit if review produces no required code or test changes.

- [ ] **Step 6: Verify final branch state and publish to the existing PR.**

Run:

~~~powershell
git diff --check
git status --short
git log -1 --oneline
git push origin codex/v2-redesign-foundation
git ls-remote --heads origin codex/v2-redesign-foundation
~~~

Expected: no remaining task changes, remote SHA equals local HEAD, and no new
PR is created.

## Self-review checklist

- Task 1 removes the multi-runtime coverage blocker before a Stream mapping is
  Implemented.
- Task 2 covers every approved model, request, validation, query, transport,
  and response rule.
- Task 3 records local evidence only and makes no live claim.
- Task 4 requires fresh tests, the offline gate, independent review, and
  remote verification.
- No task changes V1, adds binary playback, invents a response schema, adds a
  core dependency, introduces retries, exposes a secret, or pushes master.
