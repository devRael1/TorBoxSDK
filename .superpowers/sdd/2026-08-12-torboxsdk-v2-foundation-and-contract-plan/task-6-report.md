# Task 6 report — V2 options and root client hierarchy

## Status

Implemented on `codex/v2-redesign-foundation` from the requested starting
commit `88d9e3e3564fb8033f27aae5c212e5ee1cae7555`. This report is the Task 6
implementation handoff; it is not a self-review. An independent review remains
for the controller/reviewer.

The scope is limited to the approved Task 6 files, plus the explicit Task 4
handoff that moves the unit-test `InternalsVisibleTo` attribute into
`Properties/AssemblyInfo.cs`. No V1 source, project file, lock file, plan,
contract, DI-package implementation, endpoint method, or resource mapping was
changed.

## Files

Created production files:

- `src/TorBoxSDK.V2/ITorBoxClient.cs`
- `src/TorBoxSDK.V2/TorBoxClient.cs`
- `src/TorBoxSDK.V2/TorBoxClientOptions.cs`
- `src/TorBoxSDK.V2/Internal/TorBoxClientFactory.cs`
- `src/TorBoxSDK.V2/Properties/AssemblyInfo.cs`
- `src/TorBoxSDK.V2/Main/IMainApiClient.cs`
- `src/TorBoxSDK.V2/Main/MainApiClient.cs`
- `src/TorBoxSDK.V2/Search/ISearchApiClient.cs`
- `src/TorBoxSDK.V2/Search/SearchApiClient.cs`
- `src/TorBoxSDK.V2/Relay/IRelayApiClient.cs`
- `src/TorBoxSDK.V2/Relay/RelayApiClient.cs`
- `src/TorBoxSDK.V2/Main/General/IGeneralClient.cs`
- `src/TorBoxSDK.V2/Main/General/GeneralClient.cs`
- `src/TorBoxSDK.V2/Main/Torrents/ITorrentsClient.cs`
- `src/TorBoxSDK.V2/Main/Torrents/TorrentsClient.cs`
- `src/TorBoxSDK.V2/Main/Usenet/IUsenetClient.cs`
- `src/TorBoxSDK.V2/Main/Usenet/UsenetClient.cs`
- `src/TorBoxSDK.V2/Main/WebDownloads/IWebDownloadsClient.cs`
- `src/TorBoxSDK.V2/Main/WebDownloads/WebDownloadsClient.cs`
- `src/TorBoxSDK.V2/Main/User/IUserClient.cs`
- `src/TorBoxSDK.V2/Main/User/UserClient.cs`
- `src/TorBoxSDK.V2/Main/Notifications/INotificationsClient.cs`
- `src/TorBoxSDK.V2/Main/Notifications/NotificationsClient.cs`
- `src/TorBoxSDK.V2/Main/Rss/IRssClient.cs`
- `src/TorBoxSDK.V2/Main/Rss/RssClient.cs`
- `src/TorBoxSDK.V2/Main/Stream/IStreamClient.cs`
- `src/TorBoxSDK.V2/Main/Stream/StreamClient.cs`
- `src/TorBoxSDK.V2/Main/Integrations/IIntegrationsClient.cs`
- `src/TorBoxSDK.V2/Main/Integrations/IntegrationsClient.cs`
- `src/TorBoxSDK.V2/Main/Vendors/IVendorsClient.cs`
- `src/TorBoxSDK.V2/Main/Vendors/VendorsClient.cs`
- `src/TorBoxSDK.V2/Main/Queued/IQueuedClient.cs`
- `src/TorBoxSDK.V2/Main/Queued/QueuedClient.cs`

Created tests:

- `tests/TorBoxSDK.V2.UnitTests/TorBoxClientTests.cs`
- `tests/TorBoxSDK.V2.UnitTests/Configuration/TorBoxClientOptionsTests.cs`

Modified only for the explicit Task 4 handoff:

- `src/TorBoxSDK.V2/Models/Common/TorBoxStreamResponse.cs` — removed its
  temporary assembly-level `InternalsVisibleTo` declaration.

Created delivery record:

- `.superpowers/sdd/2026-08-12-torboxsdk-v2-foundation-and-contract-plan/task-6-report.md`

## URL evidence and defaults

The defaults were selected from local, versioned evidence only. No V1 source
or V1 URL-composition code was copied.

| Family | Resolved default | Local evidence | Resolution |
| --- | --- | --- | --- |
| Main | `https://api.torbox.app/v1/api/` | `contracts/torbox/baseline/manifest.json:2` records the official snapshot URL `https://api.torbox.app/openapi.json`; the minified snapshot at `contracts/torbox/baseline/openapi.json:1` records server `https://api.torbox.app` and routes such as `/v1/api/user/me`. | Official host + the baseline’s common `/v1/api/` route prefix. |
| Search | `https://search-api.torbox.app/` | `contracts/torbox/search/manifest.json:2-7` records the frozen Postman documentation source and its `requires-live-validation` status; `contracts/torbox/search/operations.json:3-31` records its host-relative routes; existing frozen documentation explicitly names the Search base URL at `docs/guides/configuration.md:19,48` and `docs/guides/configuration/urls-and-versioning.md:15,30`. | Direct Search host, with no fabricated version segment. This does not assert that the host is currently reachable. |
| Relay | `https://relay.torbox.app/v1/` | `contracts/torbox/relay/manifest.json:2` records the official Relay snapshot URL `https://relay.torbox.app/openapi.json`; the minified snapshot at `contracts/torbox/relay/openapi.json:1` records `/v1/inactivecheck/torrent/{auth_id}/{torrent_id}`. | Official host + the snapshot’s `/v1/` route prefix. |

`TorBoxClientOptions` exposes mutable configuration-bindable `ApiKey`,
`MainApiBaseUrl`, `SearchApiBaseUrl`, `RelayApiBaseUrl`, and `Timeout` values.
Construction validates them once into the internal immutable
`ValidatedTorBoxClientOptions` snapshot. It rejects a null, empty, or
whitespace key; missing, non-absolute, non-HTTP(S), query/fragment-bearing, or
trailing-slash-less family URLs; and timeout values less than or equal to zero.
The `netstandard2.0` nullable compiler does not infer the same non-null state
after `IsNullOrWhiteSpace` as the newer targets, so the final implementation
uses a minimal `?? throw` guard before the shared validation; the targeted
netstandard build then passed with zero warnings and errors.

## Public navigation surface

```text
ITorBoxClient : IDisposable
|- Main : IMainApiClient
|  |- General       : IGeneralClient
|  |- Torrents      : ITorrentsClient
|  |- Usenet        : IUsenetClient
|  |- WebDownloads  : IWebDownloadsClient
|  |- User          : IUserClient
|  |- Notifications : INotificationsClient
|  |- Rss           : IRssClient
|  |- Stream        : IStreamClient
|  |- Integrations  : IIntegrationsClient
|  |- Vendors       : IVendorsClient
|  `- Queued        : IQueuedClient
|- Search : ISearchApiClient
`- Relay  : IRelayApiClient
```

All resource interfaces, `ISearchApiClient`, and `IRelayApiClient` deliberately
contain no endpoint methods. The implementation classes are internal. No public
client/family/resource surface exposes `HttpClient` or `HttpResponseMessage`,
and no resource concatenates a version or token into an endpoint path.

## Construction and ownership

Direct `new TorBoxClient(options)`:

- validates and snapshots options before creating any pipeline;
- creates three distinct `HttpClient` instances, each with its own resolved
  family `BaseAddress` and validated `Timeout`;
- places Task 5 `AuthHandler` above a Task 5
  `TorBoxHttpClientHandlerFactory` handler, which retains
  `AllowAutoRedirect = false`;
- supplies one internal `TorBoxApiTransport` to the internal family/resource
  facades; and
- owns and disposes all three clients only in this direct mode.

`TorBoxSDK.Internal.TorBoxClientFactory.Create` accepts the three externally
managed `HttpClient` instances, `ITorBoxApiTransport`, and an explicit
`ownsHttpClients` flag. It returns only `ITorBoxClient`. Core has no
Microsoft.Extensions reference. `AssemblyInfo.cs` grants internals only to
`TorBoxSDK.DependencyInjection` and `TorBoxSDK.V2.UnitTests`. A final search
found exactly those two declarations in `Properties/AssemblyInfo.cs`; there is
no duplicate attribute left in `TorBoxStreamResponse.cs`.

Two synthetic-handler tests prove the ownership contract without a network
request:

- `ownsHttpClients: false`: after root disposal, each external Main, Search,
  and Relay client can still send a relative `probe` request to its own handler;
  each returns HTTP 200, has one send, and its handler remains undisposed.
- `ownsHttpClients: true`: root disposal deterministically disposes all three
  supplied primary handlers. This handler-state assertion is stable on net6.0
  through net10.0 and avoids relying on target-specific wording/timing of an
  `HttpClient` use-after-dispose exception.

## TDD evidence

### RED

Before any Task 6 production source existed, these two test files were added
and the required filtered command was executed:

```powershell
dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj \
  --configuration Release --no-restore \
  --filter "FullyQualifiedName~TorBoxClient|FullyQualifiedName~TorBoxClientOptions"
```

It failed as expected on net6.0 through net10.0 with `CS0234` for absent
`TorBoxSDK.Main`, `TorBoxSDK.Search`, and `TorBoxSDK.Relay` namespaces and
`CS0246` for absent `TorBoxClientOptions`. This was the intended missing-Task-6
failure, not a network request or a test typo.

### GREEN

After the minimal hierarchy/options implementation, the same filter passed
with 21 tests per executable target. The later two ownership tests were added
at the controller’s request. Their first compile attempt found only a missing
`System.Net` import in the new test file (`CS0103` for `HttpStatusCode`); this
was corrected as a test-source defect before behavioral interpretation. The
re-run passed 2/2 on every executable target, exposing no production defect.

The final required filtered run, after all Task 6 source/test changes, passed
23 tests with zero failures and zero skips on each of net6.0, net7.0, net8.0,
net9.0, and net10.0.

## Validation performed

All commands were run from
`D:\Bot discord\TorBoxSDK\.worktrees\v2-redesign-foundation`. No validation
sent a request to TorBox or used credentials.

- `dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore`
  - Passed; core built for `netstandard2.0` plus net6.0 through net10.0; zero
    warnings and zero errors.
- Required hierarchy/options filter (above)
  - Passed; 23/23 tests on each executable target.
- `dotnet test tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-build --no-restore`
  - Passed; 78/78 tests on each executable target.
- `dotnet test tests/TorBoxSDK.V2.ContractTests/TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-build --no-restore`
  - Passed; 39 passed, 0 failed, 1 skipped (`V2ReleaseContractTests.AllOperationsAreImplemented`, disabled by design) on each executable target.
- `dotnet format TorBoxSDK.V2.slnx whitespace --no-restore --include <Task 6 files>`
  - Passed (exit 0).
- `dotnet format TorBoxSDK.V2.slnx style --verify-no-changes --no-restore --include <Task 6 files>`
  - Passed (exit 0).
- `dotnet format TorBoxSDK.V2.slnx analyzers --verify-no-changes --no-restore --include <Task 6 files>`
  - Passed (exit 0).
  - The formatter commands emitted the existing generic workspace-load warning,
    but no formatter/analyzer failure.
- `git diff --check`
  - Passed with no whitespace errors before staging.

## Task 7 handoff

Task 7 can now implement the separate optional DI package without changing the
core package dependencies:

1. Bind/configure `TorBoxClientOptions` in the DI project, validate options
   before resolving/sending, and register three named family clients with their
   own resolved base URI, timeout, and authentication pipeline.
2. Create one internal `TorBoxApiTransport` and call
   `TorBoxClientFactory.Create(main, search, relay, transport,
   ownsHttpClients: false)`.
3. Register only `ITorBoxClient`, not family/resource interfaces.
4. Preserve the factory ownership rule: DI-managed clients stay usable after a
   root client is disposed; their lifetime remains with `IHttpClientFactory`.

No endpoint method belongs to Task 7 or this Task 6 slice. Main/Search/Relay
resource plans must add contract-backed endpoint methods later.

## Fix round 1/5 — timeout range validation

This isolated fix addresses the confirmed major validation gap found after the
Task 6 commit. `ValidateAndNormalize()` previously rejected only timeout values
less than or equal to zero. It therefore accepted values above the maximum that
`HttpClient.Timeout` accepts, allowing direct construction to reach the BCL
setter instead of reporting a complete options error before pipeline creation.

The selected upper bound is exactly
`TimeSpan.FromMilliseconds(int.MaxValue)` (`24.20:31:23.647`). It is the
maximum supported by `HttpClient.Timeout`, is expressed solely with portable
`TimeSpan` and `int` APIs available to `netstandard2.0`, and preserves the BCL
boundary rather than choosing an arbitrary lower SDK limit. The validation rule
is now explicitly `Timeout > TimeSpan.Zero && Timeout <=
TimeSpan.FromMilliseconds(int.MaxValue)` and throws `ArgumentException` with
the `Timeout` parameter name outside that range.

### RED/GREEN evidence

Two regression tests were first added in
`Configuration/TorBoxClientOptionsTests.cs` and execute `ValidateAndNormalize()`
directly, so they create no `HttpClient`, handler, or network request:

- `ValidateAndNormalize_WithTimeoutAboveHttpClientMaximum_ThrowsArgumentException`
  sets `TimeSpan.FromDays(25)`. Before production code changed, it failed as
  intended on net6.0 through net10.0: `Assert.Throws() Failure: No exception
  was thrown` (1 failed, 1 passed, 2 total per target).
- `ValidateAndNormalize_WithMaximumHttpClientTimeout_AcceptsTimeout` sets the
  exact BCL maximum and verifies that the normalized immutable snapshot retains
  it. It was already green during RED and remains green after the fix.

After adding the explicit upper-bound guard, the focused two-test filter passed
2/2 with zero failures on each of net6.0, net7.0, net8.0, net9.0, and net10.0.
The full post-fix validation passed with the following counts on each executable
target: hierarchy/options filter 25/25, UnitTests 80/80, and ContractTests 39
passed / 0 failed / 1 skipped (the existing disabled
`V2ReleaseContractTests.AllOperationsAreImplemented`). The Release V2 solution
build also passed with zero warnings and zero errors, including the
`netstandard2.0` core target. Targeted whitespace/style/analyzer formatting
passed; the formatter repeated its existing generic workspace-load warning but
reported no formatting or analyzer failure.
