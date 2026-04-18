---
uid: error-handling
title: Error Handling Overview
description: Handle TorBox API errors and exceptions in TorBoxSDK.
---

# Error Handling Overview

TorBoxSDK uses two related patterns:

- successful responses return <xref:TorBoxSDK.Models.Common.TorBoxResponse> or <xref:TorBoxSDK.Models.Common.TorBoxResponse`1>
- unsuccessful API responses throw <xref:TorBoxSDK.Models.Common.TorBoxException>
- transport failures and cancellations may still surface as `HttpRequestException` or `TaskCanceledException`

## Response envelope

The standard TorBox envelope includes:

- `Success`
- `Error`
- `Detail`
- `Data` for generic responses

## Exception model

<xref:TorBoxSDK.Models.Common.TorBoxException> includes:

- `Message`
- `ErrorCode`
- `Detail`

`ErrorCode` is a <xref:TorBoxSDK.Models.Common.TorBoxErrorCode> value, which exposes known TorBox API error codes in a strongly typed form.

## Basic example

```csharp
try
{
    await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cancellationToken);
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"Error code: {ex.ErrorCode}");
    Console.Error.WriteLine($"Detail: {ex.Detail ?? ex.Message}");
}
```

## Handling specific TorBox errors

```csharp
try
{
    await client.Search.SearchTorrentsAsync("ubuntu", cancellationToken);
}
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.BadToken)
{
    Console.Error.WriteLine("The TorBox API key is invalid or expired.");
}
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.TooManyRequests)
{
    Console.Error.WriteLine("Rate limited by the API. Retry later.");
}
```

## Notes

- All public async methods accept `CancellationToken`
- API-level errors are normalized into <xref:TorBoxSDK.Models.Common.TorBoxException>
- Transport failures and cancellations may surface as `HttpRequestException` or `TaskCanceledException`
- Unmapped API error strings fall back to `TorBoxErrorCode.Unknown`


