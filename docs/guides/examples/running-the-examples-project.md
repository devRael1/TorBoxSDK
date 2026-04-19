---
uid: examples-running-project
title: Running the Examples Project
description: Run the TorBoxSDK examples runner and understand how the sample application is structured.
---

# Running the Examples Project

This page shows how to execute the examples project and how the menu-driven runner maps to the source files.

## Prerequisites

- .NET SDK installed
- a TorBox API key in the `TORBOX_API_KEY` environment variable
- the repository checked out locally

## Run the examples

From the repository root:

```bash
dotnet run --project src/TorBoxSDK.Examples
```

The console application displays a numbered menu implemented in [Program.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Program.cs). Each menu entry executes one example class through its `RunAsync()` method.

## Shared infrastructure

The shared helper in [ExampleHelper.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Helpers/ExampleHelper.cs) provides the common setup used across most examples:

- creates a DI-based `ITorBoxClient`
- reads `TORBOX_API_KEY`
- centralizes a default 30-second timeout
- keeps a shared `ServiceProvider` alive for the runner lifetime

That helper is useful for examples, but it is not part of the public SDK API.

## Menu categories

The runner groups examples into these categories:

- Getting Started
- Main API
- Search Client
- Relay Client
- Error Handling

## Source map

| Category | Representative source files |
|---|---|
| Getting Started | [BasicSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/BasicSetupExample.cs), [ConfigurationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/ConfigurationExample.cs), [StandaloneSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs) |
| Main API | [CreateTorrentExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/CreateTorrentExample.cs), [AuthenticationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/AuthenticationExample.cs), [OAuthExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Integrations/OAuthExample.cs) |
| Search Client | [SearchTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchTorrentsExample.cs), [DownloadSearchResultsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/DownloadSearchResultsExample.cs) |
| Relay Client | [RelayExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Relay/RelayExample.cs) |
| Error Handling | [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs) |

## Good first examples

If you are new to TorBoxSDK, start in this order:

1. [BasicSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/BasicSetupExample.cs)
2. [StandaloneSetupExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs)
3. [ListTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/ListTorrentsExample.cs)
4. [SearchTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Search/SearchTorrentsExample.cs)
5. [ErrorHandlingExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/ErrorHandling/ErrorHandlingExample.cs)
