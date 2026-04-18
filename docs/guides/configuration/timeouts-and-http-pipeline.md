---
uid: configuration-timeouts-http-pipeline
title: Timeouts and HTTP Pipeline
description: Configure request timeout behavior and understand SDK HttpClient pipeline usage.
---

# Timeouts and HTTP Pipeline

TorBoxSDK uses `IHttpClientFactory` when registered through DI.

## Timeout strategy

A single `Timeout` option is applied to SDK HTTP clients.

```csharp
services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Missing TORBOX_API_KEY.");
    options.Timeout = TimeSpan.FromSeconds(45);
});
```

Choose timeout based on workload:

- metadata/status calls: 10-30s
- larger search/list operations: 30-60s
- unreliable network zones: consider higher timeout + retries upstream

## Pipeline behavior

- authentication is attached by internal handler
- sub-clients share configured named HTTP pipelines
- root `ITorBoxClient` exposes all API families through one coherent entry point

## Cancellation tokens

Even with global timeout, always pass `CancellationToken` for call-level cancellation:

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(20));
await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts.Token);
```
