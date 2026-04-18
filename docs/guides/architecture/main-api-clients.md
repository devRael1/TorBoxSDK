---
uid: architecture-main-api-clients
title: Main API Clients
description: Detailed overview of the 11 Main API clients and their responsibilities.
---

# Main API Clients

The Main API is the largest TorBoxSDK surface. It is split into specialized clients under <xref:TorBoxSDK.Main.IMainApiClient>.

## Responsibilities by client

- `General`: service health, stats, changelog feeds, speedtest discovery.
- `Torrents`: torrent lifecycle, queue operations, cache checks, metadata lookup.
- `Usenet`: NZB/Usenet lifecycle operations and cache checks.
- `WebDownloads`: hoster and direct-download workflows.
- `User`: account profile, token flows, referrals, subscriptions, settings.
- `Notifications`: notification read/clear/test operations.
- `Rss`: RSS feed creation, control, and feed item retrieval.
- `Stream`: stream URL creation and stream metadata.
- `Integrations`: OAuth provider links and integration job lifecycle.
- `Vendors`: vendor account/user management.
- `Queued`: generic queue listing and control operations.

## Usage example

```csharp
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

TorBoxResponse<IReadOnlyList<Torrent>> myTorrents =
    await client.Main.Torrents.GetMyTorrentListAsync(cancellationToken: cancellationToken);

TorBoxResponse<UserProfile> me =
    await client.Main.User.GetMeAsync(cancellationToken: cancellationToken);
```

## Why this split matters

- Stronger discoverability for endpoint families.
- Easier maintenance and testing per client.
- More stable public API than one monolithic interface.
