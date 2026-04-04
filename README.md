# TorBoxSDK

[![NuGet](https://img.shields.io/badge/NuGet-TorBoxSDK-blue?logo=nuget)](https://www.nuget.org/packages/TorBoxSDK)
![Build](https://img.shields.io/badge/build-placeholder-lightgrey)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-purple)](#supported-net-versions)

TorBoxSDK is an open-source, MIT-licensed C# SDK for the [TorBox API](https://api-docs.torbox.app/). It provides typed clients for the TorBox Main, Search, and Relay APIs, integrates cleanly with dependency injection and `IHttpClientFactory`, and gives .NET applications a consistent way to work with TorBox responses, authentication, and error handling across 107 endpoints.

## Features

- Covers the full TorBox platform surface across the Main, Search, and Relay APIs
- Multi-targets `.NET 6` through `.NET 10`
- Root `TorBoxClient` with `Main`, `Search`, and `Relay` API families
- Resource-oriented Main API clients for torrents, usenet, web downloads, user operations, integrations, and more
- Dependency injection support via `AddTorBox()` with both `Action<TorBoxClientOptions>` and `IConfiguration`
- Bearer token authentication through an internal `DelegatingHandler`
- Standard TorBox response envelope via `TorBoxResponse` and `TorBoxResponse<T>`
- Typed exception handling with `TorBoxException` and `TorBoxErrorCode`
- `System.Text.Json` serialization with `snake_case` naming
- `CancellationToken` support on all async methods
- Library awaits use `ConfigureAwait(false)`
- SourceLink enabled for package debugging

## Supported .NET Versions

| Target Framework | Supported |
|---|---|
| `net6.0` | Yes |
| `net7.0` | Yes |
| `net8.0` | Yes |
| `net9.0` | Yes |
| `net10.0` | Yes |

## Installation

### .NET CLI

```bash
dotnet add package TorBoxSDK
```

### Package Manager Console

```powershell
Install-Package TorBoxSDK
```

## Quick Start

Register the SDK with dependency injection and provide your TorBox API key:

```csharp
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;

ServiceCollection services = new();

services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable.");
});

using ServiceProvider provider = services.BuildServiceProvider();
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
```

Then call the API through the typed clients:

```csharp
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

TorBoxResponse<IReadOnlyList<Torrent>> torrents = await client.Main.Torrents.GetMyTorrentListAsync();

if (torrents.Data is not null)
{
    foreach (Torrent torrent in torrents.Data)
    {
        Console.WriteLine($"{torrent.Name} — {torrent.Progress:P0}");
    }
}

TorBoxResponse<IReadOnlyList<TorrentSearchResult>> searchResults =
    await client.Search.SearchTorrentsAsync("ubuntu");
```

## Architecture

The SDK is organized around a single root client that exposes three API families. The Main API is further split into focused resource clients.

```text
TorBoxClient (ITorBoxClient)
├── Main (IMainApiClient)
│   ├── General (IGeneralClient) — 4 endpoints
│   ├── Torrents (ITorrentsClient) — 14 endpoints
│   ├── Usenet (IUsenetClient) — 8 endpoints
│   ├── WebDownloads (IWebDownloadsClient) — 9 endpoints
│   ├── User (IUserClient) — 16 endpoints
│   ├── Notifications (INotificationsClient) — 8 endpoints
│   ├── Rss (IRssClient) — 5 endpoints
│   ├── Stream (IStreamClient) — 2 endpoints
│   ├── Integrations (IIntegrationsClient) — 17 endpoints
│   ├── Vendors (IVendorsClient) — 8 endpoints
│   └── Queued (IQueuedClient) — 2 endpoints
├── Search (ISearchApiClient) — 12 endpoints
└── Relay (IRelayApiClient) — 2 endpoints
```

## API Coverage

| Area | Interface | API Family | Endpoints |
|---|---|---|---:|
| General | `IGeneralClient` | Main | 4 |
| Torrents | `ITorrentsClient` | Main | 14 |
| Usenet | `IUsenetClient` | Main | 8 |
| Web Downloads | `IWebDownloadsClient` | Main | 9 |
| User | `IUserClient` | Main | 16 |
| Notifications | `INotificationsClient` | Main | 8 |
| RSS | `IRssClient` | Main | 5 |
| Stream | `IStreamClient` | Main | 2 |
| Integrations | `IIntegrationsClient` | Main | 17 |
| Vendors | `IVendorsClient` | Main | 8 |
| Queued | `IQueuedClient` | Main | 2 |
| Search API | `ISearchApiClient` | Search | 12 |
| Relay API | `IRelayApiClient` | Relay | 2 |
| **Total** |  |  | **107** |

## Configuration

`AddTorBox()` binds to `TorBoxClientOptions`.

| Property | Required | Default | Description |
|---|---|---|---|
| `ApiKey` | Yes | — | TorBox API key used for Bearer authentication |
| `MainApiBaseUrl` | No | `https://api.torbox.app/v1/api/` | Base URL for the Main API |
| `SearchApiBaseUrl` | No | `https://search-api.torbox.app/` | Base URL for the Search API |
| `RelayApiBaseUrl` | No | `https://relay.torbox.app/` | Base URL for the Relay API |
| `Timeout` | No | `00:00:30` | HTTP timeout applied to SDK clients |

## Error Handling

Successful requests return the standard TorBox response envelope. API failures are surfaced as `TorBoxException`, which includes a typed `TorBoxErrorCode` and the API-provided detail message.

```csharp
using TorBoxSDK.Models.Common;

try
{
    await client.Main.Torrents.GetMyTorrentListAsync(ct: cts.Token);
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"[{ex.ErrorCode}] {ex.Detail ?? ex.Message}");
}
```

TorBoxSDK includes 22 TorBox-specific error codes, plus `Unknown` for unmapped values.

## Advanced Usage

### Bind from `IConfiguration`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;

services.AddTorBox(configuration);
```

`appsettings.json`:

```json
{
  "TorBox": {
    "ApiKey": "your-api-key",
    "MainApiBaseUrl": "https://api.torbox.app/v1/api/",
    "SearchApiBaseUrl": "https://search-api.torbox.app/",
    "RelayApiBaseUrl": "https://relay.torbox.app/",
    "Timeout": "00:00:30"
  }
}
```

### Direct instantiation for non-DI scenarios

For applications that do not use dependency injection, you can instantiate API-family clients directly with an authenticated `HttpClient`:

```csharp
using System.Net.Http.Headers;
using TorBoxSDK.Search;

string apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
    ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable.");

HttpClient searchHttpClient = new()
{
    BaseAddress = new Uri("https://search-api.torbox.app/"),
    Timeout = TimeSpan.FromSeconds(30),
};

searchHttpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", apiKey);

ISearchApiClient searchClient = new SearchApiClient(searchHttpClient);
```

The same pattern can be used for `RelayApiClient`, individual Main API resource clients, or full manual composition of `TorBoxClient`.

## Documentation

- [Getting Started](docs/getting-started.md)
- [Architecture Overview](docs/architecture.md)
- [API Reference](docs/api-reference.md)
- [Configuration Reference](docs/configuration.md)
- [Error Handling](docs/error-handling.md)
- [Roadmap](docs/TODO.md)

## Contributing

Contributions are welcome. Please open an issue or pull request for bug fixes, API coverage improvements, documentation updates, or test additions. Keep documentation and examples aligned with the public SDK surface.

## License

TorBoxSDK is released under the MIT License.
