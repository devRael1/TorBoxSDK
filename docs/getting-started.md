# Getting Started

This guide is for developers integrating TorBoxSDK into a .NET application for the first time.

## Install

```bash
dotnet add package TorBoxSDK
```

## Register with dependency injection

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

## Make your first requests

```csharp
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

TorBoxResponse<IReadOnlyList<Torrent>> torrents = await client.Main.Torrents.GetMyTorrentListAsync();
TorBoxResponse<TorrentSearchResponse> results = await client.Search.SearchTorrentsAsync("ubuntu");
```

## What to expect from responses

- Successful calls return `TorBoxResponse<T>`
- API errors throw `TorBoxException`
- All SDK methods are asynchronous
- All public async methods accept `CancellationToken`

## Next steps

- [API Reference](api-reference.md)
- [Configuration Reference](configuration.md)
- [Error Handling](error-handling.md)
- [Architecture Overview](architecture.md)
