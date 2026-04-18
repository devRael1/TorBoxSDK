---
uid: getting-started-standalone-client
title: Standalone Client and Lifetime
description: Use TorBoxClient without dependency injection and manage HttpClient lifetime safely.
---

# Standalone Client and Lifetime

Standalone mode is ideal for console tools, scripts, and short-running jobs.

## Constructors

TorBoxSDK supports multiple standalone creation patterns:

```csharp
using TorBoxSDK;

using TorBoxClient clientA = new("your-api-key");

using TorBoxClient clientB = new(new TorBoxClientOptions
{
    ApiKey = "your-api-key",
    Timeout = TimeSpan.FromSeconds(60)
});

using TorBoxClient clientC = new(options =>
{
    options.ApiKey = "your-api-key";
    options.Timeout = TimeSpan.FromSeconds(45);
});
```

## Why `using` matters

`TorBoxClient` implements `IDisposable`.

In standalone mode it owns internal HTTP clients, so always dispose it via `using` to release sockets and handlers quickly.

## Practical request example

```csharp
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

using TorBoxClient client = new("your-api-key");
TorBoxResponse<UserProfile> me = await client.Main.User.GetMeAsync();
Console.WriteLine(me.Data?.Email);
```

## Best practices

- reuse a client instance during a logical unit of work
- do not create/dispose a new client per request in tight loops
- pass `CancellationToken` for long-running operations
