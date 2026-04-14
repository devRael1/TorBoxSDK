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

General service endpoints for root status, platform statistics, speedtest file discovery, and changelog retrieval.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetUpStatusAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets Main API status data. |
| `GetStatsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<Stats>` | Gets current aggregate TorBox statistics. |
| `Get30DayStatsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<DailyStats>>` | Gets daily statistics snapshots for the last 30 days. |
| `GetSpeedtestFilesAsync` | `options: SpeedtestOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<SpeedtestServer>>` | Gets speedtest server data using optional region, IP, and test length options. |
| `GetChangelogsRssAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<ChangelogsRssFeed>` | Gets the changelogs as a parsed RSS 2.0 feed. |
| `GetChangelogsJsonAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Changelog>>` | Gets changelog entries as JSON data. |

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
| `GetTransactionPdfAsync` | `transactionId: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets PDF data or a PDF link for a transaction. |
| `AddSearchEnginesAsync` | `request: AddSearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Adds search engines to the user's configuration. |
| `GetSearchEnginesAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<SearchEngine>>` | Gets configured search engines. |
| `ModifySearchEnginesAsync` | `request: ModifySearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Replaces or updates the configured search engine list. |
| `ControlSearchEnginesAsync` | `request: ControlSearchEnginesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Applies a control-style operation to search engine settings. |
| `EditSettingsAsync` | `request: EditSettingsRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Updates account settings. |

### Notifications Client

Interface: `INotificationsClient`

Notification feeds, notification management, and test notifications.

| Method | Parameters | Return type | Description |
|---|---|---|---|
| `GetNotificationRssAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets the notifications RSS feed URL or content reference. |
| `GetMyNotificationsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<Notification>>` | Gets the authenticated user's notifications. |
| `ClearAllNotificationsAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse` | Clears all notifications. |
| `ClearNotificationAsync` | `notificationId: long`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse` | Clears a specific notification by ID. |
| `SendTestNotificationAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse` | Sends a test notification. |
| `GetIntercomHashAsync` | `authId: string`<br>`email: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IntercomHash>` | Gets the authenticated user's Intercom hash. |

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
| `GetOAuthMeAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyDictionary<string, bool>>` | Gets connected OAuth providers keyed by provider name. |
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
| `GetLinkedDiscordRolesAsync` | `request: LinkedRolesRequest`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<object>` | Gets linked Discord role data. |

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
| `SearchTorrentsAsync` | `query: string`<br>`options: TorrentSearchOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<TorrentSearchResponse>` | Searches torrent indexers and returns metadata plus matching torrents. |
| `GetTorrentByIdAsync` | `id: string`<br>`options: TorrentSearchOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<TorrentSearchResult>` | Gets a torrent search result by ID. |
| `GetUsenetSearchTutorialAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets Usenet search tutorial content. |
| `SearchUsenetAsync` | `query: string`<br>`options: UsenetSearchOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UsenetSearchResponse>` | Searches Usenet indexers and returns metadata plus matching NZBs. |
| `GetUsenetByIdAsync` | `id: string`<br>`options: UsenetSearchOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<UsenetSearchResult>` | Gets a Usenet search result by ID. |
| `DownloadUsenetAsync` | `id: string`<br>`guid: string`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Downloads NZB data for a Usenet result. |
| `GetMetaSearchTutorialAsync` | `cancellationToken: CancellationToken = default` | `TorBoxResponse<string>` | Gets metadata search tutorial content. |
| `SearchMetaAsync` | `query: string`<br>`options: MetaSearchOptions? = null`<br>`cancellationToken: CancellationToken = default` | `TorBoxResponse<IReadOnlyList<MetaSearchResult>>` | Searches metadata entries. |
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
| `Hash` | `string?` | Torrent or content hash for the file, when available. |
| `Zipped` | `bool` | Whether the file is provided as a zip payload. |
| `Infected` | `bool` | Whether the file has been flagged as infected. |
| `AbsolutePath` | `string?` | Absolute server-side path, when available. |
| `OpensubtitlesHash` | `string?` | OpenSubtitles hash, when available. |

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
| `DownloadPath` | `string?` | Download path identifier, when available. |
| `Tracker` | `string?` | Tracker URL, when available. |
| `TotalUploaded` | `long` | Total bytes uploaded. |
| `TotalDownloaded` | `long` | Total bytes downloaded. |
| `Cached` | `bool` | Whether the torrent is cached on the server. |
| `Owner` | `string?` | Owner UUID, when available. |
| `SeedTorrent` | `bool` | Whether the torrent should seed after completion. |
| `AllowZipped` | `bool` | Whether zipped downloads are allowed. |
| `LongTermSeeding` | `bool` | Whether long-term seeding is enabled. |
| `TrackerMessage` | `string?` | Tracker status message, when available. |
| `CachedAt` | `DateTimeOffset?` | Timestamp when the torrent was cached. |
| `IsPrivate` | `bool` | Whether the torrent uses a private tracker. |
| `AlternativeHashes` | `IReadOnlyList<string>` | Alternative hashes associated with the torrent. |
| `Tags` | `IReadOnlyList<string>` | Tags associated with the torrent. |

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
| `AddOnlyIfCached` | `bool?` | Whether to only add if cached. |

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
| `PostProcessing` | `int?` | Post-processing mode flag (-1 = default). |
| `AsQueued` | `bool?` | Whether to add as queued. |
| `AddOnlyIfCached` | `bool?` | Whether to only add if cached. |

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
| `AsQueued` | `bool?` | Whether to add as queued. |
| `AddOnlyIfCached` | `bool?` | Whether to only add if cached. |

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
| `Id` | `int` | Hoster identifier. |
| `Name` | `string` | Hoster name. |
| `Domains` | `IReadOnlyList<string>` | Domains handled by the hoster. |
| `Url` | `string?` | Hoster website URL. |
| `Icon` | `string?` | Hoster icon URL. |
| `DailyBandwidthLimit` | `long?` | Daily bandwidth limit in bytes. |
| `DailyBandwidthUsed` | `long?` | Daily bandwidth already used in bytes. |
| `DailyLinkLimit` | `int` | Daily link limit. |
| `DailyLinkUsed` | `int` | Daily links already used. |
| `PerLinkSizeLimit` | `long?` | Per-link size limit in bytes. |
| `Status` | `bool` | Whether the hoster is operational. |
| `Type` | `string?` | Hoster type string. |
| `Note` | `string?` | Hoster note, when provided. |
| `Nsfw` | `bool` | Whether the hoster is marked NSFW. |
| `Regex` | `string?` | URL-matching regex, when available. |

### TorBoxSDK.Models.User

#### `UserProfile`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | User identifier. |
| `AuthId` | `string?` | User auth identifier. |
| `CreatedAt` | `DateTimeOffset?` | Account creation timestamp. |
| `UpdatedAt` | `DateTimeOffset?` | Last update timestamp. |
| `Plan` | `int` | Numeric plan identifier. |
| `TotalDownloaded` | `long` | Total bytes downloaded. |
| `Customer` | `string?` | Payment-provider customer identifier. |
| `IsSubscribed` | `bool` | Whether the user has an active subscription. |
| `PremiumExpiresAt` | `DateTimeOffset?` | Premium expiration timestamp. |
| `CooldownUntil` | `DateTimeOffset?` | Cooldown expiration timestamp. |
| `Email` | `string?` | User email address. |
| `UserReferral` | `string?` | User referral code or identifier. |
| `BaseEmail` | `string?` | Base email without aliases. |
| `TotalBytesDownloaded` | `long` | Total bytes downloaded by the user. |
| `TotalBytesUploaded` | `long` | Total bytes uploaded by the user. |
| `TorrentsDownloaded` | `long` | Number of torrent downloads completed. |
| `WebDownloadsDownloaded` | `long` | Number of web downloads completed. |
| `UsenetDownloadsDownloaded` | `long` | Number of Usenet downloads completed. |
| `AdditionalConcurrentSlots` | `int` | Additional concurrent download slots. |
| `LongTermSeeding` | `bool` | Whether long-term seeding is enabled. |
| `LongTermStorage` | `bool` | Whether long-term storage is enabled. |
| `IsVendor` | `bool` | Whether the user is a vendor. |
| `VendorId` | `string?` | Vendor identifier, when applicable. |
| `PurchasesReferred` | `int` | Number of purchases referred by the user. |
| `Settings` | `UserSettings?` | Included user settings, if requested. |

#### `UserSettings`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `EmailNotifications` | `bool?` | Whether email notifications are enabled. |
| `WebNotifications` | `bool?` | Whether web notifications are enabled. |
| `MobileNotifications` | `bool?` | Whether mobile notifications are enabled. |
| `RssNotifications` | `bool?` | Whether RSS notifications are enabled. |
| `DiscordNotifications` | `bool?` | Whether Discord notifications are enabled. |
| `JdownloaderNotifications` | `bool?` | Whether JDownloader notifications are enabled. |
| `WebhookNotifications` | `bool?` | Whether webhook notifications are enabled. |
| `TelegramNotifications` | `bool?` | Whether Telegram notifications are enabled. |
| `WebhookUrl` | `string?` | Webhook URL, when configured. |
| `TelegramId` | `string?` | Telegram identifier, when configured. |
| `DiscordId` | `string?` | Discord identifier, when configured. |
| `StremioQuality` | `IReadOnlyList<int>?` | Preferred Stremio quality filters. |
| `StremioResolution` | `IReadOnlyList<int>?` | Preferred Stremio resolution filters. |
| `StremioLanguage` | `IReadOnlyList<int>?` | Preferred Stremio language filters. |
| `StremioCache` | `IReadOnlyList<int>?` | Preferred Stremio cache filters. |
| `StremioSizeLower` | `long?` | Minimum Stremio size filter in bytes. |
| `StremioSizeUpper` | `long?` | Maximum Stremio size filter in bytes. |
| `StremioAllowAdult` | `bool?` | Whether adult content is allowed in Stremio. |
| `StremioSeedTorrents` | `int?` | Stremio torrent seeding preference. |
| `StremioSort` | `string?` | Stremio sort preference. |
| `StremioUseCustomSearchEngines` | `bool?` | Whether Stremio uses custom search engines. |
| `StremioResultSort` | `string?` | Stremio result sort preference. |
| `StremioLegacyYourMedia` | `bool?` | Whether legacy "your media" behavior is enabled. |
| `StremioOnlyYourMediaStreams` | `bool?` | Whether only "your media" streams are shown. |
| `StremioDisableYourMediaStreams` | `bool?` | Whether "your media" streams are disabled. |
| `StremioLimitPerResolutionTorrent` | `int?` | Torrent result limit per resolution in Stremio. |
| `StremioLimitPerResolutionUsenet` | `int?` | Usenet result limit per resolution in Stremio. |
| `StremioTorrentSeedersCutoff` | `int?` | Seeder cutoff used by Stremio torrent results. |
| `StremioWaitForDownloadUsenet` | `bool?` | Whether Stremio waits for Usenet downloads. |
| `StremioWaitForDownloadTorrent` | `bool?` | Whether Stremio waits for torrent downloads. |
| `StremioDisableFilteredNote` | `bool?` | Whether the filtered-note banner is disabled. |
| `StremioEmojiInDescription` | `bool?` | Whether emoji is shown in Stremio descriptions. |
| `StremioAllowZipped` | `bool?` | Whether Stremio allows zipped downloads. |
| `SeedTorrents` | `int?` | Torrent seeding preference. |
| `AllowZipped` | `bool?` | Whether zipped downloads are allowed. |
| `CdnSelection` | `string?` | Preferred CDN selection. |
| `GoogleDriveFolderId` | `string?` | Google Drive folder ID. |
| `OnedriveSavePath` | `string?` | OneDrive save path. |
| `OnefichierFolderId` | `string?` | 1Fichier folder ID. |
| `GofileFolderId` | `string?` | GoFile folder ID. |
| `PixeldrainApiKey` | `string?` | Pixeldrain API key. |
| `OnefichierApiKey` | `string?` | 1Fichier API key. |
| `GofileApiKey` | `string?` | GoFile API key. |
| `MegaEmail` | `string?` | MEGA email address. |
| `MegaPassword` | `string?` | MEGA password. |
| `PatreonId` | `string?` | Patreon identifier. |
| `DownloadSpeedInTab` | `bool?` | Whether download speed is shown in the browser tab. |
| `ShowTrackerInTorrents` | `bool?` | Whether tracker information is shown in torrent details. |
| `DashboardFilter` | `DashboardFilter?` | Saved dashboard filter configuration. |
| `WebdavUseLocalFiles` | `bool?` | Whether WebDAV uses local files. |
| `WebdavUseFolderView` | `bool?` | Whether WebDAV uses folder view. |
| `WebdavFlatten` | `bool?` | Whether WebDAV paths are flattened. |

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
| `At` | `DateTimeOffset?` | Transaction timestamp. |
| `Type` | `string?` | Payment-provider or transaction type. |
| `Amount` | `double` | Transaction amount. |
| `TransactionId` | `string?` | External transaction identifier. |

#### `ReferralData`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `ReferredAccounts` | `int` | Number of referred accounts. |
| `ReferralCode` | `string?` | Referral code. |
| `PurchasesReferred` | `int` | Number of purchases made by referred accounts. |

#### `DeviceCodeResponse`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `DeviceCode` | `string?` | Device code used for polling. |
| `Code` | `string?` | User-facing verification code. |
| `VerificationUrl` | `string?` | URL where the user completes auth. |
| `FriendlyVerificationUrl` | `string?` | Short verification URL, when available. |
| `ExpiresAt` | `DateTimeOffset?` | Device code expiration timestamp. |
| `Interval` | `int` | Polling interval in seconds. |

#### `DashboardFilter`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Active` | `bool?` | Whether active items are shown. |
| `Cached` | `bool?` | Whether cached items are shown. |
| `Queued` | `bool?` | Whether queued items are shown. |
| `Private` | `bool?` | Whether private items are shown. |
| `Borrowed` | `bool?` | Whether borrowed items are shown. |

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
| `SessionToken` | `string` | Session token from the website. |
| `ConfirmationCode` | `int` | Confirmation code sent to email. |

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
| `Type` | `string?` | Search engine type (e.g., "prowlarr", "jackett"). |
| `Url` | `string?` | Search engine URL. |
| `Apikey` | `string?` | API key. |
| `DownloadType` | `string?` | Download type ("torrents" or "usenet"). |

#### `ModifySearchEnginesRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Search engine ID. |
| `Type` | `string?` | Search engine type. |
| `Url` | `string?` | Search engine URL. |
| `Apikey` | `string?` | API key. |
| `DownloadType` | `string?` | Download type. |

#### `ControlSearchEnginesRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Operation` | `string` | Operation to perform ("delete", "enable", "disable", "check"). |
| `Id` | `long?` | Search engine ID. |
| `All` | `bool?` | Whether to apply to all. |

#### `EditSettingsRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `DashboardFilter` | `DashboardFilter?` | Saved dashboard filter configuration. |

### TorBoxSDK.Models.Notifications

#### `Notification`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `long` | Notification identifier. |
| `AuthId` | `string?` | Owning user auth identifier. |
| `Title` | `string?` | Notification title. |
| `Message` | `string?` | Notification body. |
| `CreatedAt` | `DateTimeOffset?` | Creation timestamp. |
| `Action` | `string?` | Action type associated with the notification. |
| `ActionData` | `string?` | Action payload, such as a URL. |
| `ActionCta` | `string?` | Call-to-action label, when available. |

#### `IntercomHash`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Hash` | `string?` | Intercom identity hash. |

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
| `Name` | `string?` | New name. |
| `DoRegex` | `string?` | Include regex filter. |
| `DontRegex` | `string?` | Exclude regex filter. |
| `ScanInterval` | `int?` | Scan interval in minutes. |
| `DontOlderThan` | `int?` | Max age in days. |
| `RssType` | `string?` | RSS type. |
| `TorrentSeeding` | `int?` | Seeding preference. |

### TorBoxSDK.Models.Integrations

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
| `DownloadId` | `long?` | Download identifier (serialized as `id`). |
| `DownloadType` | `string?` | Download type string (serialized as `type`). |
| `FileId` | `long?` | Specific file ID. |
| `Zip` | `bool?` | Whether to zip before integration. |

#### `OAuthRegisterRequest`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Token` | `string` | OAuth access token. |
| `RefreshToken` | `string` | OAuth refresh token. |

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
| `CreatedAt` | `DateTimeOffset?` | Queue timestamp. |
| `Magnet` | `string?` | Magnet URI, when applicable. |
| `TorrentFile` | `string?` | Torrent-file path, when applicable. |
| `Hash` | `string?` | Content hash. |
| `Name` | `string?` | Queued item name. |
| `Type` | `string?` | Queued download type string. |
| `NameOverride` | `string?` | Name override applied to the queued item. |
| `SeedTorrentOverride` | `int?` | Seed-torrent override value. |

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
| `Owned` | `bool?` | Whether the torrent is already owned by the user. |
| `Cached` | `bool?` | Whether the torrent is cached. |
| `Website` | `string?` | Website URL of the source indexer. |
| `Id` | `string?` | Result identifier. |
| `Quality` | `string?` | Quality label, when available. |
| `RawTitle` | `string?` | Raw unparsed title from the indexer. |

#### `TorrentSearchResponse`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Metadata` | `MetaSearchResult?` | Metadata associated with the search query. |
| `Torrents` | `IReadOnlyList<TorrentSearchResult>` | Matching torrent results. |

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
| `GlobalId` | `string?` | Global metadata identifier. |
| `Id` | `string?` | Metadata identifier. |
| `ImdbId` | `string?` | IMDb identifier. |
| `TmdbId` | `long?` | TMDb identifier. |
| `TvdbId` | `long?` | TVDb identifier. |
| `TvmazeId` | `long?` | TVmaze identifier. |
| `TraktId` | `long?` | Trakt identifier. |
| `MalId` | `long?` | MyAnimeList identifier. |
| `AnilistId` | `long?` | AniList identifier. |
| `KitsuId` | `long?` | Kitsu identifier. |
| `SimklId` | `long?` | Simkl identifier. |
| `Title` | `string?` | Media title. |
| `Titles` | `IReadOnlyList<string>` | Alternate titles. |
| `Keywords` | `IReadOnlyList<string>` | Keywords associated with the media item. |
| `Genres` | `IReadOnlyList<string>` | Media genres. |
| `Actors` | `IReadOnlyList<string>` | Featured actors. |
| `ReleaseYears` | `int?` | Release year. |
| `MediaType` | `string?` | Media type string. |
| `Characters` | `IReadOnlyList<string>` | Character names. |
| `Link` | `string?` | External metadata link. |
| `Description` | `string?` | Media overview or synopsis. |
| `Rating` | `double?` | Rating value, when available. |
| `Languages` | `IReadOnlyList<string>` | Available languages. |
| `ContentRating` | `string?` | Content rating, when available. |
| `Trailer` | `MetaTrailer?` | Trailer metadata, when available. |
| `Image` | `string?` | Poster or cover image URL. |

#### `MetaTrailer`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `YoutubeId` | `string?` | YouTube video identifier. |
| `FullUrl` | `string?` | Full trailer URL. |
| `Thumbnail` | `string?` | Trailer thumbnail URL. |

#### `UsenetSearchResponse`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Metadata` | `MetaSearchResult?` | Metadata associated with the search query. |
| `Nzbs` | `IReadOnlyList<UsenetSearchResult>` | Matching Usenet results. |

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
| `Data` | `RelayData?` | Relay worker and connection metrics. |

#### `RelayData`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `CurrentOnline` | `int` | Number of currently online relay connections. |
| `CurrentWorkers` | `int` | Number of currently active relay workers. |

#### `InactiveCheckResult`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Status` | `string?` | Inactivity check status message. |
| `IsInactive` | `bool` | Whether the torrent is inactive. |
| `LastActive` | `DateTimeOffset?` | Last active timestamp. |

### TorBoxSDK.Models.General

#### `Stats`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TotalUsers` | `long` | Total registered users on the platform. |
| `TotalServers` | `int` | Total available servers. |

#### `Changelog`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Unique identifier of the changelog entry. |
| `Name` | `string` | Changelog version or entry name. |
| `Html` | `string?` | HTML content of the changelog entry. |
| `Markdown` | `string?` | Markdown content of the changelog entry. |
| `Link` | `string?` | Link associated with the changelog entry. |
| `CreatedAt` | `DateTimeOffset?` | Creation date of the changelog entry. |

#### `DailyStats`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `TotalDownloads` | `long` | Total downloads for the snapshot. |
| `TotalUsers` | `long` | Total users for the snapshot. |
| `TotalBytesDownloaded` | `long` | Total bytes downloaded for the snapshot. |
| `TotalBytesUploaded` | `long` | Total bytes uploaded for the snapshot. |
| `TotalBytesEgressed` | `long` | Total bytes egressed for the snapshot. |
| `TotalServers` | `int` | Total servers for the snapshot. |
| `CreatedAt` | `DateTimeOffset?` | Snapshot timestamp. |

#### `SpeedtestServer`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Region` | `string?` | Server region identifier. |
| `Name` | `string?` | Server display name. |
| `Domain` | `string?` | Server base domain. |
| `Path` | `string?` | Relative path to the test file. |
| `Url` | `string?` | Full speedtest URL. |
| `Closest` | `bool` | Whether this is the closest server. |
| `Coordinates` | `ServerCoordinates?` | Geographic coordinates, when available. |

#### `ServerCoordinates`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Lat` | `double` | Latitude. |
| `Lng` | `double` | Longitude. |

#### `SpeedtestOptions`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `UserIp` | `string?` | User IP to use for the speedtest request. |
| `Region` | `string?` | Region override. |
| `TestLength` | `string?` | Test length override. |

#### `ChangelogsRssFeed`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Version` | `string` | RSS version (e.g. "2.0"). |
| `Channel` | `ChangelogsRssChannel?` | Channel metadata and items. |

#### `ChangelogsRssChannel`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Title` | `string` | Channel title. |
| `Link` | `string` | Channel link. |
| `Description` | `string` | Channel description. |
| `Language` | `string` | Channel language. |
| `LastBuildDate` | `string` | Last build date as a string. |
| `Items` | `IReadOnlyList<ChangelogsRssItem>` | List of RSS items. |

#### `ChangelogsRssItem`

Type: `record`

| Property | Type | Description |
|---|---|---|
| `Title` | `string` | Item title. |
| `Link` | `string` | Item link. |
| `Description` | `string` | Item description. |
| `PubDate` | `string` | Publication date as a string. |
| `ContentEncoded` | `string?` | Content:encoded element value. |

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
