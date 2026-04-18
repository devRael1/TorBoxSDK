---
uid: error-handling-exception-model
title: Exception Model
description: Understand TorBoxException semantics and transport-level exceptions in TorBoxSDK.
---

# Exception Model

TorBoxSDK separates API failures from transport failures.

## API failures

API failures are normalized into <xref:TorBoxSDK.Models.Common.TorBoxException>.

Main properties:

- `ErrorCode` (typed enum)
- `Detail` (server detail text)
- `Message` (exception message)

## Transport failures

Transport-level issues are still native .NET exceptions, commonly:

- `HttpRequestException`
- `TaskCanceledException`

## Catch pattern

```csharp
try
{
    await client.Main.User.GetMeAsync(cancellationToken: cancellationToken);
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"API failure [{ex.ErrorCode}] {ex.Detail ?? ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"Network/HTTP failure: {ex.Message}");
}
catch (TaskCanceledException)
{
    Console.Error.WriteLine("Request canceled or timed out.");
}
```
