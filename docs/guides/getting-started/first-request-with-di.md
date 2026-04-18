---
uid: getting-started-first-request-di
title: First Request with Dependency Injection
description: Register TorBoxSDK through IServiceCollection and execute your first request with ITorBoxClient.
---

# First Request with Dependency Injection

This is the recommended path for ASP.NET Core and hosted services.

## Register TorBoxSDK

```csharp
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK.DependencyInjection;

ServiceCollection services = new();

services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Set TORBOX_API_KEY.");
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

## Resolve root client

```csharp
using ServiceProvider provider = services.BuildServiceProvider();
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
```

## Make first API calls

```csharp
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

TorBoxResponse<IReadOnlyList<Torrent>> myTorrents =
    await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts.Token);

TorBoxResponse<TorrentSearchResponse> search =
    await client.Search.SearchTorrentsAsync("ubuntu", cancellationToken: cts.Token);
```

## Common verification checks

- `client.Main` is reachable
- calls return `TorBoxResponse<T>`
- failures are caught as `TorBoxException`

## Troubleshooting

- missing API key: ensure `TORBOX_API_KEY` exists in process environment
- timeout: increase `options.Timeout`
- DNS/network: check HTTPS connectivity to TorBox endpoints
