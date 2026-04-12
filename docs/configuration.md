# Configuration Reference

TorBoxSDK is configured through `TorBoxClientOptions`.

## Options

| Property | Required | Default | Notes |
|---|---|---|---|
| `ApiKey` | Yes | — | Required for authenticated TorBox requests |
| `MainApiBaseUrl` | No | `https://api.torbox.app/v1/api/` | Trailing slash should be preserved |
| `SearchApiBaseUrl` | No | `https://search-api.torbox.app/` | Trailing slash should be preserved |
| `RelayApiBaseUrl` | No | `https://relay.torbox.app/` | Trailing slash should be preserved |
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

## Registration overloads

- `AddTorBox(Action<TorBoxClientOptions>)`
- `AddTorBox(IConfiguration)`

Both overloads register:

- authenticated named `HttpClient` pipelines (Main, Search, Relay)
- `ITorBoxClient` as the single SDK entry point

All sub-clients (`MainApiClient`, `SearchApiClient`, `RelayApiClient`, and their resource clients) are instantiated internally by `TorBoxClient`. They are **not** registered individually in the DI container. To access any sub-client, resolve `ITorBoxClient` and navigate through its properties:

```csharp
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
client.Main.Torrents   // ITorrentsClient
client.Search          // ISearchApiClient
client.Relay           // IRelayApiClient
```
