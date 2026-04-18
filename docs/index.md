---
uid: index
title: TorBoxSDK Documentation
description: Unofficial C# SDK for the TorBox API — typed clients for Main, Search, and Relay APIs with dependency injection support.
---

# TorBoxSDK

[![NuGet](https://img.shields.io/badge/NuGet-TorBoxSDK-blue?logo=nuget)](https://www.nuget.org/packages/TorBoxSDK)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/devRael1/TorBoxSDK/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-purple)](#)

**Important:** This SDK is unofficial and is not affiliated with or endorsed by TorBox.

TorBoxSDK is an open-source, MIT-licensed C# SDK for the [TorBox API](https://api-docs.torbox.app/). It gives .NET applications typed access to the TorBox Main, Search, and Relay APIs with dependency injection support, consistent response handling, and a public surface designed to be easy to explore in IntelliSense.

## Quick Links

- [Getting Started](articles/getting-started.md) — install and make your first request
- [API Reference](api/index.md) — auto-generated reference from XML documentation
- [Architecture Overview](articles/architecture.md) — understand the client hierarchy
- [Configuration](articles/configuration.md) — configure options, base URLs, and timeouts
- [Error Handling](articles/error-handling.md) — handle API errors and exceptions

## Why TorBoxSDK?

- Covers the TorBox Main, Search, and Relay APIs through a single root client
- Organizes the Main API into focused resource clients such as Torrents, Usenet, Web Downloads, User, Notifications, RSS, and Integrations
- Multi-targets `.NET 6` through `.NET 10`
- Integrates with `IServiceCollection`, `IHttpClientFactory`, and configuration binding
- Uses the standard `TorBoxResponse` envelope and surfaces API failures through `TorBoxException`
- Ships XML documentation, SourceLink metadata, symbols, and package README support

## Quick Start

```csharp
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Models.Common;
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

    Console.WriteLine($"Torrents returned: {torrents.Data?.Count ?? 0}");
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"TorBox API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
}
```

## Source Code

The source code is available on [GitHub](https://github.com/devRael1/TorBoxSDK).
