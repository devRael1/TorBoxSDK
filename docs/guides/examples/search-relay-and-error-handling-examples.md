---
uid: examples-search-relay-errors
title: Search, Relay, and Error Handling Examples
description: Review TorBoxSDK examples for Search API calls, Relay monitoring, and production-oriented error handling patterns.
---

# Search, Relay, and Error Handling Examples

These examples cover the remaining SDK families outside the Main API and the cross-cutting error-handling patterns that apply everywhere.

## Search API examples

The Search API examples show how to query torrents, Usenet, metadata, and tutorials, then retrieve or hand off results.

| Scenario | Source file |
|---|---|
| Search torrents and fetch result details by ID | [SearchTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchTorrentsExample.cs) |
| Search Usenet content | [SearchUsenetExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchUsenetExample.cs) |
| Search metadata such as movies and TV | [SearchMetaExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchMetaExample.cs) |
| Search tutorials | [SearchTutorialsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchTutorialsExample.cs) |
| Download search results and retrieve search entries by ID | [DownloadSearchResultsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/DownloadSearchResultsExample.cs) |

If you want a good first Search API example, start with [SearchTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchTorrentsExample.cs). It demonstrates a simple query, advanced options, TV-specific filters, and a follow-up detail call.

## Relay API example

The relay surface is covered by a focused example:

| Scenario | Source file |
|---|---|
| Check relay server status and torrent inactivity | [RelayExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Relay/RelayExample.cs) |

Use it when you need to understand `client.Relay` workflows rather than content-management workflows in `client.Main`.

## Error handling patterns

The error-handling example is useful even if you are not using the menu runner. It acts as a reference for production code structure.

| Pattern | Source file |
|---|---|
| API-level `TorBoxException` handling | [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) |
| `HttpRequestException` transport failures | [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) |
| timeout and cancellation behavior | [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) |
| combined production-style catch structure | [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) |

## Recommended usage pattern

- Start from a setup example in [Setup and Configuration Examples](setup-and-configuration-examples.md).
- Pick the Search or Relay example closest to your scenario.
- Reuse the catch structure from [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) in your real application code.
