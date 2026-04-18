---
uid: architecture-client-hierarchy
title: Client Hierarchy
description: Detailed hierarchy of TorBoxSDK clients and practical navigation from ITorBoxClient.
---

# Client Hierarchy

TorBoxSDK exposes one root client and three API families.

## Canonical tree

```text
TorBoxClient (ITorBoxClient)
|- Main (IMainApiClient)
|  |- General
|  |- Torrents
|  |- Usenet
|  |- WebDownloads
|  |- User
|  |- Notifications
|  |- Rss
|  |- Stream
|  |- Integrations
|  |- Vendors
|  └─ Queued
|- Search (ISearchApiClient)
└─ Relay (IRelayApiClient)
```

## Entry point

Always resolve <xref:TorBoxSDK.ITorBoxClient> and navigate through its properties:

```csharp
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

IMainApiClient main = client.Main;
ISearchApiClient search = client.Search;
IRelayApiClient relay = client.Relay;
```

## Main API resource clients

`Main` groups 11 focused resource clients:

- <xref:TorBoxSDK.Main.General.IGeneralClient>
- <xref:TorBoxSDK.Main.Torrents.ITorrentsClient>
- <xref:TorBoxSDK.Main.Usenet.IUsenetClient>
- <xref:TorBoxSDK.Main.WebDownloads.IWebDownloadsClient>
- <xref:TorBoxSDK.Main.User.IUserClient>
- <xref:TorBoxSDK.Main.Notifications.INotificationsClient>
- <xref:TorBoxSDK.Main.Rss.IRssClient>
- <xref:TorBoxSDK.Main.Stream.IStreamClient>
- <xref:TorBoxSDK.Main.Integrations.IIntegrationsClient>
- <xref:TorBoxSDK.Main.Vendors.IVendorsClient>
- <xref:TorBoxSDK.Main.Queued.IQueuedClient>

## Design intent

This structure keeps discoverability high in IntelliSense while preventing one giant flat client.
