---
uid: examples-main-api
title: Main API Examples
description: Browse TorBoxSDK example files for torrents, Usenet, web downloads, user, integrations, notifications, RSS, vendors, queued downloads, stream, and general endpoints.
---

# Main API Examples

The Main API examples cover the largest part of the SDK surface. They are organized by resource client under the examples project.

## Torrents

The torrents examples cover list, create, control, download, cache checks, and edit workflows.

| Scenario | Source file |
|---|---|
| List torrents | [ListTorrentsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/ListTorrentsExample.cs) |
| Create torrents from magnet or file | [CreateTorrentExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/CreateTorrentExample.cs) |
| Pause, resume, or delete torrents | [ControlTorrentExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/ControlTorrentExample.cs) |
| Download generated files | [DownloadTorrentExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/DownloadTorrentExample.cs) |
| Check cached hashes | [CheckCachedExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/CheckCachedExample.cs) |
| Edit torrent metadata and related workflows | [EditTorrentExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Torrents/EditTorrentExample.cs) |

## Usenet

| Scenario | Source file |
|---|---|
| List Usenet downloads | [ListUsenetExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Usenet/ListUsenetExample.cs) |
| Create a Usenet download | [CreateUsenetExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Usenet/CreateUsenetExample.cs) |
| Advanced cache, edit, and async creation flows | [UsenetAdvancedExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Usenet/UsenetAdvancedExample.cs) |

## Web Downloads

| Scenario | Source file |
|---|---|
| List web downloads | [ListWebDownloadsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/WebDownloads/ListWebDownloadsExample.cs) |
| Create web downloads and inspect hosters | [CreateWebDownloadExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/WebDownloads/CreateWebDownloadExample.cs) |
| Cache checks, edit, and async create | [WebDownloadsAdvancedExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/WebDownloads/WebDownloadsAdvancedExample.cs) |

## User and account workflows

These examples cover profile retrieval, settings, authentication, search engines, and billing data.

| Scenario | Source file |
|---|---|
| Get profile and account details | [GetProfileExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/GetProfileExample.cs) |
| Edit user settings | [ManageSettingsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/ManageSettingsExample.cs) |
| Refresh token, device auth, confirmation, delete account flow | [AuthenticationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/AuthenticationExample.cs) |
| Manage search engines | [SearchEnginesExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/SearchEnginesExample.cs) |
| Transactions and PDF export | [TransactionsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/User/TransactionsExample.cs) |

## Integrations

These examples cover cloud providers, OAuth, Discord-linked roles, and background jobs.

| Scenario | Source file |
|---|---|
| Single cloud integration workflow | [CloudIntegrationExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Integrations/CloudIntegrationExample.cs) |
| All supported cloud providers | [AllCloudProvidersExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Integrations/AllCloudProvidersExample.cs) |
| OAuth redirect, callback, register, unregister, Discord roles | [OAuthExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Integrations/OAuthExample.cs) |
| Integration job management | [JobManagementExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Integrations/JobManagementExample.cs) |

## Other Main API clients

| Resource area | Source file |
|---|---|
| Notifications | [NotificationsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Notifications/NotificationsExample.cs) |
| RSS feeds | [RssFeedsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Rss/RssFeedsExample.cs) |
| Vendors | [VendorExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Vendors/VendorExample.cs) |
| Queued downloads | [QueuedDownloadsExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Queued/QueuedDownloadsExample.cs) |
| Streaming media files | [StreamExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/Stream/StreamExample.cs) |
| General service status and stats | [GeneralExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/General/GeneralExample.cs) |
| Speedtest and download helper files | [SpeedtestExample.cs](https://github.com/devRael1/TorBoxSDK/blob/master/src/TorBoxSDK.Examples/Main/General/SpeedtestExample.cs) |

## Suggested reading order

1. Start with torrents, because they show the clearest create/list/control lifecycle.
2. Move to user and integrations if your app needs account or provider setup.
3. Use the advanced Usenet and web download examples when you need async or cache-oriented flows.
