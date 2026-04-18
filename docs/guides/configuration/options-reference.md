---
uid: configuration-options-reference
title: Options Reference
description: Detailed reference for TorBoxClientOptions and practical defaults.
---

# Options Reference

TorBoxSDK is configured by <xref:TorBoxSDK.TorBoxClientOptions>.

## Core options

| Property | Required | Typical value | Notes |
|---|---|---|---|
| `ApiKey` | Yes | `TORBOX_API_KEY` | Required for authenticated calls |
| `MainApiBaseUrl` | No | `https://api.torbox.app/` | Host only, trailing slash recommended |
| `ApiVersion` | Yes | `v1` | Used to compute versioned main/relay URLs |
| `SearchApiBaseUrl` | No | `https://search-api.torbox.app/` | Search host URL |
| `RelayApiBaseUrl` | No | `https://relay.torbox.app/` | Relay host URL |
| `Timeout` | No | `00:00:30` | Shared HTTP timeout |

## Computed URLs

- `MainApiVersionedUrl` is derived from `MainApiBaseUrl` + `ApiVersion`
- `RelayApiVersionedUrl` is derived from `RelayApiBaseUrl` + `ApiVersion`

These should not be manually forced unless you fully control endpoint routing.

## Practical configuration

```csharp
services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Missing TORBOX_API_KEY.");
    options.MainApiBaseUrl = "https://api.torbox.app/";
    options.SearchApiBaseUrl = "https://search-api.torbox.app/";
    options.RelayApiBaseUrl = "https://relay.torbox.app/";
    options.ApiVersion = "v1";
    options.Timeout = TimeSpan.FromSeconds(30);
});
```
