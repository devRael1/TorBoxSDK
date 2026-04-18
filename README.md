# TorBoxSDK

[![NuGet](https://img.shields.io/badge/NuGet-TorBoxSDK-blue?logo=nuget)](https://www.nuget.org/packages/TorBoxSDK)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/devRael1/TorBoxSDK/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-purple)](#supported-net-versions)

**Important:** This SDK is unofficial and is not affiliated with or endorsed by TorBox.

TorBoxSDK is an open-source, MIT-licensed C# SDK for the [TorBox API](https://api-docs.torbox.app/). It gives .NET applications typed access to the TorBox Main, Search, and Relay APIs with dependency injection support, consistent response handling, and a public surface designed to be easy to explore in IntelliSense.

## Why TorBoxSDK?

- Covers the TorBox Main, Search, and Relay APIs through a single root client
- Organizes the Main API into focused resource clients such as Torrents, Usenet, Web Downloads, User, Notifications, RSS, and Integrations
- Multi-targets `.NET 6` through `.NET 10`
- Integrates with `IServiceCollection`, `IHttpClientFactory`, and configuration binding
- Uses the standard `TorBoxResponse` envelope and surfaces API failures through `TorBoxException`
- Ships XML documentation, SourceLink metadata, symbols, and package README support for a better open-source consumer experience

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

Register the SDK with dependency injection, resolve `ITorBoxClient`, and make your first request:

```csharp
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

ServiceCollection services = new();

services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable.");
});

using ServiceProvider provider = services.BuildServiceProvider();
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

try
{
    TorBoxResponse<IReadOnlyList<Torrent>> torrents =
        await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts.Token);

    TorBoxResponse<TorrentSearchResponse> searchResults =
        await client.Search.SearchTorrentsAsync("ubuntu", cancellationToken: cts.Token);

    Console.WriteLine($"Torrents returned: {torrents.Data?.Count ?? 0}");
    Console.WriteLine($"Search results returned: {searchResults.Data?.Torrents?.Count ?? 0}");
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"TorBox API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
}
```

## Client Hierarchy

The SDK is structured around a single root client with three API families:

```text
TorBoxClient (ITorBoxClient)
├── Main (IMainApiClient)
│   ├── General
│   ├── Torrents
│   ├── Usenet
│   ├── WebDownloads
│   ├── User
│   ├── Notifications
│   ├── Rss
│   ├── Stream
│   ├── Integrations
│   ├── Vendors
│   └── Queued
├── Search (ISearchApiClient)
└── Relay (IRelayApiClient)
```

That public shape currently covers 107 endpoints across 13 clients.

## Configuration

`AddTorBox()` binds `TorBoxClientOptions`.

| Property | Required | Default | Description |
|---|---|---|---|
| `ApiKey` | Yes | — | TorBox API key used for Bearer authentication |
| `MainApiBaseUrl` | No | `https://api.torbox.app/v1/api/` | Main API base URL. Keep the trailing slash. |
| `SearchApiBaseUrl` | No | `https://search-api.torbox.app/` | Search API base URL. Keep the trailing slash. |
| `RelayApiBaseUrl` | No | `https://relay.torbox.app/` | Relay API base URL. Keep the trailing slash. |
| `Timeout` | No | `00:00:30` | HTTP timeout applied to configured clients |

You can also register the SDK from `IConfiguration`:

```csharp
services.AddTorBox(configuration);
```

## Error Handling

- Successful calls return `TorBoxResponse` or `TorBoxResponse<T>`
- API-level failures throw `TorBoxException`
- Transport issues may still surface as `HttpRequestException` or `TaskCanceledException`
- All public async methods accept `CancellationToken`

## Documentation and Examples

- [Getting Started](docs/getting-started.md)
- [Architecture Overview](docs/architecture.md)
- [API Reference](docs/api-reference.md)
- [Configuration Reference](docs/configuration.md)
- [Error Handling](docs/error-handling.md)
- [Examples project](src/TorBoxSDK.Examples/) with 37 runnable scenarios across Main, Search, Relay, and error-handling workflows

## Contributing

Contributions are welcome from SDK consumers and maintainers alike. TorBoxSDK is an open-source, community-driven project — everyone is free to participate, open issues, and submit pull requests.

- Read the [Contributing Guide](CONTRIBUTING.md) for development workflow and PR expectations
- Review the [Code of Conduct](CODE_OF_CONDUCT.md) before participating
- Report security vulnerabilities privately via the [Security Policy](SECURITY.md)
- File bugs or request features through [GitHub Issues](https://github.com/devRael1/TorBoxSDK/issues) (templates are provided)
- Prefer focused pull requests with matching docs or XML documentation updates when public behavior changes
- For changes that call live TorBox services, document any environment requirements such as `TORBOX_API_KEY`

## Development Quick Checks

Common local commands:

```bash
dotnet build
dotnet test tests/TorboxSDK.UnitTests/
dotnet test tests/TorBoxSDK.IntegrationTests/
```

Integration tests are designed to skip gracefully when `TORBOX_API_KEY` is not set.

## Versioning

This project follows [Semantic Versioning (SemVer)](https://semver.org). Each release version uses the `MAJOR.MINOR.PATCH` format:

- **MAJOR** — incompatible API changes
- **MINOR** — new functionality added in a backward-compatible manner
- **PATCH** — backward-compatible bug fixes

## License

This repository is available under the [MIT License](LICENSE).
