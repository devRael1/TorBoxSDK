---
uid: api-index
title: API Reference
description: Auto-generated API reference for TorBoxSDK, built from XML documentation comments.
---

# API Reference

This section contains the auto-generated API reference for TorBoxSDK, built from the XML documentation comments (`///`) in the source code.

## Client Hierarchy

The SDK is structured around a single root client with three API families:

- **<xref:TorBoxSDK.ITorBoxClient>** — the single entry point for all SDK functionality
  - **Main** (<xref:TorBoxSDK.Main.IMainApiClient>) — the largest surface, split into 11 resource clients
    - Torrents, Usenet, Web Downloads, User, Notifications, RSS, Stream, Integrations, Vendors, Queued, General
  - **Search** (<xref:TorBoxSDK.Search.ISearchApiClient>) — search-oriented endpoints for torrents, usenet, metadata, Torznab, and Newznab
  - **Relay** (<xref:TorBoxSDK.Relay.IRelayApiClient>) — relay status and inactivity checks

## Key Types

| Type | Description |
|---|---|
| <xref:TorBoxSDK.ITorBoxClient> | Root SDK interface — the only type exposed via DI |
| <xref:TorBoxSDK.TorBoxClient> | Concrete client implementation supporting both DI and standalone usage |
| <xref:TorBoxSDK.TorBoxClientOptions> | Configuration options (API key, base URLs, timeout) |
| <xref:TorBoxSDK.Models.Common.TorBoxResponse`1> | Standard API response envelope with typed data |
| <xref:TorBoxSDK.TorBoxException> | Exception thrown on API-level failures |

## Browsing

Use the table of contents on the left to browse namespaces, classes, interfaces, and their members. Each type page includes:

- Summary and remarks from XML documentation
- Constructor, property, and method signatures
- Parameter descriptions and return types
- Links to related types via cross-references
