---
uid: error-handling-error-codes
title: Error Codes and Mapping
description: How TorBox API errors map to TorBoxErrorCode and how to branch behavior safely.
---

# Error Codes and Mapping

TorBoxSDK maps API error strings to <xref:TorBoxSDK.Models.Common.TorBoxErrorCode>.

## Why this matters

Typed codes let you implement deterministic behavior without brittle string matching.

## Typical branches

```csharp
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.BadToken)
{
    // Refresh credentials or fail fast.
}
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.TooManyRequests)
{
    // Backoff and retry later.
}
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.PermissionDenied)
{
    // Surface authorization guidance to user.
}
```

## Unknown mapping behavior

Unrecognized server error strings map to `TorBoxErrorCode.Unknown`.

Treat `Unknown` as recoverable by default unless business logic requires strict failure.

## Logging guidance

Prefer structured logs:

```csharp
logger.LogWarning("TorBox API error {ErrorCode}: {Detail}", ex.ErrorCode, ex.Detail ?? ex.Message);
```
