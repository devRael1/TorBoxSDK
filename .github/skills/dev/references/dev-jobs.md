# Dev Jobs Reference

Complete definition of the 6 recurring job types for TorBoxSDK, including their cadence, execution order, expected output, and completion checklist.

> This file does not duplicate the content of specialized skills. It defines the role of each job in the development chain and the handoff between jobs.

---

## Overview

| Job | Primary Output | Typical Next Job |
|-----|-------------------|----------------------|
| J1 Architecture | target structure and responsibility placement | J2 or J6 |
| J2 Endpoint | working code for an endpoint or a slice | J3 |
| J3 Tests | safety net and behavior validation | J4 |
| J4 Code Review | validation verdict before merge | J5 or end of task |
| J5 Docs & Packaging | user-facing surface and release readiness | end of task or release |
| J6 Foundation | buildable and injectable SDK foundation | J1 then J2 |

---

## J1 — Architecture

**When:** before writing code for a new component, or when the existing structure is being refactored.

**Skill to load:** `.github/skills/architecture/SKILL.md`

**Recurring jobs:**
- Define or adapt the `TorBoxClient → Main/Search/Relay → resource clients` hierarchy
- Decide the public surface of a new resource client (interface + implementation)
- Design a new exception or error type
- Refactor DI (`AddTorBox()`) when dependencies change
- Decide on namespace splitting or sub-package creation

**J1 completion checklist:**
- [ ] Client hierarchy is respected (no endpoint directly on `TorBoxClient`)
- [ ] Public interfaces are defined before implementations
- [ ] No duplication of transport logic across resource clients
- [ ] Namespaces follow the `TorBoxSDK.<Layer>.<Resource>` convention
- [ ] Multi-target net6.0→net10.0 is preserved (no runtime API unavailable on net6.0)

---

## J2 — Endpoint Implementation

**When:** for each TorBox endpoint to add according to the development plan (`docs/TODO.md`).

**Workflow to use:** `J2` in `/dev`, with references:
- `./endpoint-placement-and-naming.md`
- `./endpoint-implementation-checklist.md`

**Recommended development order (by phase, consistent with `docs/TODO.md`):**

### Phase 1 — Foundations (before any endpoint)
1. Multi-targeting `net6.0;net7.0;net8.0;net9.0;net10.0`
2. `Directory.Build.props` + `.editorconfig`
3. `TorBoxClient`, `ITorBoxClient`, `TorBoxClientOptions`
4. `MainApiClient`, `SearchApiClient`, `RelayApiClient`
5. Empty resource clients: `TorrentsClient`, `UsenetClient`, `WebDownloadsClient`, `UserClient`, `NotificationsClient`, `RssClient`, `StreamClient`, `IntegrationsClient`, `VendorsClient`, `QueuedClient`
6. `TorBoxResponse<T>`, `TorBoxResponse`, `TorBoxException`, `TorBoxErrorCode`
7. `AuthHandler` (DelegatingHandler), `AddTorBox()` DI

### Phase 2 — Models (by resource family)
Implement models in this order: **Common → Torrents → Usenet → WebDownloads → User → General → Notifications → RSS → Stream → Integrations → Vendors → Queued**

For each resource:
- [ ] Primary model (e.g., `Torrent`)
- [ ] Request models (e.g., `CreateTorrentRequest`, `ControlTorrentRequest`)
- [ ] Resource-specific enums if needed

### Phase 3 — Main API Services (by resource client)
For each resource client, recommended order: **Torrents → Usenet → WebDownloads → User → General → Notifications → RSS → Stream → Integrations → Vendors → Queued**

For each endpoint:
- [ ] Method on the `ITorrentsClient` interface
- [ ] Implementation in `TorrentsClient`
- [ ] Unit tests (J3) immediately after

### Phase 4 — Search API
- Models: `SearchResult`, `TorrentSearchResult`, `UsenetSearchResult`, `MetaSearchResult`, enums
- Endpoints: `SearchTorrentsAsync`, `SearchUsenetAsync`, `SearchMetaAsync`, torznab/newznab

### Phase 5 — Relay API
- Models: `InactiveCheckResult`
- Endpoints: `GetStatusAsync`, `CheckForInactiveAsync`

**J2 completion checklist (per endpoint):**
- [ ] Request model created (if complex parameters) with `[JsonPropertyName]` on each property
- [ ] Response model created (or justified reuse) with `init` properties
- [ ] Method added to the interface with complete XML doc
- [ ] Implementation in the correct resource client, without hardcoded URLs
- [ ] `CancellationToken ct = default` as the last parameter, propagated to `_httpClient`
- [ ] Deserialization via `TorBoxResponse<T>`
- [ ] API errors mapped to `TorBoxException` with `TorBoxErrorCode`
- [ ] Required input parameter validation (`ArgumentNullException.ThrowIfNull`)

---

## J3 — Tests

**When:** immediately after each implemented endpoint (J2), or to improve existing coverage.

**Skill to load:** `.github/skills/tests/SKILL.md`

**Recurring jobs and cadence:**

| Type | When | Location |
|------|-------|-------------|
| Unit tests | After each J2 implementation | `tests/TorboxSDK.UnitTests/` |
| Serialization tests | After each new model (J2 Phase 2) | `tests/TorboxSDK.UnitTests/Models/` |
| Integration tests | After stabilization of a complete resource client | `tests/TorBoxSDK.IntegrationTests/` |
| Performance tests | On critical paths (serialization, HTTP) | `tests/TorBoxSDK.PerformanceTests/` |

**For each endpoint (unit tests):**
- [ ] Happy path: `success: true` response deserialized correctly
- [ ] API error: `success: false` → `TorBoxException` thrown with correct `ErrorCode`
- [ ] HTTP 4xx/5xx → typed exception
- [ ] Required null parameters → `ArgumentNullException`
- [ ] Correct URL + HTTP method (verified via `MockHttpMessageHandler`)
- [ ] Authentication headers present in the request

**J3 completion checklist:**
- [ ] No test makes a real HTTP call (mocked `DelegatingHandler`)
- [ ] Each test follows `MethodName_State_ExpectedBehavior`
- [ ] AAA structure with `// Arrange` / `// Act` / `// Assert` comments
- [ ] Integration tests marked `[Trait("Category", "Integration")]` and skipped without `TORBOX_API_KEY`
- [ ] `dotnet test` passes without errors

---

## J4 — Code Review

**When:** before considering a development complete. **Always after J2 and J3.**

**Skill to load:** `.github/skills/code-review/SKILL.md`

**Recurring jobs:**
- Review of a modified file before merge
- Full audit of a resource client after implementation
- Compliance verification after refactoring

**Recommended cadence:**
- After each endpoint (J2): review of the resource client file + models
- After each batch of tests (J3): review of the test file
- Before each phase (e.g., transition from Phase 2 → Phase 3): audit of the relevant directory

**J4 completion checklist:**
- [ ] `APPROVED` or `APPROVED WITH MINOR ISSUES` verdict obtained on all modified files
- [ ] No unresolved CRITICAL or MAJOR finding

---

## J5 — Docs & Packaging

**When:** after stabilization of a complete resource client, and mandatory before any NuGet release.

**Skill to load:** `.github/skills/docs/SKILL.md`

**Recurring jobs:**
- README update after each completed phase
- Creation or update of a sample after each stable resource client
- XML doc verification on the public surface before release
- NuGet metadata preparation (PackageId, version, icon, tags)

**J5 completion checklist:**
- [ ] README reflects the covered APIs
- [ ] At least one compilable sample per major use case
- [ ] `GenerateDocumentationFile` enabled in the csproj
- [ ] All `public` members have `<summary>`, `<param>`, `<returns>`
- [ ] `PackageId`, `Description`, `Tags`, `RepositoryUrl`, `License` configured in the csproj
- [ ] SourceLink configured (`Microsoft.SourceLink.GitHub`)

---

## J6 — Foundation Jobs

**When:** Phase 1 of the project, or during an infrastructure restructuring.

No dedicated skill — follow the Phase 1 tasks directly in `docs/TODO.md`.

**Phase 1 checklist:**

### 1.1 — Project Configuration
- [ ] `TorBoxSDK.csproj`: multi-targeting `net6.0;net7.0;net8.0;net9.0;net10.0`
- [ ] `Directory.Build.props`: nullable, implicit usings, version, authors, license, `TreatWarningsAsErrors`
- [ ] `.editorconfig`: C# conventions (based on `csharp-conventions.instructions.md`)
- [ ] NuGet properties in the csproj: `PackageId`, `Description`, `Tags`, `RepositoryUrl`, `LicenseExpression`, `Icon`
- [ ] SourceLink: `Microsoft.SourceLink.GitHub`

### 1.2 — Base Architecture
- [ ] `TorBoxClient` + `ITorBoxClient` + `TorBoxClientOptions`
- [ ] `MainApiClient`, `SearchApiClient`, `RelayApiClient` (facades)
- [ ] Empty resource clients exposed via `Main.*` (Torrents, Usenet, WebDownloads, User, Notifications, Rss, Stream, Integrations, Vendors, Queued)
- [ ] `AuthHandler` (DelegatingHandler Bearer token)
- [ ] Internal `HttpClient` with no external dependency

### 1.3 — Standard Response and Errors
- [ ] `TorBoxResponse<T>` and `TorBoxResponse` (non-generic)
- [ ] `TorBoxException` with `ErrorCode` and `Detail`
- [ ] Complete `TorBoxErrorCode` enum (all documented API codes)
- [ ] System.Text.Json deserialization with `DateTimeOffset` UTC

### 1.4 — Dependency Injection
- [ ] `AddTorBox(Action<TorBoxClientOptions>)` on `IServiceCollection`
- [ ] `IHttpClientFactory` for `HttpClient` management via named clients
- [ ] `IConfiguration` section `TorBox` support
- [ ] Only `ITorBoxClient` is registered in the DI container
- [ ] Sub-clients (`MainApiClient`, `SearchApiClient`, `RelayApiClient`, resource clients) are `internal` and instantiated by `TorBoxClient` — not individually registered

**Phase 1 exit criteria:**
- `dotnet build` passes without errors or warnings on all targets
- `services.AddTorBox(o => o.ApiKey = "...")` compiles
- `provider.GetRequiredService<ITorBoxClient>()` resolves the client
- `client.Main.Torrents` is accessible (even if empty)
- Sub-clients are not directly resolvable from the DI container

---

## Recommended Overall Development Order

```
J6 Foundation (Phase 1)
  └─ J1 Architecture (validate the hierarchy)
       └─ J2 Endpoint: Phase 2 common models
            └─ J2 Endpoint: Phase 2 Torrents models
                 └─ J2 Endpoint: Phase 3.1 TorrentsClient
                      └─ J3 TorrentsClient unit tests
                           └─ J4 Review TorrentsClient + tests
                                └─ (repeat J2→J3→J4 for each resource client)
                                     └─ J2+J3+J4 Search API
                                          └─ J2+J3+J4 Relay API
                                               └─ J5 Docs + samples
                                                    └─ J5 NuGet + release
```
