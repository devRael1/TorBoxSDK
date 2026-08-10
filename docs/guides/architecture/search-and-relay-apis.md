---
uid: architecture-search-relay-apis
title: Search and Relay APIs
description: Understand the dedicated Search and Relay API clients in TorBoxSDK.
---

# Search and Relay APIs

Beyond `Main`, TorBoxSDK exposes two dedicated API families through the root client.

## Search API

Access via <xref:TorBoxSDK.Search.ISearchApiClient> (`client.Search`).

> [!IMPORTANT]
> The TorBox Search API is not generally public. Since May 20, 2026, TorBox
> restricts access to approved projects and whitelisted source IP addresses.
> Installing TorBoxSDK or supplying a paid-account API key does not grant
> access. The application server's outbound IP must be approved by TorBox.

Typical scenarios:

- torrent search and detailed torrent retrieval
- Usenet search and NZB retrieval
- metadata search
- Torznab and Newznab style XML search

```csharp
TorBoxResponse<TorrentSearchResponse> searchResults =
    await client.Search.SearchTorrentsAsync("ubuntu", cancellationToken: cancellationToken);
```

This example applies only to an authorized deployment. DNS resolution or API
access can fail for other callers. Contact TorBox support before making Search
API availability a production dependency.

## Relay API

Access via <xref:TorBoxSDK.Relay.IRelayApiClient> (`client.Relay`).

Typical scenarios:

- relay service status checks
- inactivity checks for relay links

```csharp
TorBoxResponse<RelayStatus> status =
    await client.Relay.GetStatusAsync(cancellationToken: cancellationToken);
```

## Separation rationale

- Search and Relay have distinct hosts and semantics.
- Consumers can reason about each API family independently.
- The root client remains simple while preserving explicit boundaries.
