---
uid: architecture-dependency-injection
title: Dependency Injection
description: How AddTorBox registers TorBoxSDK and how to consume ITorBoxClient correctly.
---

# Dependency Injection

TorBoxSDK integrates with `IServiceCollection` through <xref:TorBoxSDK.DependencyInjection.TorBoxServiceCollectionExtensions.AddTorBox*>.

## Registration model

`AddTorBox(...)` registers:

- named `HttpClient` pipelines for Main, Search, and Relay
- options binding for <xref:TorBoxSDK.TorBoxClientOptions>
- one public entry point: <xref:TorBoxSDK.ITorBoxClient>

Sub-clients are internal implementation details and are not registered individually.

## Correct consumption

```csharp
services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Missing TorBox API key.");
});

using ServiceProvider provider = services.BuildServiceProvider();
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();
```

## Standalone mode

For non-DI environments, instantiate <xref:TorBoxSDK.TorBoxClient> directly:

```csharp
using TorBoxClient client = new("your-api-key");
```

## Lifetime and disposal

- In DI mode, the container controls `HttpClient` lifetime.
- In standalone mode, dispose `TorBoxClient` with `using`.
