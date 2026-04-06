# API Reference

This reference documents the full TorBoxSDK public API surface across the Main, Search, and Relay API families. All methods return the standard `TorBoxResponse` envelope, all async methods accept `CancellationToken cancellationToken = default`, and API failures may throw `TorBoxException`.

## Table of Contents

- [Main API](#main-api)
  - [General Client](#general-client)
  - [Torrents Client](#torrents-client)
  - [Usenet Client](#usenet-client)
  - [Web Downloads Client](#web-downloads-client)
  - [User Client](#user-client)
  - [Notifications Client](#notifications-client)
  - [RSS Client](#rss-client)
  - [Stream Client](#stream-client)
  - [Integrations Client](#integrations-client)
  - [Vendors Client](#vendors-client)
  - [Queued Client](#queued-client)
- [Search API](#search-api)
- [Relay API](#relay-api)
- [Models](#models)
  - [TorBoxSDK.Models.Common](#torboxsdkmodelscommon)
  - [TorBoxSDK.Models.Torrents](#torboxsdkmodelstorrents)
  - [TorBoxSDK.Models.Usenet](#torboxsdkmodelsusenet)
  - [TorBoxSDK.Models.WebDownloads](#torboxsdkmodelswebdownloads)
  - [TorBoxSDK.Models.User](#torboxsdkmodelsuser)
  - [TorBoxSDK.Models.Notifications](#torboxsdkmodelsnotifications)
  - [TorBoxSDK.Models.Rss](#torboxsdkmodelsrss)
  - [TorBoxSDK.Models.Integrations](#torboxsdkmodelsintegrations)
  - [TorBoxSDK.Models.Vendors](#torboxsdkmodelsvendors)
  - [TorBoxSDK.Models.Queued](#torboxsdkmodelsqueued)
  - [TorBoxSDK.Models.Search](#torboxsdkmodelssearch)
  - [TorBoxSDK.Models.Relay](#torboxsdkmodelsrelay)
  - [TorBoxSDK.Models.General](#torboxsdkmodelsgeneral)
- [Enums](#enums)

## Main API

The Main API is exposed through 11 resource-oriented clients under `ITorBoxClient.Main`.

### General Client

Interface: `IGeneralClient`

General service endpoints for root status, platform statistics, and speedtest file discovery.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetUpStatusAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets Main API status data. |
| `GetStatsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<Stats>` | Gets current aggregate TorBox statistics. |
| `Get30DayStatsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<Stats>` | Gets statistics for the last 30 days. |
| `GetSpeedtestFilesAsync` | `options: SpeedtestOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets speedtest file data using optional region, IP, and test length options. |

### Torrents Client

Interface: `ITorrentsClient`

Torrent lifecycle operations including creation, listing, cache checks, metadata lookup, queued handling, and download requests.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `CreateTorrentAsync` | `request: CreateTorrentRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<Torrent>` | Creates a torrent from a magnet link or torrent file. |
| `AsyncCreateTorrentAsync` | `request: CreateTorrentRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<Torrent>` | Creates a torrent using asynchronous server-side processing. |
| `ControlTorrentAsync` | `request: ControlTorrentRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation such as pause, resume, delete, or recheck. |
| `RequestDownloadAsync` | `options: RequestDownloadOptions`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Requests a torrent download URL. |
| `GetMyTorrentListAsync` | `id: long? = null`<br>`offset: int? = null`<br>`limit: int? = null`<br>`bypassCache: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Torrent>>` | Gets the authenticated user's torrent list or a single torrent when `id` is supplied. |
| `CheckCachedAsync` | `hashes: IReadOnlyList<string>`<br>`format: string? = null`<br>`listFiles: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks torrent cache availability using query parameters. |
| `CheckCachedByPostAsync` | `request: CheckCachedRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks torrent cache availability using a request body. |
| `ExportDataAsync` | `torrentId: long`<br>`type: string? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Exports torrent data in a requested format. |
| `GetTorrentInfoAsync` | `hash: string`<br>`timeout: int? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<TorrentInfo>` | Gets torrent metadata from an info hash. |
| `GetTorrentInfoByFileAsync` | `file: byte[]`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<TorrentInfo>` | Gets torrent metadata from raw torrent file bytes. |
| `EditTorrentAsync` | `request: EditTorrentRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates torrent properties such as name, tags, or alternative hashes. |
| `GetQueuedTorrentsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<QueuedDownload>>` | Gets queued torrent downloads. |
| `ControlQueuedTorrentsAsync` | `request: ControlQueuedRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation to queued torrent downloads. |
| `MagnetToFileAsync` | `request: MagnetToFileRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Converts a magnet link to torrent file content. |

### Usenet Client

Interface: `IUsenetClient`

Usenet download management, cache checks, editing, and download URL requests.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `CreateUsenetDownloadAsync` | `request: CreateUsenetDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UsenetDownload>` | Creates a Usenet download from a link or NZB file. |
| `AsyncCreateUsenetDownloadAsync` | `request: CreateUsenetDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UsenetDownload>` | Creates a Usenet download using asynchronous server-side processing. |
| `ControlUsenetDownloadAsync` | `request: ControlUsenetDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation to a Usenet download. |
| `RequestDownloadAsync` | `options: RequestUsenetDownloadOptions`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Requests a Usenet download URL. |
| `GetMyUsenetListAsync` | `id: long? = null`<br>`offset: int? = null`<br>`limit: int? = null`<br>`bypassCache: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<UsenetDownload>>` | Gets the authenticated user's Usenet downloads or a single item when `id` is supplied. |
| `CheckCachedAsync` | `hashes: IReadOnlyList<string>`<br>`format: string? = null`<br>`listFiles: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks Usenet cache availability using query parameters. |
| `CheckCachedByPostAsync` | `request: CheckUsenetCachedRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks Usenet cache availability using a request body. |
| `EditUsenetDownloadAsync` | `request: EditUsenetDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates Usenet download properties. |

### Web Downloads Client

Interface: `IWebDownloadsClient`

Direct-download and hoster operations including creation, cache checks, hoster discovery, editing, and download requests.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `CreateWebDownloadAsync` | `request: CreateWebDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<WebDownload>` | Creates a web download from a direct URL. |
| `AsyncCreateWebDownloadAsync` | `request: CreateWebDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<WebDownload>` | Creates a web download using asynchronous server-side processing. |
| `ControlWebDownloadAsync` | `request: ControlWebDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation to a web download. |
| `RequestDownloadAsync` | `options: RequestWebDownloadOptions`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Requests a web download URL. |
| `GetMyWebDownloadListAsync` | `id: long? = null`<br>`offset: int? = null`<br>`limit: int? = null`<br>`bypassCache: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<WebDownload>>` | Gets the authenticated user's web downloads or a single item when `id` is supplied. |
| `CheckCachedAsync` | `hashes: IReadOnlyList<string>`<br>`format: string? = null`<br>`listFiles: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks web-download cache availability using query parameters. |
| `CheckCachedByPostAsync` | `request: CheckWebCachedRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Checks web-download cache availability using a request body. |
| `GetHostersAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Hoster>>` | Gets supported hosters and current bandwidth status. |
| `EditWebDownloadAsync` | `request: EditWebDownloadRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates web download properties. |

### User Client

Interface: `IUserClient`

Authentication, account profile, referrals, subscriptions, transactions, search engine settings, and user settings.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `RefreshTokenAsync` | `request: RefreshTokenRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Refreshes a session token. |
| `GetConfirmationAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets confirmation-related account data. |
| `GetMeAsync` | `settings: bool? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UserProfile>` | Gets the authenticated user's profile, optionally including settings. |
| `AddReferralAsync` | `referralCode: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a referral code to the authenticated user. |
| `StartDeviceAuthAsync` | `app: string? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<DeviceCodeResponse>` | Starts the device authorization flow. |
| `GetDeviceTokenAsync` | `request: DeviceTokenRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Exchanges a device code for token data. |
| `DeleteMeAsync` | `request: DeleteAccountRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Deletes the authenticated user's account. |
| `GetReferralDataAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<ReferralData>` | Gets referral totals and referral code data. |
| `GetSubscriptionsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Subscription>>` | Gets the user's subscriptions. |
| `GetTransactionsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Transaction>>` | Gets the user's transactions. |
| `GetTransactionPdfAsync` | `transactionId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets PDF data or a PDF link for a transaction. |
| `AddSearchEnginesAsync` | `request: AddSearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Adds search engines to the user's configuration. |
| `GetSearchEnginesAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<SearchEngine>>` | Gets configured search engines. |
| `ModifySearchEnginesAsync` | `request: ModifySearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Replaces or updates the configured search engine list. |
| `ControlSearchEnginesAsync` | `request: ControlSearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control-style operation to search engine settings. |
| `EditSettingsAsync` | `request: EditSettingsRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates account settings. |

### Notifications Client

Interface: `INotificationsClient`

Notification feeds, notification management, test notifications, and changelog retrieval.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetNotificationRssAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets the notifications RSS feed URL or content reference. |
| `GetMyNotificationsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Notification>>` | Gets the authenticated user's notifications. |
| `ClearAllNotificationsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse` | Clears all notifications. |
| `ClearNotificationAsync` | `notificationId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Clears a specific notification by ID. |
| `SendTestNotificationAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse` | Sends a test notification. |
| `GetIntercomHashAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IntercomHash>` | Gets the authenticated user's Intercom hash. |
| `GetChangelogsRssAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets the changelog RSS feed URL or content reference. |
| `GetChangelogsJsonAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Changelog>>` | Gets changelog entries as JSON data. |

### RSS Client

Interface: `IRssClient`

RSS feed creation, control, modification, listing, and feed item retrieval.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `AddRssAsync` | `request: AddRssRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Adds a new RSS feed. |
| `ControlRssAsync` | `request: ControlRssRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation to an RSS feed. |
| `ModifyRssAsync` | `request: ModifyRssRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates RSS feed properties. |
| `GetFeedsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<RssFeed>>` | Gets configured RSS feeds. |
| `GetFeedItemsAsync` | `rssFeedId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<RssFeedItem>>` | Gets items for a specific RSS feed. |

### Stream Client

Interface: `IStreamClient`

Stream creation and stream metadata retrieval for downloadable files.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `CreateStreamAsync` | `id: long`<br>`fileId: long`<br>`type: string`<br>`chosenSubtitleIndex: int? = null`<br>`chosenAudioIndex: int? = null`<br>`chosenResolutionIndex: int? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Creates a stream and returns stream URL/data. |
| `GetStreamDataAsync` | `presignedToken: string`<br>`token: string`<br>`chosenSubtitleIndex: int? = null`<br>`chosenAudioIndex: int? = null`<br>`chosenResolutionIndex: int? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets stream metadata or untyped stream data. |

### Integrations Client

Interface: `IIntegrationsClient`

OAuth integration management and third-party job creation for cloud and file-hosting providers.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetOAuthMeAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<OAuthIntegration>>` | Gets connected OAuth integrations. |
| `CreateGoogleDriveJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a Google Drive integration job. |
| `CreateDropboxJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a Dropbox integration job. |
| `CreateOnedriveJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a OneDrive integration job. |
| `CreateGofileJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a GoFile integration job. |
| `CreateOneFichierJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a 1Fichier integration job. |
| `CreatePixeldrainJobAsync` | `request: CreateIntegrationJobRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Creates a Pixeldrain integration job. |
| `GetJobAsync` | `jobId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntegrationJob>` | Gets a specific integration job. |
| `DeleteJobAsync` | `jobId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Deletes an integration job. |
| `GetJobsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<IntegrationJob>>` | Gets all integration jobs for the authenticated user. |
| `GetJobsByHashAsync` | `hash: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<IntegrationJob>>` | Gets integration jobs associated with a specific hash. |
| `OAuthRedirectAsync` | `provider: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets the OAuth redirect URL for a provider. |
| `OAuthCallbackAsync` | `provider: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Handles OAuth callback data for a provider. |
| `OAuthSuccessAsync` | `provider: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets OAuth success data for a provider. |
| `OAuthRegisterAsync` | `provider: string`<br>`request: OAuthRegisterRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Registers an OAuth integration for a provider. |
| `OAuthUnregisterAsync` | `provider: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Unregisters an OAuth integration for a provider. |
| `GetLinkedDiscordRolesAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets linked Discord role data. |

### Vendors Client

Interface: `IVendorsClient`

Vendor registration, vendor account management, and vendor user lifecycle operations.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `RegisterAsync` | `request: RegisterVendorRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<VendorAccount>` | Registers a vendor account. |
| `GetAccountAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<VendorAccount>` | Gets the authenticated vendor account. |
| `UpdateAccountAsync` | `request: UpdateVendorAccountRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<VendorAccount>` | Updates vendor account details. |
| `GetAccountsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<VendorAccount>>` | Gets vendor-managed accounts. |
| `GetAccountByAuthIdAsync` | `userAuthId: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<VendorAccount>` | Gets an account by user auth ID. |
| `RegisterUserAsync` | `request: RegisterVendorUserRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Registers a user under the vendor account. |
| `RemoveUserAsync` | `request: RemoveVendorUserRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Removes a user from the vendor account. |
| `RefreshAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<VendorAccount>` | Refreshes vendor credentials. |

### Queued Client

Interface: `IQueuedClient`

Generic queued-download listing and control operations.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetQueuedAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<QueuedDownload>>` | Gets queued downloads. |
| `ControlQueuedAsync` | `request: ControlQueuedRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control operation to queued downloads. |

## Search API

Interface: `ISearchApiClient`

The Search API covers tutorials, torrent search, Usenet search, metadata search, and Torznab/Newznab-compatible XML search endpoints.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetTorrentSearchTutorialAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets torrent search tutorial content. |
| `SearchTorrentsAsync` | `query: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<TorrentSearchResult>>` | Searches torrent indexers. |
| `GetTorrentByIdAsync` | `id: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<TorrentSearchResult>` | Gets a torrent search result by ID. |
| `GetUsenetSearchTutorialAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets Usenet search tutorial content. |
| `SearchUsenetAsync` | `query: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<UsenetSearchResult>>` | Searches Usenet indexers. |
| `GetUsenetByIdAsync` | `id: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UsenetSearchResult>` | Gets a Usenet search result by ID. |
| `DownloadUsenetAsync` | `id: string`<br>`guid: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Downloads NZB data for a Usenet result. |
| `GetMetaSearchTutorialAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets metadata search tutorial content. |
| `SearchMetaAsync` | `query: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<MetaSearchResult>>` | Searches metadata entries. |
| `GetMetaByIdAsync` | `id: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<MetaSearchResult>` | Gets a metadata search result by ID. |
| `SearchTorznabAsync` | `query: string`<br>`apiKey: string? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Queries the Torznab-compatible endpoint and returns XML. |
| `SearchNewznabAsync` | `query: string`<br>`apiKey: string? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Queries the Newznab-compatible endpoint and returns XML. |

## Relay API

Interface: `IRelayApiClient`

The Relay API provides relay health and torrent inactivity checking.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetStatusAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<RelayStatus>` | Gets relay service status. |
| `CheckForInactiveAsync` | `authId: string`<br>`torrentId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<InactiveCheckResult>` | Checks whether a torrent is inactive on the relay. |

## Models

### TorBoxSDK.Models.Common

#### `TorBoxResponse`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Success` | `bool` | Indicates whether the request succeeded. |
| `Error` | `string?` | API error message, if any. |
| `Detail` | `string?` | Additional response or error detail. |

#### `TorBoxResponse<T>`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Success` | `bool` | Indicates whether the request succeeded. |
| `Error` | `string?` | API error message, if any. |
| `Detail` | `string?` | Additional response or error detail. |
| `Data` | `T?` | Typed payload returned by the API. |

#### `TorBoxException`

Type: `class`

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `TorBoxErrorCode` | Parsed TorBox error code. |
| `Detail` | `string?` | Additional API-provided detail. |

#### `TorBoxErrorCode`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `DownloadStatus`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `DownloadType`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `ControlOperation`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `DownloadFile`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | File identifier. |
| `Md5` | `string?` | MD5 hash, when available. |
| `Mimetype` | `string?` | MIME type, when known. |
| `Name` | `string` | Full file name or path. |
| `S3Path` | `string?` | Backing S3 path, when available. |
| `ShortName` | `string?` | Short display name. |
| `Size` | `long` | File size in bytes. |

### TorBoxSDK.Models.Torrents

#### `Torrent`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Torrent identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `Hash` | `string?` | Torrent info hash. |
| `Name` | `string` | Torrent display name. |
| `Magnet` | `string?` | Magnet URI. |
| `Size` | `long` | Total size in bytes. |
| `Active` | `bool` | Whether the torrent is active. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |
| `ExpiresAt` | `DateTimeOffset?` | Expiration timestamp. |
| `DownloadState` | `string?` | Current download state string. |
| `DownloadSpeed` | `long` | Current download speed in bytes/sec. |
| `UploadSpeed` | `long` | Current upload speed in bytes/sec. |
| `Seeds` | `int` | Connected seed count. |
| `Peers` | `int` | Connected peer count. |
| `Ratio` | `double` | Upload/download ratio. |
| `Progress` | `double` | Progress from `0.0` to `1.0`. |
| `Availability` | `double` | Swarm data availability. |
| `Eta` | `long` | Estimated completion time in seconds. |
| `DownloadFinished` | `bool` | Whether download is complete. |
| `DownloadPresent` | `bool` | Whether downloaded data is present. |
| `TorrentFile` | `bool` | Whether the original torrent file is available. |
| `InactiveCheck` | `long` | Inactive check interval in seconds. |
| `Server` | `int` | Hosting server identifier. |
| `Files` | `IReadOnlyList<DownloadFile>` | Files contained in the torrent. |

#### `TorrentFile`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | File name in the torrent metadata. |
| `Size` | `long` | File size in bytes. |

#### `TorrentInfo`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hash` | `string?` | Torrent info hash. |
| `Name` | `string` | Torrent name. |
| `Size` | `long` | Total size in bytes. |
| `Peers` | `int` | Peer count. |
| `Seeds` | `int` | Seed count. |
| `Files` | `IReadOnlyList<TorrentFile>` | Files declared by the torrent. |
| `Trackers` | `IReadOnlyList<string>` | Associated tracker URLs. |

#### `CreateTorrentRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Magnet` | `string?` | Magnet URI to add. |
| `File` | `byte[]?` | Raw torrent file bytes; sent separately from JSON. |
| `Name` | `string?` | Optional override name. |
| `Seed` | `SeedPreference?` | Preferred seeding behavior. |
| `AllowZip` | `bool?` | Whether zip downloads should be allowed. |
| `AsQueued` | `bool?` | Whether to add as queued instead of starting immediately. |

#### `ControlTorrentRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TorrentId` | `long?` | Target torrent ID. |
| `Operation` | `ControlOperation` | Control operation to apply. |
| `All` | `bool?` | Whether to apply to all torrents. |

#### `RequestDownloadOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TorrentId` | `long` | Torrent ID to download. |
| `FileId` | `long?` | Specific file ID, if only one file should be downloaded. |
| `ZipLink` | `bool?` | Whether to request a zip link. |
| `UserIp` | `string?` | User IP to pass to the API. |
| `Redirect` | `bool?` | Whether the API should redirect directly. |

#### `CheckCachedRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hashes` | `IReadOnlyList<string>` | Torrent hashes to check. |
| `Format` | `string?` | Optional response format. |
| `ListFiles` | `bool?` | Whether to include file listings. |

#### `EditTorrentRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TorrentId` | `long` | Torrent ID to edit. |
| `Name` | `string?` | Updated name. |
| `Tags` | `IReadOnlyList<string>?` | Updated tags. |
| `AlternativeHashes` | `IReadOnlyList<string>?` | Alternative hashes to associate. |

#### `MagnetToFileRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Magnet` | `string` | Magnet URI to convert. |

#### `SeedPreference`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `ExportDataOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TorrentId` | `long` | Torrent ID to export. |
| `ExportType` | `string?` | Export format or type. |

### TorBoxSDK.Models.Usenet

#### `UsenetDownload`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Usenet download identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `Hash` | `string?` | Associated hash. |
| `Name` | `string` | Download name. |
| `Size` | `long` | Total size in bytes. |
| `Active` | `bool` | Whether the download is active. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |
| `ExpiresAt` | `DateTimeOffset?` | Expiration timestamp. |
| `DownloadState` | `string?` | Current download state string. |
| `DownloadSpeed` | `long` | Current download speed in bytes/sec. |
| `UploadSpeed` | `long` | Current upload speed in bytes/sec. |
| `Progress` | `double` | Progress from `0.0` to `1.0`. |
| `Availability` | `double` | Data availability indicator. |
| `Eta` | `long` | Estimated completion time in seconds. |
| `DownloadFinished` | `bool` | Whether the download is complete. |
| `DownloadPresent` | `bool` | Whether data is present on the server. |
| `InactiveCheck` | `long` | Inactive check interval in seconds. |
| `Server` | `int` | Hosting server identifier. |
| `Files` | `IReadOnlyList<DownloadFile>` | Files in the download. |

#### `CreateUsenetDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Link` | `string?` | NZB link to add. |
| `File` | `byte[]?` | Raw NZB file bytes; sent separately from JSON. |
| `Name` | `string?` | Optional override name. |
| `Password` | `string?` | Archive password, if required. |
| `PostProcessing` | `string?` | Post-processing mode. |

#### `ControlUsenetDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UsenetId` | `long?` | Target Usenet download ID. |
| `Operation` | `ControlOperation` | Control operation to apply. |
| `All` | `bool?` | Whether to apply to all Usenet downloads. |

#### `RequestUsenetDownloadOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UsenetId` | `long` | Usenet download ID to download. |
| `FileId` | `long?` | Specific file ID. |
| `ZipLink` | `bool?` | Whether to request a zip link. |
| `UserIp` | `string?` | User IP to pass to the API. |

#### `EditUsenetDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UsenetDownloadId` | `long` | Usenet download ID to edit. |
| `Name` | `string?` | Updated name. |
| `Tags` | `IReadOnlyList<string>?` | Updated tags. |

#### `CheckUsenetCachedRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hashes` | `IReadOnlyList<string>` | Hashes to check. |
| `Format` | `string?` | Optional response format. |
| `ListFiles` | `bool?` | Whether to include file listings. |

### TorBoxSDK.Models.WebDownloads

#### `WebDownload`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Web download identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `Hash` | `string?` | Associated hash. |
| `Name` | `string` | Download name. |
| `Size` | `long` | Total size in bytes. |
| `Active` | `bool` | Whether the download is active. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |
| `ExpiresAt` | `DateTimeOffset?` | Expiration timestamp. |
| `DownloadState` | `string?` | Current download state string. |
| `DownloadSpeed` | `long` | Current download speed in bytes/sec. |
| `UploadSpeed` | `long` | Current upload speed in bytes/sec. |
| `Progress` | `double` | Progress from `0.0` to `1.0`. |
| `Availability` | `double` | Data availability indicator. |
| `Eta` | `long` | Estimated completion time in seconds. |
| `DownloadFinished` | `bool` | Whether the download is complete. |
| `DownloadPresent` | `bool` | Whether data is present on the server. |
| `InactiveCheck` | `long` | Inactive check interval in seconds. |
| `Server` | `int` | Hosting server identifier. |
| `Error` | `string?` | Download error message, if any. |
| `Files` | `IReadOnlyList<DownloadFile>` | Files in the download. |

#### `CreateWebDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Link` | `string?` | Direct URL to download. |
| `Password` | `string?` | Password required by the hoster, if any. |
| `Name` | `string?` | Optional override name. |

#### `ControlWebDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `WebdlId` | `long?` | Target web download ID. |
| `Operation` | `ControlOperation` | Control operation to apply. |
| `All` | `bool?` | Whether to apply to all web downloads. |

#### `RequestWebDownloadOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `WebId` | `long` | Web download ID to download. |
| `FileId` | `long?` | Specific file ID. |
| `ZipLink` | `bool?` | Whether to request a zip link. |
| `UserIp` | `string?` | User IP to pass to the API. |

#### `EditWebDownloadRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `WebdlId` | `long` | Web download ID to edit. |
| `Name` | `string?` | Updated name. |
| `Tags` | `IReadOnlyList<string>?` | Updated tags. |

#### `CheckWebCachedRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hashes` | `IReadOnlyList<string>` | Hashes to check. |
| `Format` | `string?` | Optional response format. |
| `ListFiles` | `bool?` | Whether to include file listings. |

#### `Hoster`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | Hoster name. |
| `DailyBandwidthLimit` | `long?` | Daily bandwidth limit in bytes. |
| `DailyBandwidthUsed` | `long?` | Daily bandwidth already used in bytes. |
| `Status` | `bool` | Whether the hoster is operational. |

### TorBoxSDK.Models.User

#### `UserProfile`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | User identifier. |
| `Email` | `string?` | User email address. |
| `Plan` | `int` | Numeric plan identifier. |
| `TotalDownloaded` | `long` | Total bytes downloaded. |
| `CreatedAt` | `DateTimeOffset?` | Account creation timestamp. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |
| `IsSubscribed` | `bool` | Whether the user has an active subscription. |
| `PremiumExpiresAt` | `DateTimeOffset?` | Premium expiration timestamp. |
| `CooldownUntil` | `DateTimeOffset?` | Cooldown expiration timestamp. |
| `AuthId` | `string?` | User auth identifier. |
| `UserReferralCode` | `string?` | User referral code. |
| `BaseEmail` | `string?` | Base email without aliases. |
| `Settings` | `UserSettings?` | Included user settings, if requested. |

#### `UserSettings`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `SaveMagnetHistory` | `bool?` | Whether magnet history is saved. |
| `DownloadBehavior` | `string?` | Default download behavior. |
| `TorrentSeedPreference` | `int?` | Numeric torrent seed preference. |
| `DefaultTorrentName` | `string?` | Default torrent name. |
| `EnableNotifications` | `bool?` | Whether notifications are enabled. |

#### `Subscription`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Subscription identifier. |
| `PlanName` | `string?` | Subscription plan name. |
| `Amount` | `double` | Subscription amount. |
| `Currency` | `string?` | Currency code. |
| `Status` | `string?` | Subscription status. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `ExpiresAt` | `DateTimeOffset?` | Expiration timestamp. |

#### `Transaction`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Transaction identifier. |
| `Amount` | `double` | Transaction amount. |
| `Currency` | `string?` | Currency code. |
| `Description` | `string?` | Transaction description. |
| `Status` | `string?` | Transaction status. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |

#### `ReferralData`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TotalReferrals` | `int` | Number of referrals. |
| `ReferralCode` | `string?` | Referral code. |

#### `DeviceCodeResponse`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `DeviceCode` | `string?` | Device code used for polling. |
| `UserCode` | `string?` | User-facing verification code. |
| `VerificationUrl` | `string?` | URL where the user completes auth. |
| `ExpiresIn` | `int` | Device code lifetime in seconds. |
| `Interval` | `int` | Polling interval in seconds. |

#### `RefreshTokenRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `SessionToken` | `string` | Session token to refresh. |

#### `DeviceTokenRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `DeviceCode` | `string` | Device code obtained from the device flow. |

#### `DeleteAccountRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Confirmation` | `string?` | Confirmation text required for deletion. |

#### `AddReferralRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `ReferralCode` | `string` | Referral code to apply. |

#### `SearchEngine`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Name` | `string?` | Search engine name. |
| `Enabled` | `bool` | Whether the engine is enabled. |

#### `AddSearchEnginesRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `SearchEngines` | `IReadOnlyList<string>` | Search engines to add. |

#### `ModifySearchEnginesRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `SearchEngines` | `IReadOnlyList<string>` | Search engines to set. |

#### `ControlSearchEnginesRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Operation` | `string?` | Operation to perform. |
| `SearchEngines` | `IReadOnlyList<string>` | Search engines the operation applies to. |

#### `EditSettingsRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `SaveMagnetHistory` | `bool?` | Updated magnet-history setting. |
| `DownloadBehavior` | `string?` | Updated download behavior. |
| `TorrentSeedPreference` | `int?` | Updated numeric seed preference. |
| `DefaultTorrentName` | `string?` | Updated default torrent name. |
| `EnableNotifications` | `bool?` | Updated notifications setting. |

### TorBoxSDK.Models.Notifications

#### `Notification`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Notification identifier. |
| `Title` | `string?` | Notification title. |
| `Message` | `string?` | Notification body. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `Read` | `bool` | Whether the notification has been read. |

#### `Stats`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `ActiveTorrents` | `int` | Number of active torrents. |
| `ActiveUsenet` | `int` | Number of active Usenet downloads. |
| `ActiveWebDownloads` | `int` | Number of active web downloads. |
| `TotalTorrentsDownloaded` | `long` | Total completed torrents. |
| `TotalUsenetDownloaded` | `long` | Total completed Usenet downloads. |
| `TotalWebDownloadsDownloaded` | `long` | Total completed web downloads. |
| `TotalBytesDownloaded` | `long` | Total bytes downloaded. |
| `TotalBytesUploaded` | `long` | Total bytes uploaded. |

#### `IntercomHash`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hash` | `string?` | Intercom identity hash. |

#### `Changelog`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Changelog identifier. |
| `Title` | `string?` | Changelog title. |
| `Body` | `string?` | Changelog body. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |

### TorBoxSDK.Models.Rss

#### `RssFeed`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | RSS feed identifier. |
| `Name` | `string?` | Feed display name. |
| `Url` | `string?` | Feed URL. |
| `ScanInterval` | `int` | Poll interval in minutes. |
| `RssType` | `string?` | Feed type. |
| `Active` | `bool` | Whether the feed is active. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `RegexFilter` | `string?` | Include regex filter. |
| `RegexFilterExclude` | `string?` | Exclude regex filter. |

#### `RssFeedItem`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Feed item identifier. |
| `RssFeedId` | `long` | Parent feed identifier. |
| `Title` | `string?` | Item title. |
| `Link` | `string?` | Item link. |
| `PublishedAt` | `DateTimeOffset?` | Published timestamp. |

#### `AddRssRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Url` | `string` | Feed URL to add. |
| `Name` | `string?` | Optional display name. |
| `RegexFilter` | `string?` | Include regex filter. |
| `RegexFilterExclude` | `string?` | Exclude regex filter. |
| `ScanInterval` | `int?` | Poll interval in minutes. |
| `RssType` | `string?` | Feed type override. |

#### `ControlRssRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `RssFeedId` | `long?` | Target RSS feed ID. |
| `Operation` | `ControlOperation` | Control operation to apply. |
| `All` | `bool?` | Whether to apply to all RSS feeds. |

#### `ModifyRssRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `RssFeedId` | `long` | RSS feed ID to modify. |
| `Name` | `string?` | Updated name. |
| `RegexFilter` | `string?` | Updated include filter. |
| `RegexFilterExclude` | `string?` | Updated exclude filter. |
| `ScanInterval` | `int?` | Updated poll interval in minutes. |

### TorBoxSDK.Models.Integrations

#### `OAuthIntegration`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Provider` | `string?` | OAuth provider name. |
| `Connected` | `bool` | Whether the integration is connected. |
| `CreatedAt` | `DateTimeOffset?` | Connection timestamp. |

#### `IntegrationJob`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Integration job identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `JobType` | `string?` | Integration job type. |
| `Status` | `string?` | Job status. |
| `Progress` | `double` | Progress from `0.0` to `1.0`. |
| `Detail` | `string?` | Additional job detail. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `Hash` | `string?` | Associated content hash. |
| `DownloadId` | `long?` | Associated download ID. |
| `DownloadType` | `string?` | Associated download type. |

#### `CreateIntegrationJobRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `DownloadId` | `long?` | Download ID to export or sync. |
| `DownloadType` | `string?` | Download type string. |
| `FileId` | `long?` | Specific file ID. |
| `Zip` | `bool?` | Whether to zip before integration. |

#### `OAuthRegisterRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Provider` | `string?` | Provider name. |
| `Code` | `string?` | OAuth authorization code. |
| `RedirectUri` | `string?` | Redirect URI used during auth. |

### TorBoxSDK.Models.Vendors

#### `VendorAccount`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Vendor account identifier. |
| `VendorName` | `string?` | Vendor name. |
| `VendorUrl` | `string?` | Vendor website URL. |
| `ApiKey` | `string?` | Vendor API key. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |

#### `RegisterVendorRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `VendorName` | `string` | Vendor name to register. |
| `VendorUrl` | `string?` | Vendor website URL. |

#### `UpdateVendorAccountRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `VendorName` | `string?` | Updated vendor name. |
| `VendorUrl` | `string?` | Updated vendor URL. |

#### `RegisterVendorUserRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UserEmail` | `string` | User email to register. |

#### `RemoveVendorUserRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UserEmail` | `string` | User email to remove. |

### TorBoxSDK.Models.Queued

#### `QueuedDownload`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Queued download identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `Name` | `string?` | Queued item name. |
| `DownloadType` | `string?` | Queued download type string. |
| `Magnet` | `string?` | Magnet URI, when applicable. |
| `Hash` | `string?` | Content hash. |
| `Size` | `long` | Size in bytes. |
| `CreatedAt` | `DateTimeOffset?` | Queue timestamp. |
| `Status` | `string?` | Queue status string. |

#### `ControlQueuedRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `QueuedId` | `long?` | Target queued item ID. |
| `Operation` | `ControlOperation` | Control operation to apply. |
| `All` | `bool?` | Whether to apply to all queued items. |

### TorBoxSDK.Models.Search

#### `TorrentSearchResult`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hash` | `string?` | Torrent info hash. |
| `Name` | `string?` | Result name. |
| `Size` | `long` | Total size in bytes. |
| `Seeders` | `int` | Current seeder count. |
| `Leechers` | `int` | Current leecher count. |
| `Source` | `string?` | Source indexer. |
| `Category` | `string?` | Result category. |
| `Magnet` | `string?` | Magnet URI. |
| `LastKnownSeeders` | `int?` | Last recorded seeder count. |
| `LastKnownLeechers` | `int?` | Last recorded leecher count. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |

#### `UsenetSearchResult`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `string?` | Search result identifier. |
| `Name` | `string?` | Result name. |
| `Size` | `long` | Total size in bytes. |
| `Source` | `string?` | Source indexer. |
| `Category` | `string?` | Result category. |
| `NzbLink` | `string?` | Direct NZB link. |
| `Age` | `int?` | Age in days. |
| `PostedAt` | `DateTimeOffset?` | Posted timestamp. |

#### `MetaSearchResult`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `string?` | Metadata identifier. |
| `Name` | `string?` | Media title. |
| `Type` | `string?` | Media type string. |
| `Year` | `int?` | Release year. |
| `ImdbId` | `string?` | IMDb identifier. |
| `TmdbId` | `long?` | TMDb identifier. |
| `Poster` | `string?` | Poster image URL. |
| `Overview` | `string?` | Media overview or synopsis. |

#### `MediaType`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

#### `SearchType`

Type: `enum`

| Property | Type | Description |
|---|---|---|
| `Values` | — | See [Enums](#enums). |

### TorBoxSDK.Models.Relay

#### `RelayStatus`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Status` | `string?` | Relay status string. |

#### `InactiveCheckResult`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Status` | `string?` | Inactivity check status message. |
| `IsInactive` | `bool` | Whether the torrent is inactive. |
| `LastActive` | `DateTimeOffset?` | Last active timestamp. |

### TorBoxSDK.Models.General

#### `SpeedtestOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UserIp` | `string?` | User IP to use for the speedtest request. |
| `Region` | `string?` | Region override. |
| `TestLength` | `int?` | Test length in seconds. |

## Enums

### `TorBoxErrorCode`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unmapped error code. |
| `DatabaseError` | Database error on the server. |
| `UnknownError` | Unspecified server-side error. |
| `NoAuth` | Missing authentication. |
| `BadToken` | Invalid or expired token. |
| `InvalidOption` | Invalid option or parameter. |
| `PermissionDenied` | Caller lacks permission. |
| `PlanRestrictedFeature` | Feature restricted by plan. |
| `DuplicateItem` | Duplicate item detected. |
| `BreachOfTos` | Terms-of-service violation. |
| `ActiveLimit` | Active download limit reached. |
| `SeedingLimit` | Seeding limit reached. |
| `BannedContentDetected` | Banned content detected. |
| `CouldNotPerformAction` | Requested action could not be completed. |
| `ItemNotFound` | Requested item was not found. |
| `InvalidDevice` | Invalid device identifier or state. |
| `DeviceAlreadyAuthed` | Device already authorized. |
| `TooManyRequests` | Rate limit exceeded. |
| `DownloadTooLarge` | Download exceeds allowed size. |
| `MissingRequiredOption` | Required option is missing. |
| `BannedUser` | User account is banned. |
| `SearchError` | Search operation failed. |
| `ServerError` | Internal server error. |

### `DownloadStatus`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unmapped status. |
| `Downloading` | Currently downloading. |
| `Uploading` | Currently uploading or seeding. |
| `Stalled` | Stalled with no active transfer. |
| `Paused` | Paused by the user. |
| `Completed` | Completed successfully. |
| `Cached` | Cached and immediately available. |
| `Metadl` | Downloading torrent metadata. |
| `Checkingdl` | Verifying downloaded data. |
| `Error` | Download failed. |
| `Queued` | Waiting in queue. |

### `DownloadType`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unmapped download type. |
| `Torrent` | Torrent download. |
| `Usenet` | Usenet download. |
| `WebDownload` | Direct-link or hoster download. |

### `ControlOperation`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unmapped operation. |
| `Reannounce` | Reannounce to trackers or source. |
| `Delete` | Delete the item. |
| `Resume` | Resume the item. |
| `Pause` | Pause the item. |
| `Recheck` | Recheck data integrity. |

### `SeedPreference`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unmapped preference. |
| `Auto` | Use default or automatic behavior. |
| `Seed` | Seed after completion. |
| `NoSeed` | Do not seed after completion. |

### `MediaType`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unspecified media type. |
| `Movie` | Movie content. |
| `Tv` | Television content. |
| `Anime` | Anime content. |
| `Music` | Music content. |
| `Game` | Game content. |
| `Book` | Book content. |
| `Software` | Software content. |
| `Other` | Other or uncategorized content. |

### `SearchType`

| Value | Description |
|---|---|
| `Unknown` | Unknown or unspecified search type. |
| `Torrent` | Torrent search. |
| `Usenet` | Usenet search. |
| `Meta` | Metadata search. |
