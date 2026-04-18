---
uid: getting-started
title: Getting Started
description: Install TorBoxSDK and make your first request to the TorBox API.
---

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

## Use without dependency injection

For console apps, scripts, or environments without a DI container, create the client directly:

```csharp
using TorBoxSDK;

string apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
    ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable.");

using TorBoxClient client = new(apiKey);
```

You can also pass a <xref:TorBoxSDK.TorBoxClientOptions> instance or a configuration delegate:

```csharp
using TorBoxClient client = new(new TorBoxClientOptions
{
    ApiKey = apiKey,
    Timeout = TimeSpan.FromSeconds(60)
});
```

<xref:TorBoxSDK.TorBoxClient> implements `IDisposable`. Always use a `using` statement to ensure HTTP clients are properly released. In DI mode, the container manages the lifecycle automatically.

> **When to choose standalone vs DI?**
> Use standalone for simple console tools, scripts, and one-off programs. Use DI for ASP.NET Core apps, hosted services, and anything with `IServiceCollection`.

## Make your first requests

```csharp
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;
using TorBoxSDK.Models.Torrents;

TorBoxResponse<IReadOnlyList<Torrent>> torrents = await client.Main.Torrents.GetMyTorrentListAsync();
TorBoxResponse<TorrentSearchResponse> results = await client.Search.SearchTorrentsAsync("ubuntu");
```

## What to expect from responses

- Successful calls return <xref:TorBoxSDK.Models.Common.TorBoxResponse`1>
- API errors throw <xref:TorBoxSDK.TorBoxException>
- All SDK methods are asynchronous
- All public async methods accept `CancellationToken`

## Next steps

- [API Reference](api-reference.md)
- [Configuration Reference](configuration.md)
- [Error Handling](error-handling.md)
- [Architecture Overview](architecture.md)
