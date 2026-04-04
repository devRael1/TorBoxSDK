# Error Handling

TorBoxSDK uses two related patterns:

- successful responses return `TorBoxResponse` or `TorBoxResponse<T>`
- unsuccessful API responses throw `TorBoxException`

## Response envelope

The standard TorBox envelope includes:

- `Success`
- `Error`
- `Detail`
- `Data` for generic responses

## Exception model

`TorBoxException` includes:

- `Message`
- `ErrorCode`
- `Detail`

`ErrorCode` is a `TorBoxErrorCode` value, which exposes known TorBox API error codes in a strongly typed form.

## Basic example

```csharp
try
{
    await client.Main.Torrents.GetMyTorrentListAsync(ct: cancellationToken);
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
- Transport and API-level errors are normalized into `TorBoxException`
- Unmapped API error strings fall back to `TorBoxErrorCode.Unknown`
