# TorBoxSDK — Plan de développement

SDK C# open-source pour l'API TorBox, compatible .NET 6 à .NET 10.

## Récapitulatif des APIs à couvrir

| API | Base URL | Endpoints |
|-----|----------|-----------|
| **Main API** | `https://api.torbox.app/v1/api` | ~90 endpoints |
| **Search API** | `https://search-api.torbox.app` | ~12 endpoints |
| **Relay API** | `https://relay.torbox.app` | ~2 endpoints |

---

## Phase 1 — Fondations du SDK

### 1.1 Configuration du projet multi-target

- [ ] Configurer le multi-targeting `net6.0;net7.0;net8.0;net9.0;net10.0` dans `TorBoxSDK.csproj`
- [ ] Ajouter `Directory.Build.props` pour les propriétés partagées (nullable, implicit usings, version, authors, license)
- [ ] Ajouter un `.editorconfig` à la racine pour les conventions de code C#
- [ ] Configurer les propriétés NuGet dans le csproj (PackageId, Description, Tags, RepositoryUrl, License, Icon)
- [ ] Ajouter le SourceLink pour le débogage NuGet (`Microsoft.SourceLink.GitHub`)

### 1.2 Architecture de base du client HTTP

- [ ] Créer `TorBoxClient` — point d'entrée principal du SDK
- [ ] Créer `TorBoxClientOptions` — configuration (ApiKey, BaseUrl, ApiVersion, timeout)
- [ ] Créer `ITorBoxClient` — interface publique
- [ ] Structurer le SDK par famille d'API : `Main`, `Search`, `Relay`
- [ ] Exposer les APIs via des propriétés sur `TorBoxClient` (`Main`, `Search`, `Relay`)
- [ ] Créer `MainApiClient` — façade dédiée à `https://api.torbox.app`
- [ ] Créer `SearchApiClient` — façade dédiée à `https://search-api.torbox.app`
- [ ] Créer `RelayApiClient` — façade dédiée à `https://relay.torbox.app`
- [ ] Dans `MainApiClient`, exposer des resource clients dédiés : `Torrents`, `Usenet`, `WebDownloads`, `User`, `Notifications`, `Rss`, `Stream`, `Integrations`, `Vendors`, `Queued`
- [ ] Définir une convention similaire pour `SearchApiClient` et `RelayApiClient` en gardant une surface claire et compacte
- [ ] Implémenter le client HTTP interne basé sur `HttpClient` (pas de dépendance externe)
- [ ] Gérer l'authentification Bearer Token via `DelegatingHandler`
- [ ] Support du `CancellationToken` sur toutes les méthodes publiques

### 1.3 Réponse standard et gestion d'erreurs

- [ ] Créer `TorBoxResponse<T>` — modèle de réponse standard (`Success`, `Error`, `Detail`, `Data`)
- [ ] Créer `TorBoxResponse` — version non-générique pour les endpoints sans data
- [ ] Créer `TorBoxException` — exception typée avec code d'erreur et détail
- [ ] Créer `TorBoxErrorCode` — enum de tous les codes d'erreur API (DATABASE_ERROR, BAD_TOKEN, etc.)
- [ ] Implémenter la désérialisation JSON (System.Text.Json) avec support des dates UTC (`%Y-%m-%dT%H:%M:%SZ`)
- [ ] Gérer les cas HTTP 200/400/403/500 avec exceptions typées

### 1.4 Injection de dépendances

- [ ] Créer un package séparé `TorBoxSDK.Extensions.DependencyInjection` ou intégrer dans le package principal
- [ ] Créer `AddTorBox(Action<TorBoxClientOptions>)` extension method sur `IServiceCollection`
- [ ] Utiliser `IHttpClientFactory` pour la gestion du `HttpClient`
- [ ] Support de la configuration via `IConfiguration` (section `TorBox`)

---

## Phase 2 — Modèles de données (Main API)

### 2.1 Modèles communs

- [ ] `TorBoxResponse<T>` / `TorBoxResponse` (déjà en Phase 1)
- [ ] Enums partagés : `DownloadStatus`, `DownloadType`, `ControlOperation`

### 2.2 Torrents

- [ ] `Torrent` — modèle principal d'un torrent
- [ ] `CreateTorrentRequest` — fichier ou magnet, seed, allow_zip, name, as_queued
- [ ] `ControlTorrentRequest` — operation, torrent_id, all
- [ ] `EditTorrentRequest` — torrent_id, name, tags, alternative_hashes
- [ ] `RequestDownloadOptions` — torrent_id, file_id, zip_link, user_ip, redirect
- [ ] `CheckCachedRequest` — hashes, format, list_files
- [ ] `TorrentInfo` — info complète d'un torrent (hash, timeout, peers)
- [ ] `ExportDataOptions` — torrent_id, type
- [ ] `MagnetToFileRequest` — magnet

### 2.3 Usenet

- [ ] `UsenetDownload` — modèle principal
- [ ] `CreateUsenetDownloadRequest` — link/file, name, password, post_processing
- [ ] `ControlUsenetDownloadRequest` — operation, usenet_id, all
- [ ] `EditUsenetDownloadRequest` — usenet_download_id, name, tags
- [ ] `RequestUsenetDownloadOptions` — usenet_id, file_id, zip_link, user_ip

### 2.4 Web Downloads / Debrid

- [ ] `WebDownload` — modèle principal
- [ ] `CreateWebDownloadRequest` — link, password, name
- [ ] `ControlWebDownloadRequest` — operation, webdl_id, all
- [ ] `EditWebDownloadRequest` — webdl_id, name, tags
- [ ] `RequestWebDownloadOptions` — web_id, file_id, zip_link, user_ip
- [ ] `Hoster` — modèle d'un hébergeur supporté

### 2.5 User

- [ ] `User` — modèle utilisateur avec settings
- [ ] `UserSettings` (BaseSettingsModel) — toutes les préférences utilisateur
- [ ] `RefreshTokenRequest` — session_token
- [ ] `DeviceCodeResponse` / `DeviceTokenRequest`
- [ ] `Subscription`, `Transaction` — modèles financiers
- [ ] `SearchEngine` / `SearchEngineEditRequest` / `ControlSearchEngineRequest`
- [ ] `DeleteAccountRequest` — session_token, confirmation_code

### 2.6 General / Notifications

- [ ] `Stats` / `Stats30Days` — statistiques TorBox
- [ ] `Notification` — modèle de notification
- [ ] `Changelog` — modèle de changelog
- [ ] `SpeedtestOptions` — user_ip, region, test_length

### 2.7 RSS

- [ ] `RssFeed` / `RssFeedItem` — modèles RSS
- [ ] `AddRssRequest` — url, name, regex filters, scan_interval, rss_type
- [ ] `ControlRssRequest` — operation, rss_feed_id
- [ ] `ModifyRssRequest` — rss_feed_id, name, regex, interval

### 2.8 Stream

- [ ] `CreateStreamOptions` — id, file_id, type, subtitle/audio/resolution indexes
- [ ] `StreamData` — données de stream

### 2.9 Integrations

- [ ] `AddToGoogleDriveRequest`, `AddToDropboxRequest`, `AddToOnedriveRequest`
- [ ] `AddToGofileRequest`, `AddTo1FichierRequest`, `AddToPixeldrainRequest`
- [ ] `IntegrationJob` — modèle de job de transfert
- [ ] `OAuthIntegration` / `OAuthRegisterRequest`

### 2.10 Vendors

- [ ] `RegisterVendorRequest` — vendor_name, vendor_url
- [ ] `VendorAccount` / `UpdateVendorAccountRequest`
- [ ] `RegisterVendorUserRequest` — user_email

### 2.11 Queued

- [ ] `QueuedDownload` — modèle de téléchargement en file d'attente
- [ ] `ControlQueuedRequest` — operation, queued_id, all

---

## Phase 3 — Services API (Main API)

Chaque service est un client métier dédié exposé via `TorBoxClient.Main`.

### 3.1 ITorrentsService

- [ ] `CreateTorrentAsync` — POST /torrents/createtorrent (multipart)
- [ ] `AsyncCreateTorrentAsync` — POST /torrents/asynccreatetorrent
- [ ] `ControlTorrentAsync` — POST /torrents/controltorrent
- [ ] `GetQueuedTorrentsAsync` — GET /torrents/getqueued
- [ ] `ControlQueuedAsync` — POST /torrents/controlqueued
- [ ] `RequestDownloadAsync` — GET /torrents/requestdl
- [ ] `GetMyTorrentListAsync` — GET /torrents/mylist (+ surcharge by id)
- [ ] `CheckCachedAsync` — GET et POST /torrents/checkcached
- [ ] `ExportDataAsync` — GET /torrents/exportdata
- [ ] `MagnetToFileAsync` — POST /torrents/magnettofile
- [ ] `GetTorrentInfoAsync` — GET /torrents/torrentinfo
- [ ] `GetTorrentInfoByFileAsync` — POST /torrents/torrentinfo
- [ ] `EditTorrentAsync` — PUT /torrents/edittorrent

### 3.2 IUsenetService

- [ ] `CreateDownloadAsync` — POST /usenet/createusenetdownload
- [ ] `AsyncCreateDownloadAsync` — POST /usenet/asynccreateusenetdownload
- [ ] `ControlDownloadAsync` — POST /usenet/controlusenetdownload
- [ ] `RequestDownloadAsync` — GET /usenet/requestdl
- [ ] `GetMyListAsync` — GET /usenet/mylist
- [ ] `CheckCachedAsync` — GET et POST /usenet/checkcached
- [ ] `EditDownloadAsync` — PUT /usenet/editusenetdownload

### 3.3 IWebDownloadsService

- [ ] `CreateDownloadAsync` — POST /webdl/createwebdownload
- [ ] `AsyncCreateDownloadAsync` — POST /webdl/asynccreatewebdownload
- [ ] `ControlDownloadAsync` — POST /webdl/controlwebdownload
- [ ] `RequestDownloadAsync` — GET /webdl/requestdl
- [ ] `GetMyListAsync` — GET /webdl/mylist
- [ ] `CheckCachedAsync` — GET et POST /webdl/checkcached
- [ ] `GetHostersAsync` — GET /webdl/hosters
- [ ] `EditDownloadAsync` — PUT /webdl/editwebdownload

### 3.4 IUserService

- [ ] `RefreshTokenAsync` — POST /user/refreshtoken
- [ ] `GetConfirmationCodeAsync` — GET /user/getconfirmation
- [ ] `GetMeAsync` — GET /user/me (+ option settings)
- [ ] `AddReferralAsync` — POST /user/addreferral
- [ ] `StartDeviceCodeAsync` — GET /user/auth/device/start
- [ ] `GetTokenFromDeviceCodeAsync` — POST /user/auth/device/token
- [ ] `DeleteAccountAsync` — DELETE /user/deleteme
- [ ] `GetReferralDataAsync` — GET /user/referraldata
- [ ] `GetSubscriptionsAsync` — GET /user/subscriptions
- [ ] `GetTransactionsAsync` — GET /user/transactions
- [ ] `GetTransactionPdfAsync` — GET /user/transaction/pdf
- [ ] `AddSearchEngineAsync` — PUT /user/settings/addsearchengines
- [ ] `GetSearchEnginesAsync` — GET /user/settings/searchengines
- [ ] `EditSearchEngineAsync` — POST /user/settings/modifysearchengines
- [ ] `ControlSearchEngineAsync` — POST /user/settings/controlsearchengines
- [ ] `EditSettingsAsync` — PUT /user/settings/editsettings

### 3.5 IGeneralService

- [ ] `GetStatusAsync` — GET /
- [ ] `GetStatsAsync` — GET /stats
- [ ] `Get30DayStatsAsync` — GET /stats/30days
- [ ] `GetSpeedtestFilesAsync` — GET /speedtest

### 3.6 INotificationsService

- [ ] `GetRssNotificationsAsync` — GET /notifications/rss
- [ ] `GetNotificationsAsync` — GET /notifications/mynotifications
- [ ] `ClearAllAsync` — POST /notifications/clear
- [ ] `ClearAsync` — POST /notifications/clear/{id}
- [ ] `TestNotificationAsync` — POST /notifications/test
- [ ] `GetIntercomHashAsync` — GET /intercom/hash
- [ ] `GetRssChangelogAsync` — GET /changelogs/rss
- [ ] `GetJsonChangelogAsync` — GET /changelogs/json

### 3.7 IRssService

- [ ] `AddFeedAsync` — POST /rss/addrss
- [ ] `ControlFeedAsync` — POST /rss/controlrss
- [ ] `ModifyFeedAsync` — POST /rss/modifyrss
- [ ] `GetFeedsAsync` — GET /rss/getfeeds
- [ ] `GetFeedItemsAsync` — GET /rss/getfeeditems

### 3.8 IStreamService

- [ ] `CreateStreamAsync` — GET /stream/createstream
- [ ] `GetStreamDataAsync` — GET /stream/getstreamdata

### 3.9 IIntegrationsService

- [ ] `GetOAuthIntegrationsAsync` — GET /integration/oauth/me
- [ ] `OAuthRedirectAsync` — GET /integration/oauth/{provider}
- [ ] `OAuthCallbackAsync` — GET+POST /integration/oauth/{provider}/callback
- [ ] `OAuthSuccessAsync` — GET /integration/oauth/{provider}/success
- [ ] `OAuthRegisterAsync` — POST /integration/oauth/{provider}/register
- [ ] `OAuthUnregisterAsync` — DELETE /integration/oauth/{provider}/unregister
- [ ] `GetLinkedDiscordRolesAsync` — POST /integration/oauth/discord/linked_roles
- [ ] `AddToGoogleDriveAsync` — POST /integration/googledrive
- [ ] `AddToDropboxAsync` — POST /integration/dropbox
- [ ] `AddToOnedriveAsync` — POST /integration/onedrive
- [ ] `AddToGofileAsync` — POST /integration/gofile
- [ ] `AddTo1FichierAsync` — POST /integration/1fichier
- [ ] `AddToPixeldrainAsync` — POST /integration/pixeldrain
- [ ] `GetJobStatusAsync` — GET /integration/job/{job_id}
- [ ] `CancelJobAsync` — DELETE /integration/job/{job_id}
- [ ] `GetAllJobsAsync` — GET /integration/jobs
- [ ] `GetJobByHashAsync` — GET /integration/jobs/{hash}

### 3.10 IVendorsService

- [ ] `RegisterVendorAsync` — POST /vendors/register
- [ ] `GetVendorAccountAsync` — GET /vendors/account
- [ ] `UpdateVendorAccountAsync` — PUT /vendors/updateaccount
- [ ] `GetVendorAccountsAsync` — GET /vendors/getaccounts
- [ ] `GetUserVendorAccountAsync` — GET /vendors/getaccount
- [ ] `RegisterUserAsync` — POST /vendors/registeruser
- [ ] `DeleteUserAsync` — DELETE /vendors/removeuser
- [ ] `RefreshVendorUsersAsync` — PATCH /vendors/refresh

### 3.11 IQueuedService

- [ ] `GetQueuedDownloadsAsync` — GET /queued/getqueued
- [ ] `ControlQueuedAsync` — POST /queued/controlqueued

---

## Phase 4 — Search API

Base URL : `https://search-api.torbox.app`

Cette API doit être exposée via `TorBoxClient.Search`.

### 4.1 Modèles

- [ ] `SearchResult`, `TorrentSearchResult`, `UsenetSearchResult`, `MetaSearchResult`
- [ ] `MediaType` enum, `SearchType` enum

### 4.2 ISearchService

- [ ] `GetTorrentSearchTutorialAsync` — GET /torrents
- [ ] `SearchTorrentsAsync` — GET /torrents/search/{query}
- [ ] `GetTorrentByIdAsync` — GET /torrents/{id}
- [ ] `GetUsenetSearchTutorialAsync` — GET /usenet
- [ ] `SearchUsenetAsync` — GET /usenet/search/{query}
- [ ] `GetUsenetByIdAsync` — GET /usenet/{id}
- [ ] `DownloadUsenetAsync` — GET /usenet/download/{id}/{guid}
- [ ] `GetMetaSearchTutorialAsync` — GET /meta
- [ ] `SearchMetaAsync` — GET /meta/search/{query}
- [ ] `GetMetaByIdAsync` — GET /meta/{id}
- [ ] `SearchTorznabAsync` — GET /torznab/api
- [ ] `SearchNewznabAsync` — GET /newznab/api

---

## Phase 5 — Relay API

Base URL : `https://relay.torbox.app`

Cette API doit être exposée via `TorBoxClient.Relay`.

### 5.1 Modèles

- [ ] `InactiveCheckResult`

### 5.2 IRelayService

- [ ] `GetStatusAsync` — GET /
- [ ] `CheckForInactiveAsync` — GET /v1/inactivecheck/torrent/{auth_id}/{torrent_id}

---

## Phase 6 — Tests

### 6.1 Tests unitaires

- [ ] Tests de sérialisation/désérialisation de tous les modèles
- [ ] Tests de `TorBoxResponse<T>` avec succès et erreurs
- [ ] Tests du mapping `TorBoxErrorCode`
- [ ] Tests de `TorBoxClient` avec `HttpMessageHandler` mocké
- [ ] Tests de chaque service avec réponses simulées
- [ ] Tests de l'extension DI `AddTorBox()`
- [ ] Tests des edge cases (null, nullable, dates UTC)

### 6.2 Tests d'intégration

- [ ] Tests de bout en bout avec l'API réelle (nécessitent une clé API)
- [ ] Marqués `[Trait("Category", "Integration")]` pour pouvoir les exclure
- [ ] Couvrir au minimum : auth, create/list/control torrent, user info

### 6.3 Tests de performance

- [ ] Benchmarks de sérialisation/désérialisation JSON (BenchmarkDotNet)
- [ ] Benchmarks de throughput HTTP simulé

---

## Phase 7 — Documentation et samples

### 7.1 README

- [ ] Badges (NuGet, build, coverage, license)
- [ ] Description du projet, features, APIs couvertes
- [ ] Quick start (installation NuGet, code minimal)
- [ ] Exemples d'utilisation DI et sans DI
- [ ] Table de compatibilité .NET
- [ ] Lien vers la doc complète

### 7.2 Samples

- [ ] `samples/BasicUsage` — console app minimaliste
- [ ] `samples/DependencyInjection` — avec `IServiceCollection`
- [ ] `samples/TorrentDownload` — workflow complet : create → check status → request download

### 7.3 Documentation XML

- [ ] Documenter toutes les classes et méthodes publiques avec `<summary>`, `<param>`, `<returns>`
- [ ] Activer `GenerateDocumentationFile` dans le csproj

---

## Phase 8 — CI/CD et Distribution

### 8.1 GitHub Actions

- [ ] Workflow `build.yml` — build + tests sur chaque push/PR
- [ ] Matrix de tests sur .NET 6/7/8/9/10
- [ ] Workflow `release.yml` — publish NuGet + GitHub Packages sur tag

### 8.2 NuGet

- [ ] Configuration NuGet.org (API key en secret)
- [ ] Configuration GitHub Packages
- [ ] Versionning SemVer automatique depuis les tags Git

### 8.3 Qualité

- [ ] Code coverage avec Coverlet + rapport dans CI
- [ ] Analyse statique (nullable warnings as errors, `TreatWarningsAsErrors`)

---

## Ordre de développement recommandé

```
Phase 1 (Fondations)
  └─> Phase 2.1-2.2 (Modèles communs + Torrents)
       └─> Phase 3.1 (TorrentsService)
            └─> Phase 6.1 (Tests unitaires premiers services)
                 └─> Phase 2.3-2.11 (Reste des modèles)
                      └─> Phase 3.2-3.11 (Reste des services Main API)
                           └─> Phase 4 (Search API)
                                └─> Phase 5 (Relay API)
                                     └─> Phase 7 (Documentation + Samples)
                                          └─> Phase 8 (CI/CD)
```

---

## Inventaire des endpoints (résumé)

| Section | Endpoints | Priorité |
|---------|-----------|----------|
| Torrents | 13 | Haute |
| User | 16 | Haute |
| Web Downloads | 9 | Haute |
| Usenet | 8 | Haute |
| General | 4 | Moyenne |
| Notifications | 8 | Moyenne |
| RSS | 5 | Moyenne |
| Stream | 2 | Moyenne |
| Integrations | 17 | Moyenne |
| Vendors | 8 | Basse |
| Queued | 2 | Basse |
| **Search API** | 12 | Moyenne |
| **Relay API** | 2 | Basse |
| **Total** | **~106** | |
