# Task 7 report — V2 optional dependency-injection package

## Handoff scope

This record describes the Task 7 implementation handoff on
`codex/v2-redesign-foundation`. It does not provide an independent review
verdict of the implementation.

The change is limited to the optional V2 DI surface, its deterministic unit
tests, the unit-test project reference and its resulting lock graph:

- `src/TorBoxSDK.DependencyInjection.V2/TorBoxServiceCollectionExtensions.cs`
- `tests/TorBoxSDK.V2.UnitTests/DependencyInjection/TorBoxServiceCollectionExtensionsTests.cs`
- `tests/TorBoxSDK.V2.UnitTests/TorBoxSDK.V2.UnitTests.csproj`
- `tests/TorBoxSDK.V2.UnitTests/packages.lock.json`
- this handoff record

No V1 source, V2 core source, contract/plan/specification, package version,
or non-unit-test project file was changed.

## Delivered API and registrations

The separate assembly remains `TorBoxSDK.DependencyInjection` and exposes the
two public extensions:

```csharp
IServiceCollection AddTorBox(
    this IServiceCollection services,
    Action<TorBoxClientOptions> configure)

IServiceCollection AddTorBox(
    this IServiceCollection services,
    IConfiguration configuration)
```

The configuration overload binds exactly `configuration.GetSection("TorBox")`.
Values at the root, including an `ApiKey` outside this section, are not used.

Only `ITorBoxClient` is registered as an SDK client service. Its registration
is transient, and each resolution creates a distinct root client. The Main,
Search, Relay, and all eleven Main resource interfaces are deliberately not
registered. The registration factory obtains the three factory-managed named
clients and constructs the root through:

```csharp
TorBoxClientFactory.Create(
    mainHttpClient,
    searchHttpClient,
    relayHttpClient,
    new TorBoxApiTransport(),
    ownsHttpClients: false)
```

Consequently, the root client has no ownership of the factory-managed
`HttpClient` instances and does not expose them through its public surface.

### Repeated `AddTorBox` calls

Repeated calls compose `AddOptions<TorBoxClientOptions>().Configure(...)`
delegates in registration order. A later delegate can therefore override a
value configured by an earlier one. The named HTTP registrations and the root
service are added once: a private marker prevents the second core registration,
and `TryAddTransient<ITorBoxClient>` preserves a single root descriptor. The
unit test configures a first Main URL then a second one, proves the second URL
is applied, and asserts exactly one `ITorBoxClient` descriptor.

## HTTP-client composition and validation

The DI package creates three separate named clients:

| Name | Resolved default base address |
| --- | --- |
| `TorBoxSDK.Main` | `https://api.torbox.app/v1/api/` |
| `TorBoxSDK.Search` | `https://search-api.torbox.app/` |
| `TorBoxSDK.Relay` | `https://relay.torbox.app/v1/` |

Each named pipeline obtains a new private `AuthHandler` from the validated
options and a primary handler from `TorBoxHttpClientHandlerFactory.Create`.
The latter retains Task 5's `AllowAutoRedirect = false` behavior. The auth
handler receives a validated key, replaces any authorization with one Bearer
value, and rejects a blank key before a send.

`CreateRootClient` validates `TorBoxClientOptions` before it calls
`IHttpClientFactory.CreateClient` for any family. Invalid delegate or
configuration values thus fail during root resolution without creating the
configured primary handlers or sending a request. The registered named-client
configuration also gets the same validated snapshot before setting base address
and timeout.

The deterministic tests replace all exercised primary handlers with synthetic
handlers. They prove individual Main/Search/Relay base addresses, their three
resolved `probe` request URIs, three distinct handler instances, and exactly
one `Bearer test-key` header at each handler. An invalid base-URL test uses a
synthetic API-key sentinel and proves that it is absent from the exception
message. No test uses a TorBox key or sends a request to TorBox.

## TDD evidence

### RED

The first registration test and the authorized unit-test project reference
were written before the DI extension source existed. The command below failed
on `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0` with `CS0234`: the
`TorBoxSDK.DependencyInjection` namespace and `AddTorBox` did not exist.

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj \
  --configuration Release --no-restore \
  --filter "FullyQualifiedName~TorBoxServiceCollectionExtensionsTests"
```

After the initial minimal implementation, the repeated-registration regression
test was added before the idempotency implementation. It failed on all five
TFMs with `Assert.Single()` because two transient `ITorBoxClient` descriptors
were present. The private registration marker plus `TryAddTransient` made the
same test green.

The `IConfiguration` test was also added before the overload. After its test
fixture was made compatible with Configuration.Abstractions on net6/net7, it
failed on all five TFMs with `CS1503`, because an `IConfiguration` could only
be supplied to the delegate overload. Adding the exact `TorBox`-section
overload made it green.

### GREEN

The final focused DI command passed **12 tests, 0 failures, 0 skips** on each
of `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`. It covers:

- public delegate/configuration registration, null input, and exact section
  binding;
- transient root instances and absence of all family/resource registrations;
- default and configured named-client base addresses and resolved request URIs;
- per-family synthetic primary handlers and one Bearer value;
- invalid blank key, missing `TorBox:ApiKey`, and invalid base URL before
  handler creation or request send, including no key leak in the exception;
- non-destructive root disposal for Main, Search, and Relay factory pipelines;
- repeated registration ordering and the single root descriptor.

## Dependency and lockfile evidence

The DI project already declares only the needed Microsoft.Extensions APIs:
Configuration.Abstractions, DependencyInjection.Abstractions, Http, Options,
and Options.ConfigurationExtensions. It carries the configuration binder
transitively through Options.ConfigurationExtensions. No package version was
added or changed.

The V2 unit-test project received one project reference to
`TorBoxSDK.DependencyInjection.V2`. The initial locked restore failed only
with `NU1004` for the new `TorBoxSDK.DependencyInjection` project reference on
all five executable TFMs. A normal restore was then run only for the V2 unit
test project. Its lockfile added that project and its already-declared
Microsoft.Extensions transitive graph: 8.0.x assets for net6/net7 and 10.0.6
assets for net8/net10. No other lockfile changed. The final command below
passed:

```powershell
dotnet restore tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --locked-mode
```

The Release solution build also compiled the DI project for `netstandard2.0`,
confirming the selected API surface remains compatible with that asset in
addition to net6 through net10.

## Final deterministic validation

All commands below ran from the V2 worktree. They use no TorBox request and no
real credential.

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj \
  --configuration Release --no-restore
```

Result: **92 passed, 0 failed, 0 skipped** per executable TFM (`net6.0` through
`net10.0`).

```powershell
dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore
```

Result: success with **0 warnings and 0 errors**, including
`TorBoxSDK.DependencyInjection` for `netstandard2.0` and net6 through net10.

```powershell
dotnet test tests\TorBoxSDK.V2.ContractTests\TorBoxSDK.V2.ContractTests.csproj \
  --configuration Release --no-build --no-restore
```

Result: **39 passed, 0 failed, 1 skipped** per executable TFM. The skip is the
existing `V2ReleaseContractTests.AllOperationsAreImplemented` release gate.

```powershell
dotnet format TorBoxSDK.V2.slnx whitespace --no-restore --include <Task7 files>
dotnet format TorBoxSDK.V2.slnx style --verify-no-changes --no-restore --include <Task7 files>
dotnet format TorBoxSDK.V2.slnx analyzers --verify-no-changes --no-restore --include <Task7 files>
git diff --check
```

Result: all formatter commands returned zero; they emitted only the existing
generic workspace-load warning. The final Git whitespace check is recorded
with the staged delivery check.
