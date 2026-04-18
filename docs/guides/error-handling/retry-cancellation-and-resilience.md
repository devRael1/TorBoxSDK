---
uid: error-handling-retry-cancellation-resilience
title: Retry, Cancellation, and Resilience
description: Resilience patterns for retries, backoff, and cooperative cancellation around TorBoxSDK calls.
---

# Retry, Cancellation, and Resilience

Reliable clients combine retry policy, cancellation, and clear timeout boundaries.

## Retry principles

Retry only transient failures:

- `TooManyRequests`
- temporary network failures
- gateway/timeouts from upstream infrastructure

Avoid retrying permanent failures:

- `BadToken`
- `PermissionDenied`
- malformed request parameters

## Simple retry with backoff

```csharp
for (int attempt = 1; attempt <= 3; attempt++)
{
    try
    {
        await client.Search.SearchTorrentsAsync("ubuntu", cancellationToken: cancellationToken);
        break;
    }
    catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.TooManyRequests && attempt < 3)
    {
        await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
    }
}
```

## Cancellation boundaries

Use operation-level cancellation source for user-facing actions:

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(20));
await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cts.Token);
```

## Recommended production pattern

- centralize retry policy in app service layer
- keep SDK calls pure and typed
- log error code + endpoint intent per failure
- enforce idempotency awareness for retried operations
