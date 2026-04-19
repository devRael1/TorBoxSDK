---
uid: examples-overview
title: Examples Overview
description: Learn how the TorBoxSDK examples project is organized and where to find runnable code for common SDK workflows.
---

# Examples Overview

This guide is for developers who want to learn TorBoxSDK by starting from runnable code instead of isolated snippets.

The examples live in the examples project:

- [Examples project folder](https://github.com/devRael1/TorBoxSDK/tree/master/src/TorBoxSDK.Examples)
- [Examples runner](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Program.cs)

The project currently contains 38 runnable scenarios grouped by onboarding, Main API workflows, Search API usage, Relay usage, and error handling patterns.

## What the examples show

- dependency injection setup with `AddTorBox()`
- standalone `TorBoxClient` usage without DI
- configuration binding from `appsettings.json`
- end-to-end Main API workflows such as creating, listing, editing, and controlling downloads
- Search API workflows for torrents, Usenet, metadata, tutorials, and download handoff
- Relay API monitoring calls
- production-oriented error handling and cancellation patterns

## How to use this documentation

Start with the runner and setup examples if you are new to the SDK. Move to the Main API guide when you want real resource workflows. Use the Search, Relay, and error-handling guide when you need cross-cutting patterns.

## Guides in this section

- [Running the Examples Project](examples/running-the-examples-project.md)
- [Setup and Configuration Examples](examples/setup-and-configuration-examples.md)
- [Main API Examples](examples/main-api-examples.md)
- [Search, Relay, and Error Handling Examples](examples/search-relay-and-error-handling-examples.md)

## Quick mapping

| Need | Guide |
|---|---|
| Run the menu-driven sample app | [Running the Examples Project](examples/running-the-examples-project.md) |
| Configure DI, `appsettings.json`, or standalone client usage | [Setup and Configuration Examples](examples/setup-and-configuration-examples.md) |
| Explore torrents, Usenet, web downloads, user, integrations, notifications, RSS, vendors, queued, stream, or general endpoints | [Main API Examples](examples/main-api-examples.md) |
| Explore Search API, Relay API, and resilience patterns | [Search, Relay, and Error Handling Examples](examples/search-relay-and-error-handling-examples.md) |

## Guidance for copying examples into real applications

- Replace placeholder IDs, tokens, hashes, file paths, and URLs before running any workflow.
- Prefer environment variables or configuration binding for secrets; do not hardcode API keys.
- Keep the `CancellationToken` pattern used in the examples.
- Treat `ExampleHelper` as sample infrastructure, not as part of the public SDK surface.
