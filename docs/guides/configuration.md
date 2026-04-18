---
uid: configuration
title: Configuration Overview
description: Configure TorBoxSDK options including API key, base URLs, and timeouts.
---

# Configuration Overview

TorBoxSDK is configured through <xref:TorBoxSDK.TorBoxClientOptions>.

## Options

| Property | Required | Default | Notes |
|---|---|---|---|
| `ApiKey` | Yes | — | Required for authenticated TorBox requests |
| `MainApiBaseUrl` | No | `https://api.torbox.app/` | Host URL for the Main API. Trailing slash should be preserved. |
| `ApiVersion` | Yes | `v1` | Version segment used to compute versioned Main and Relay API URLs |
| `MainApiVersionedUrl` | — | Computed | Full Main API URL with version (e.g. `https://api.torbox.app/v1/api/`). Read-only. |
| `SearchApiBaseUrl` | No | `https://search-api.torbox.app/` | Trailing slash should be preserved |
| `RelayApiBaseUrl` | No | `https://relay.torbox.app/` | Host URL for the Relay API. Trailing slash should be preserved. |
| `RelayApiVersionedUrl` | — | Computed | Full Relay API URL with version (e.g. `https://relay.torbox.app/v1/`). Read-only. |
| `Timeout` | No | `00:00:30` | Applied to all configured `HttpClient` instances |

## Configure with code

```csharp
services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Missing TorBox API key.");
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

## Configure with `IConfiguration`

```csharp
services.AddTorBox(configuration);
```

The SDK binds from the `TorBox` section:

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

## Configure without DI

When using standalone mode, pass options directly to the constructor:

```csharp
// API key only (default settings)
using TorBoxClient client = new("your-api-key");

// Full options object
using TorBoxClient client = new(new TorBoxClientOptions
{
    ApiKey = "your-api-key",
    MainApiBaseUrl = "https://api.torbox.app/",
    ApiVersion = "v1",
    SearchApiBaseUrl = "https://search-api.torbox.app/",
    RelayApiBaseUrl = "https://relay.torbox.app/",
    Timeout = TimeSpan.FromSeconds(60)
});

// Configuration delegate
using TorBoxClient client = new(options =>
{
    options.ApiKey = "your-api-key";
    options.Timeout = TimeSpan.FromMinutes(2);
});
```

## Registration overloads

- `AddTorBox(Action<TorBoxClientOptions>)`
- `AddTorBox(IConfiguration)`

Both overloads register:

- authenticated named `HttpClient` pipelines (Main, Search, Relay)
- <xref:TorBoxSDK.ITorBoxClient> as the single SDK entry point

All sub-clients (`MainApiClient`, `SearchApiClient`, `RelayApiClient`, and their resource clients) are instantiated internally by <xref:TorBoxSDK.TorBoxClient>. They are **not** registered individually in the DI container. To access any sub-client, resolve <xref:TorBoxSDK.ITorBoxClient> and navigate through its properties:

```csharp
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
client.Main.Torrents   // ITorrentsClient
client.Search          // ISearchApiClient
client.Relay           // IRelayApiClient
```


