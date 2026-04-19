---
uid: examples-setup-configuration
title: Setup and Configuration Examples
description: Review TorBoxSDK examples for dependency injection, configuration binding, and standalone client setup.
---

# Setup and Configuration Examples

These examples are the best starting point if you want to understand how to create a client, configure options, and make the first authenticated request.

## Dependency injection setup

Use [BasicSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/BasicSetupExample.cs) when you want the recommended integration style for ASP.NET Core and other DI-based applications.

This example shows how to:

- register the SDK with `AddTorBox()`
- read `TORBOX_API_KEY` from the environment
- resolve `ITorBoxClient` from `ServiceProvider`
- make a first `Main.User.GetMeAsync()` call with a `CancellationToken`

## Configuration binding from appsettings.json

Use [ConfigurationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/ConfigurationExample.cs) when your application already uses `IConfiguration`.

This example shows how to:

- bind the `TorBox` configuration section through `services.AddTorBox(configuration)`
- fall back to `TORBOX_API_KEY` if no `TorBox` section exists
- keep base URLs and timeout values in configuration instead of code

## Standalone client usage

Use [StandaloneSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs) when you do not want a DI container.

This example covers all three construction patterns:

- `new TorBoxClient(apiKey)`
- `new TorBoxClient(new TorBoxClientOptions { ... })`
- `new TorBoxClient(options => { ... })`

It also demonstrates correct `IDisposable` usage with `using` blocks.

## Shared sample helper

Many example files use [ExampleHelper.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Helpers/ExampleHelper.cs) to centralize configuration and timeout creation.

Treat it as examples infrastructure only. In a real application, you would normally integrate the SDK directly into your own host, service registration, configuration, and logging setup.

## Related source files

| Scenario | Source file |
|---|---|
| Basic DI registration and first user request | [BasicSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/BasicSetupExample.cs) |
| Configuration binding and appsettings fallback | [ConfigurationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/ConfigurationExample.cs) |
| Standalone client constructors and disposal | [StandaloneSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs) |
| Runner-wide helper methods and provider reuse | [ExampleHelper.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Helpers/ExampleHelper.cs) |

## Next step

After setup is clear, continue with [Main API Examples](main-api-examples.md) to see complete resource workflows.
