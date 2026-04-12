# Plan: TorBox SDK Corrections and Enhancements

## TL;DR
A complete parameter-by-parameter analysis of the 4 Postman collections (Main, Search, Relay, Vendors) reveals **zero missing endpoints** but **many missing parameters**, **undersized models**, **an organizational discrepancy**, **duplicates**, and **missing query params across 3 clients**. This plan addresses everything in 5 phases.

---

## Phase 1 — Missing Parameters on Existing Models (critical priority)

### Step 1.1 — `CreateTorrentRequest`: add `add_only_if_cached`
- **Gap**: Postman `Create Torrent` and `Async Create Torrent` have the `add_only_if_cached` field (bool). The SDK `CreateTorrentRequest` does not have it.
- **File**: `src/TorBoxSDK/Models/Torrents/CreateTorrentRequest.cs`
  - Add property `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.2 — `RequestDownloadOptions` (Torrents): add `token` and `append_name`
- **Gap**: Postman `Request Download Link` (Torrents) sends `token`, `redirect`, `append_name` as query params. The SDK has `redirect` but is missing `token` and `append_name`.
- **File**: `src/TorBoxSDK/Models/Torrents/RequestDownloadOptions.cs`
  - Add `Token` (string?, JSON: `"token"`)
  - Add `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.3 — `RequestUsenetDownloadOptions`: add `token`, `redirect`, `append_name`
- **Gap**: Postman Usenet `Request Download Link` has `token`, `redirect`, `append_name`. The SDK has none of them.
- **File**: `src/TorBoxSDK/Models/Usenet/RequestUsenetDownloadOptions.cs`
  - Add `Token` (string?, JSON: `"token"`)
  - Add `Redirect` (bool?, JSON: `"redirect"`)
  - Add `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.4 — `RequestWebDownloadOptions`: add `token`, `redirect`, `append_name`
- **Gap**: Postman Web Downloads `Request Download Link` has `token`, `redirect`, `append_name`. The SDK has none of the three.
- **File**: `src/TorBoxSDK/Models/WebDownloads/RequestWebDownloadOptions.cs`
  - Add `Token` (string?, JSON: `"token"`)
  - Add `Redirect` (bool?, JSON: `"redirect"`)
  - Add `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.5 — `CreateUsenetDownloadRequest`: add `as_queued` and `add_only_if_cached`
- **Gap**: Postman `Create Usenet Download` has `as_queued` and `add_only_if_cached`. The SDK has neither.
- **File**: `src/TorBoxSDK/Models/Usenet/CreateUsenetDownloadRequest.cs`
  - Add `AsQueued` (bool?, JSON: `"as_queued"`)
  - Add `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.6 — `CreateWebDownloadRequest`: add `as_queued` and `add_only_if_cached`
- **Gap**: Postman `Create Web Download` has `as_queued` and `add_only_if_cached`. The SDK has neither.
- **File**: `src/TorBoxSDK/Models/WebDownloads/CreateWebDownloadRequest.cs`
  - Add `AsQueued` (bool?, JSON: `"as_queued"`)
  - Add `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.7 — `EditUsenetDownloadRequest`: add `alternative_hashes`
- **Gap**: Postman `Edit Usenet Item` sends `{ usenet_download_id, name, tags, alternative_hashes }`. The SDK does not have `alternative_hashes`.
- **File**: `src/TorBoxSDK/Models/Usenet/EditUsenetDownloadRequest.cs`
  - Add `AlternativeHashes` (IReadOnlyList<string>?, JSON: `"alternative_hashes"`)

### Step 1.8 — `EditWebDownloadRequest`: add `alternative_hashes`
- **Gap**: Postman `Edit Web Download Item` sends `{ webdl_id, name, tags, alternative_hashes }`. The SDK does not have `alternative_hashes`.
- **File**: `src/TorBoxSDK/Models/WebDownloads/EditWebDownloadRequest.cs`
  - Add `AlternativeHashes` (IReadOnlyList<string>?, JSON: `"alternative_hashes"`)

### Step 1.9 — `GetTorrentInfoAsync` (GET): add `use_cache_lookup`
- **Gap**: Postman has the `use_cache_lookup` param (bool). The SDK only has `hash` and `timeout`.
- **File**: `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface
  - Add parameter `bool? useCacheLookup = null` to the signature
  - Add `use_cache_lookup` to the query string

### Step 1.10 — `GetTorrentInfoByFileAsync` (POST): refactor into `TorrentInfoRequest`
- **Gap**: Postman POST `/torrents/torrentinfo` accepts 6 form-data fields: `magnet`, `file`, `hash`, `timeout`, `use_cache_lookup`, `peers_only`. The SDK only accepts `byte[] file`.
- **New file**: `src/TorBoxSDK/Models/Torrents/TorrentInfoRequest.cs`
  - `File` (byte[]?, multipart)
  - `Magnet` (string?, form field)
  - `Hash` (string?, form field)
  - `Timeout` (int?, form field)
  - `UseCacheLookup` (bool?, form field `"use_cache_lookup"`)
  - `PeersOnly` (bool?, form field `"peers_only"`)
- **File**: `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface — replace `byte[] file` with `TorrentInfoRequest request`

---

## Phase 2 — Missing Parameters on Clients (query params / signatures)

### Step 2.1 — `StreamClient`: add stream parameters
- **CRITICAL Gap**: Postman `Create Stream` has 6 query params: `id`, `file_id`, `type`, `chosen_subtitle_index`, `chosen_audio_index`, `chosen_resolution_index`. The SDK only has 3.
- **File**: `src/TorBoxSDK/Main/Stream/StreamClient.cs` + interface
  - `CreateStreamAsync`: add `int? chosenSubtitleIndex`, `int? chosenAudioIndex`, `int? chosenResolutionIndex`
- **CRITICAL Gap**: Postman `Get Stream Data` has 5 query params completely different from the SDK: `presigned_token`, `token`, `chosen_subtitle_index`, `chosen_audio_index`, `chosen_resolution_index`. The SDK sends `id`, `file_id`, `type` which are the Create params, not GetStreamData params.
- **File**: `src/TorBoxSDK/Main/Stream/StreamClient.cs` + interface
  - `GetStreamDataAsync`: replace the params with `string presignedToken`, `string token`, `int? chosenSubtitleIndex`, `int? chosenAudioIndex`, `int? chosenResolutionIndex`

### Step 2.2 — `QueuedClient.GetQueuedAsync`: add query params
- **Gap**: Postman `Get Queued Downloads` accepts 5 query params: `bypass_cache`, `id`, `offset`, `limit`, `type`. The SDK has no parameters.
- **File**: `src/TorBoxSDK/Main/Queued/QueuedClient.cs` + interface
  - `GetQueuedAsync(long? id, int? offset, int? limit, bool? bypassCache, string? type, CancellationToken)`

### Step 2.3 — Create Options models for the Search API
- **Gap**: The 4 main Search API endpoints accept no query params in the SDK, while Postman documents 7: `metadata`, `season`, `episode`, `check_cache`, `check_owned`, `search_user_engines`, `cached_only`.
- **New file**: `src/TorBoxSDK/Models/Search/TorrentSearchOptions.cs`
  - `Metadata` (bool?), `Season` (int?), `Episode` (int?), `CheckCache` (bool?), `CheckOwned` (bool?), `SearchUserEngines` (bool?), `CachedOnly` (bool?)
- **New file**: `src/TorBoxSDK/Models/Search/UsenetSearchOptions.cs` — same properties
- **New file**: `src/TorBoxSDK/Models/Search/MetaSearchOptions.cs` — `Type` (string?)

### Step 2.4 — Update `SearchApiClient` to use Options (*depends on 2.3*)
- **File**: `src/TorBoxSDK/Search/SearchApiClient.cs` + interface
  - `SearchTorrentsAsync(string query, TorrentSearchOptions? options, CancellationToken)`
  - `GetTorrentByIdAsync(string id, TorrentSearchOptions? options, CancellationToken)`
  - `SearchUsenetAsync(string query, UsenetSearchOptions? options, CancellationToken)`
  - `GetUsenetByIdAsync(string id, UsenetSearchOptions? options, CancellationToken)`
  - `SearchMetaAsync(string query, MetaSearchOptions? options, CancellationToken)`

### Step 2.5 — `UserClient`: fix Search Engine models (incorrect signatures)
- **CRITICAL Gap**: The 3 Search Engine methods in the SDK have completely wrong Request models vs Postman:
  - **`AddSearchEnginesRequest`**: SDK sends `{ "search_engines": [...] }`. Postman sends `{ "type", "url", "apikey", "download_type" }`.
  - **`ModifySearchEnginesRequest`**: SDK sends `{ "search_engines": [...] }`. Postman sends `{ "id", "type", "url", "apikey", "download_type" }`.
  - **`ControlSearchEnginesRequest`**: SDK sends `{ "operation", "search_engines": [...] }`. Postman sends `{ "operation", "id", "all" }`.
- **Model files**:
  - `src/TorBoxSDK/Models/User/AddSearchEnginesRequest.cs` — overhaul: properties `Type` (string?), `Url` (string?), `Apikey` (string?), `DownloadType` (string?, JSON: `"download_type"`)
  - `src/TorBoxSDK/Models/User/ModifySearchEnginesRequest.cs` — overhaul: properties `Id` (long), `Type` (string?), `Url` (string?), `Apikey` (string?), `DownloadType` (string?, JSON: `"download_type"`)
  - `src/TorBoxSDK/Models/User/ControlSearchEnginesRequest.cs` — overhaul: properties `Operation` (string), `Id` (long?), `All` (bool?)

### Step 2.6 — `UserClient`: fix missing/incorrect User params
- **`AddReferralAsync`**: SDK sends a JSON body `{ "referral_code" }`. Postman uses a **query param** `?referral=`. Incorrect sending method.
  - **File**: `src/TorBoxSDK/Main/User/UserClient.cs` — change to send `referral` as a query string instead of JSON body
  - **File**: `src/TorBoxSDK/Models/User/AddReferralRequest.cs` — can be removed or simplified to a plain string param
- **`StartDeviceAuthAsync`**: Postman has query param `app` (string, app name). The SDK does not have it.
  - **File**: `src/TorBoxSDK/Main/User/UserClient.cs` + interface — add `string? app = null`
- **`GetSearchEnginesAsync`**: Postman has query param `id` (long?, optional) to retrieve a specific search engine. The SDK does not have it.
  - **File**: `src/TorBoxSDK/Main/User/UserClient.cs` + interface — add `long? id = null`
- **`GetTransactionPdfAsync`**: Postman uses `?transaction_id` as a query param. The SDK uses `{transactionId}` in the path. Route is potentially incorrect.
  - **File**: `src/TorBoxSDK/Main/User/UserClient.cs` — verify and switch to query param if the path route does not work

### Step 2.7 — `VendorsClient`: fix content-type and param name
- **`RegisterAsync`** and **`UpdateAccountAsync`**: Postman sends as **form-data** (`vendor_name`, `vendor_url`). The SDK sends as JSON. Content-type mismatch.
  - **File**: `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — change `TorBoxApiHelper.JsonContent()` to multipart form-data for these 2 methods
- **`GetAccountByEmailAsync`**: Postman uses the query param `user_auth_id`. The SDK uses `user_email`. Param name is potentially incorrect.
  - **File**: `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — rename the param to `userAuthId` and the query param to `user_auth_id`
  - **File**: `src/TorBoxSDK/Main/Vendors/IVendorsClient.cs` — rename the method to `GetAccountByAuthIdAsync(string userAuthId, ...)` (breaking change)
- **`RegisterUserAsync`**: Postman sends as **form-data** `user_email`. The SDK sends as JSON. Content-type mismatch.
  - **File**: `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — change to multipart form-data

---

## Phase 3 — Undersized Models (enrichment)

### Step 3.1 — `EditSettingsRequest`: major enrichment
- **MAJOR Gap**: Postman `Edit User Settings` has ~60+ properties. The SDK only has 5 (`save_magnet_history`, `download_behavior`, `torrent_seed_preference`, `default_torrent_name`, `enable_notifications`).
- **File**: `src/TorBoxSDK/Models/User/EditSettingsRequest.cs` — add ALL properties from Postman:
  - Notifications: `email_notifications`, `web_notifications`, `mobile_notifications`, `rss_notifications`, `discord_notifications`, `jdownloader_notifications`, `webhook_notifications`, `telegram_notifications`
  - Webhook/Telegram: `webhook_url`, `telegram_id`, `discord_id`
  - Stremio: `stremio_quality` (int[]?), `stremio_resolution` (int[]?), `stremio_language` (int[]?), `stremio_cache` (int[]?), `stremio_size_lower` (long?), `stremio_size_upper` (long?), `stremio_allow_adult` (bool?), `stremio_seed_torrents` (int?), `stremio_sort` (string?), `stremio_use_custom_search_engines` (bool?), `stremio_result_sort` (string?), `stremio_legacy_your_media` (bool?), `stremio_only_your_media_streams` (bool?), `stremio_disable_your_media_streams` (bool?), `stremio_limit_per_resolution_torrent` (int?), `stremio_limit_per_resolution_usenet` (int?), `stremio_torrent_seeders_cutoff` (int?), `stremio_wait_for_download_usenet` (bool?), `stremio_wait_for_download_torrent` (bool?), `stremio_disable_filtered_note` (bool?), `stremio_emoji_in_description` (bool?)
  - Downloads: `seed_torrents` (int?), `allow_zipped` (bool?), `stremio_allow_zipped` (bool?), `cdn_selection` (string?), `append_filename_to_links` (bool?)
  - Cloud storage: `google_drive_folder_id` (string?), `onedrive_save_path` (string?), `onefichier_folder_id` (string?), `gofile_folder_id` (string?), `pixeldrain_api_key` (string?), `onefichier_api_key` (string?), `gofile_api_key` (string?), `mega_email` (string?), `mega_password` (string?)
  - UI: `download_speed_in_tab` (bool?), `show_tracker_in_torrents` (bool?), `dashboard_filter` (object?), `dashboard_sort` (string?), `patreon_id` (string?)
  - Web Player: `web_player_always_transcode` (bool?), `web_player_always_skip_intro` (bool?), `web_player_audio_preferred_language` (string?), `web_player_subtitle_preferred_language` (string?), `web_player_disable_prestream_selector` (bool?), `web_player_disable_next_up_dialogue` (bool?), `web_player_enable_scrobbling` (bool?)
  - WebDAV: `webdav_use_local_files` (bool?), `webdav_use_folder_view` (bool?), `webdav_flatten` (bool?)

### Step 3.2 — `AddRssRequest`: add missing fields
- **Gap**: Postman `Add RSS Feed` has these fields beyond the SDK: `do_regex` (not `regex_filter`), `dont_regex` (not `regex_filter_exclude`), `dont_older_than`, `pass_check`, `torrent_seeding`.
- **Verification needed**: Confirm that the JSON names in the SDK (`regex_filter` / `regex_filter_exclude`) match the actual API. Postman uses `do_regex` / `dont_regex`. If the API accepts both, no action needed. Otherwise:
  - **File**: `src/TorBoxSDK/Models/Rss/AddRssRequest.cs`
    - Rename `RegexFilter` JSON name from `"regex_filter"` to `"do_regex"`
    - Rename `RegexFilterExclude` JSON name from `"regex_filter_exclude"` to `"dont_regex"`
    - Add `DontOlderThan` (int?, JSON: `"dont_older_than"`)
    - Add `PassCheck` (bool?, JSON: `"pass_check"`)
    - Add `TorrentSeeding` (int?, JSON: `"torrent_seeding"`)

---

## Phase 4 — Organizational Corrections

### Step 4.1 — Move Changelogs from NotificationsClient to GeneralClient
- **Gap**: Postman classifies them under `General`, while the SDK places them in `NotificationsClient`.
- **File**: `src/TorBoxSDK/Main/Notifications/NotificationsClient.cs` — remove `GetChangelogsRssAsync` and `GetChangelogsJsonAsync`
- **File**: `src/TorBoxSDK/Main/General/GeneralClient.cs` — add both methods
- **Impact**: Minor breaking change.

### Step 4.2 — Remove queued duplicates in TorrentsClient
- **Gap**: The routes `torrents/getqueued` and `torrents/controlqueued` do not exist in Postman. QueuedClient uses the correct routes (`queued/getqueued`, `queued/controlqueued`).
- **File**: `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface — remove `GetQueuedTorrentsAsync` and `ControlQueuedTorrentsAsync`

---

## Phase 5 — Tests and Validation (*depends on all phases*)

### Step 5.1 — Unit tests for new parameters
- Tests for each modified model (Steps 1.1-1.10)
- Tests for Stream params (Step 2.1)
- Tests for QueuedClient params (Step 2.2)
- Tests for Search Options (Step 2.4)

### Step 5.2 — Update existing tests
- NotificationsClient changelogs → GeneralClient
- TorrentsClient queued → QueuedClient
- StreamClient signature changed → update
- GetTorrentInfoByFileAsync signature changed → update

### Step 5.3 — Build and validation
- `dotnet build` with no warnings
- `dotnet test` passes at 100%

---

## Quantitative Summary of Gaps Found

| Category | Number of Gaps | Severity |
|----------|---------------|----------|
| **Missing properties in Request/Options models** | 18 properties across 8 models | High |
| **Missing query parameters in clients** | 11 params across 4 methods | Critical (Stream) |
| **Undersized EditSettingsRequest model** | ~55 missing properties | High |
| **RSS AddRssRequest potentially incorrect JSON names** | 2 renames + 3 additions | Medium |
| **Non-existent Search API Options models** | 3 new models + 5 methods | High |
| **Organizational discrepancy (changelogs)** | 2 misplaced methods | Low |
| **Duplicates (queued in Torrents)** | 2 methods to remove | Low |
| **GetStreamData completely incorrect signature** | 5 params all wrong | Critical |
| **User Search Engine models entirely incorrect** | 3 models to overhaul | Critical |
| **Missing/incorrect User params** | 4 methods (referral, device, search, pdf) | High |
| **Incorrect Vendor content-type (JSON vs form-data)** | 3 methods | High |
| **Incorrect Vendor GetAccountByEmail param name** | 1 method | High |

---

## Affected Files

### Modified (models)
- `src/TorBoxSDK/Models/Torrents/CreateTorrentRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/Torrents/RequestDownloadOptions.cs` — +2 props
- `src/TorBoxSDK/Models/Usenet/CreateUsenetDownloadRequest.cs` — +2 props
- `src/TorBoxSDK/Models/Usenet/RequestUsenetDownloadOptions.cs` — +3 props
- `src/TorBoxSDK/Models/Usenet/EditUsenetDownloadRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/WebDownloads/CreateWebDownloadRequest.cs` — +2 props
- `src/TorBoxSDK/Models/WebDownloads/RequestWebDownloadOptions.cs` — +3 props
- `src/TorBoxSDK/Models/WebDownloads/EditWebDownloadRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/User/EditSettingsRequest.cs` — +~55 props (overhaul)
- `src/TorBoxSDK/Models/Rss/AddRssRequest.cs` — +3 props, 2 possible renames

### Modified (clients)
- `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` — GetTorrentInfo params, GetTorrentInfoByFile refactor, queued removal
- `src/TorBoxSDK/Main/Torrents/ITorrentsClient.cs` — same for interface
- `src/TorBoxSDK/Main/Stream/StreamClient.cs` — CreateStream +3 params, GetStreamData complete overhaul
- `src/TorBoxSDK/Main/Stream/IStreamClient.cs` — same for interface
- `src/TorBoxSDK/Main/Queued/QueuedClient.cs` — GetQueuedAsync +5 params
- `src/TorBoxSDK/Main/Queued/IQueuedClient.cs` — same for interface
- `src/TorBoxSDK/Search/SearchApiClient.cs` — 5 methods + Options param
- `src/TorBoxSDK/Search/ISearchApiClient.cs` — same for interface
- `src/TorBoxSDK/Main/Notifications/NotificationsClient.cs` — changelogs removal
- `src/TorBoxSDK/Main/General/GeneralClient.cs` — changelogs addition
- `src/TorBoxSDK/Main/User/UserClient.cs` — AddReferral query param, StartDeviceAuth +app, GetSearchEngines +id, GetTransactionPdf routing
- `src/TorBoxSDK/Main/User/IUserClient.cs` — same for interface
- `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — Register/UpdateAccount form-data, GetAccountByEmail rename, RegisterUser form-data
- `src/TorBoxSDK/Main/Vendors/IVendorsClient.cs` — same for interface

### Modified (User models - overhauls)
- `src/TorBoxSDK/Models/User/AddSearchEnginesRequest.cs` — complete overhaul (type, url, apikey, download_type)
- `src/TorBoxSDK/Models/User/ModifySearchEnginesRequest.cs` — complete overhaul (+id)
- `src/TorBoxSDK/Models/User/ControlSearchEnginesRequest.cs` — overhaul (operation, id, all)
- `src/TorBoxSDK/Models/User/AddReferralRequest.cs` — removed or simplified

### Created
- `src/TorBoxSDK/Models/Torrents/TorrentInfoRequest.cs`
- `src/TorBoxSDK/Models/Search/TorrentSearchOptions.cs`
- `src/TorBoxSDK/Models/Search/UsenetSearchOptions.cs`
- `src/TorBoxSDK/Models/Search/MetaSearchOptions.cs`

---

## Decisions

- **Breaking changes accepted**: SDK is not yet v1.0, so we fix now
- **Options nullable by default**: backward-compatible, methods work without options
- **EditSettingsRequest**: all new properties are nullable to avoid breaking partial updates
- **RSS JSON names**: Requires testing against the real API. If both `do_regex` and `regex_filter` work, keep `regex_filter`. Otherwise rename.
- **GetStreamData**: Breaking change at the parameter level because the current signature is incorrect vs the Postman API
- **GetIntercomHashAsync stays** in NotificationsClient
- **Get30DayStatsAsync stays** in GeneralClient
- **Extra SDK endpoints** (OAuth Integrations, Torznab, Newznab, etc.) retained

## Out of Scope
- No new endpoint additions (100% coverage already achieved)
- No docs/samples updates (future phase)
- No modifications to the 13 Integrations OAuth methods (not verifiable without Postman docs)
- No deep architectural refactoring
