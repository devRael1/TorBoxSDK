---
uid: architecture-responses-and-exceptions
title: Responses and Exceptions
description: Response envelope, generic payload typing, and API exception behavior in TorBoxSDK.
---

# Responses and Exceptions

TorBoxSDK normalizes API interactions around two concepts.

## Response envelopes

Most methods return:

- <xref:TorBoxSDK.Models.Common.TorBoxResponse>
- <xref:TorBoxSDK.Models.Common.TorBoxResponse`1>

The envelope follows TorBox fields:

- `Success`
- `Error`
- `Detail`
- `Data` (generic variant)

## Exception behavior

API-level failures are surfaced as <xref:TorBoxSDK.Models.Common.TorBoxException>.

`TorBoxException` includes:

- message
- <xref:TorBoxSDK.Models.Common.TorBoxErrorCode>
- detail text when available

## Example pattern

```csharp
try
{
    TorBoxResponse<IReadOnlyList<Torrent>> response =
        await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cancellationToken);

    int count = response.Data?.Count ?? 0;
    Console.WriteLine($"Torrents: {count}");
}
catch (TorBoxException ex) when (ex.ErrorCode == TorBoxErrorCode.BadToken)
{
    Console.Error.WriteLine("Invalid or expired API key.");
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"TorBox API error: {ex.Detail ?? ex.Message}");
}
```

## Transport-level failures

Transport failures remain standard .NET exceptions (`HttpRequestException`, `TaskCanceledException`), so callers can use existing retry/cancellation patterns.
