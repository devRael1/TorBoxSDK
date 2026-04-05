# Plan : Correction et enrichissement du SDK TorBox

## TL;DR
L'analyse complète des 4 collections Postman (Main, Search, Relay, Vendors) param par param révèle **zéro endpoint manquant** mais **de nombreux paramètres absents**, **des modèles sous-dimensionnés**, **un écart organisationnel**, **des doublons**, et **des query params manquants sur 3 clients**. Ce plan corrige tout en 5 phases.

---

## Phase 1 — Paramètres manquants sur les modèles existants (priorité critique)

### Step 1.1 — `CreateTorrentRequest` : ajouter `add_only_if_cached`
- **Écart** : Postman `Create Torrent` et `Async Create Torrent` ont le champ `add_only_if_cached` (bool). Le SDK `CreateTorrentRequest` ne l'a pas.
- **Fichier** : `src/TorBoxSDK/Models/Torrents/CreateTorrentRequest.cs`
  - Ajouter propriété `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.2 — `RequestDownloadOptions` (Torrents) : ajouter `token` et `append_name`
- **Écart** : Postman `Request Download Link` (Torrents) envoie `token`, `redirect`, `append_name` en query. Le SDK a `redirect` mais manque `token` et `append_name`.
- **Fichier** : `src/TorBoxSDK/Models/Torrents/RequestDownloadOptions.cs`
  - Ajouter `Token` (string?, JSON: `"token"`)
  - Ajouter `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.3 — `RequestUsenetDownloadOptions` : ajouter `token`, `redirect`, `append_name`
- **Écart** : Postman Usenet `Request Download Link` a `token`, `redirect`, `append_name`. Le SDK n'a ni l'un ni l'autre.
- **Fichier** : `src/TorBoxSDK/Models/Usenet/RequestUsenetDownloadOptions.cs`
  - Ajouter `Token` (string?, JSON: `"token"`)
  - Ajouter `Redirect` (bool?, JSON: `"redirect"`)
  - Ajouter `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.4 — `RequestWebDownloadOptions` : ajouter `token`, `redirect`, `append_name`
- **Écart** : Postman Web Downloads `Request Download Link` a `token`, `redirect`, `append_name`. Le SDK n'a aucun des trois.
- **Fichier** : `src/TorBoxSDK/Models/WebDownloads/RequestWebDownloadOptions.cs`
  - Ajouter `Token` (string?, JSON: `"token"`)
  - Ajouter `Redirect` (bool?, JSON: `"redirect"`)
  - Ajouter `AppendName` (bool?, JSON: `"append_name"`)

### Step 1.5 — `CreateUsenetDownloadRequest` : ajouter `as_queued` et `add_only_if_cached`
- **Écart** : Postman `Create Usenet Download` a `as_queued` et `add_only_if_cached`. SDK n'a ni l'un ni l'autre.
- **Fichier** : `src/TorBoxSDK/Models/Usenet/CreateUsenetDownloadRequest.cs`
  - Ajouter `AsQueued` (bool?, JSON: `"as_queued"`)
  - Ajouter `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.6 — `CreateWebDownloadRequest` : ajouter `as_queued` et `add_only_if_cached`
- **Écart** : Postman `Create Web Download` a `as_queued` et `add_only_if_cached`. SDK n'a ni l'un ni l'autre.
- **Fichier** : `src/TorBoxSDK/Models/WebDownloads/CreateWebDownloadRequest.cs`
  - Ajouter `AsQueued` (bool?, JSON: `"as_queued"`)
  - Ajouter `AddOnlyIfCached` (bool?, JSON: `"add_only_if_cached"`)

### Step 1.7 — `EditUsenetDownloadRequest` : ajouter `alternative_hashes`
- **Écart** : Postman `Edit Usenet Item` envoie `{ usenet_download_id, name, tags, alternative_hashes }`. Le SDK n'a pas `alternative_hashes`.
- **Fichier** : `src/TorBoxSDK/Models/Usenet/EditUsenetDownloadRequest.cs`
  - Ajouter `AlternativeHashes` (IReadOnlyList<string>?, JSON: `"alternative_hashes"`)

### Step 1.8 — `EditWebDownloadRequest` : ajouter `alternative_hashes`
- **Écart** : Postman `Edit Web Download Item` envoie `{ webdl_id, name, tags, alternative_hashes }`. Le SDK n'a pas `alternative_hashes`.
- **Fichier** : `src/TorBoxSDK/Models/WebDownloads/EditWebDownloadRequest.cs`
  - Ajouter `AlternativeHashes` (IReadOnlyList<string>?, JSON: `"alternative_hashes"`)

### Step 1.9 — `GetTorrentInfoAsync` (GET) : ajouter `use_cache_lookup`
- **Écart** : Postman a le param `use_cache_lookup` (bool). SDK n'a que `hash` et `timeout`.
- **Fichier** : `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface
  - Ajouter paramètre `bool? useCacheLookup = null` à la signature
  - Ajouter en query string `use_cache_lookup`

### Step 1.10 — `GetTorrentInfoByFileAsync` (POST) : refactorer en `TorrentInfoRequest`
- **Écart** : Postman POST `/torrents/torrentinfo` accepte 6 champs form-data : `magnet`, `file`, `hash`, `timeout`, `use_cache_lookup`, `peers_only`. Le SDK n'accepte que `byte[] file`.
- **Nouveau fichier** : `src/TorBoxSDK/Models/Torrents/TorrentInfoRequest.cs`
  - `File` (byte[]?, multipart)
  - `Magnet` (string?, form field)
  - `Hash` (string?, form field)
  - `Timeout` (int?, form field)
  - `UseCacheLookup` (bool?, form field `"use_cache_lookup"`)
  - `PeersOnly` (bool?, form field `"peers_only"`)
- **Fichier** : `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface — remplacer `byte[] file` par `TorrentInfoRequest request`

---

## Phase 2 — Paramètres manquants sur les clients (query params / signatures)

### Step 2.1 — `StreamClient` : ajouter les paramètres de stream
- **Écart CRITIQUE** : Postman `Create Stream` a 6 query params : `id`, `file_id`, `type`, `chosen_subtitle_index`, `chosen_audio_index`, `chosen_resolution_index`. Le SDK n'en a que 3.
- **Fichier** : `src/TorBoxSDK/Main/Stream/StreamClient.cs` + interface
  - `CreateStreamAsync` : ajouter `int? chosenSubtitleIndex`, `int? chosenAudioIndex`, `int? chosenResolutionIndex`
- **Écart CRITIQUE** : Postman `Get Stream Data` a 5 query params complètement différents du SDK : `presigned_token`, `token`, `chosen_subtitle_index`, `chosen_audio_index`, `chosen_resolution_index`. Le SDK envoie `id`, `file_id`, `type` qui sont les params de Create, pas de GetStreamData.
- **Fichier** : `src/TorBoxSDK/Main/Stream/StreamClient.cs` + interface
  - `GetStreamDataAsync` : remplacer les params par `string presignedToken`, `string token`, `int? chosenSubtitleIndex`, `int? chosenAudioIndex`, `int? chosenResolutionIndex`

### Step 2.2 — `QueuedClient.GetQueuedAsync` : ajouter les query params
- **Écart** : Postman `Get Queued Downloads` accepte 5 query params : `bypass_cache`, `id`, `offset`, `limit`, `type`. Le SDK n'a aucun paramètre.
- **Fichier** : `src/TorBoxSDK/Main/Queued/QueuedClient.cs` + interface
  - `GetQueuedAsync(long? id, int? offset, int? limit, bool? bypassCache, string? type, CancellationToken)`

### Step 2.3 — Créer les modèles Options pour la Search API
- **Écart** : Les 4 endpoints principaux Search API n'acceptent aucun query param dans le SDK, alors que Postman en documente 7 : `metadata`, `season`, `episode`, `check_cache`, `check_owned`, `search_user_engines`, `cached_only`.
- **Nouveau fichier** : `src/TorBoxSDK/Models/Search/TorrentSearchOptions.cs`
  - `Metadata` (bool?), `Season` (int?), `Episode` (int?), `CheckCache` (bool?), `CheckOwned` (bool?), `SearchUserEngines` (bool?), `CachedOnly` (bool?)
- **Nouveau fichier** : `src/TorBoxSDK/Models/Search/UsenetSearchOptions.cs` — mêmes propriétés
- **Nouveau fichier** : `src/TorBoxSDK/Models/Search/MetaSearchOptions.cs` — `Type` (string?)

### Step 2.4 — Mettre à jour `SearchApiClient` pour utiliser les Options (*depends on 2.3*)
- **Fichier** : `src/TorBoxSDK/Search/SearchApiClient.cs` + interface
  - `SearchTorrentsAsync(string query, TorrentSearchOptions? options, CancellationToken)`
  - `GetTorrentByIdAsync(string id, TorrentSearchOptions? options, CancellationToken)`
  - `SearchUsenetAsync(string query, UsenetSearchOptions? options, CancellationToken)`
  - `GetUsenetByIdAsync(string id, UsenetSearchOptions? options, CancellationToken)`
  - `SearchMetaAsync(string query, MetaSearchOptions? options, CancellationToken)`

### Step 2.5 — `UserClient` : corriger les Search Engine models (signatures fausses)
- **Écart CRITIQUE** : Les 3 méthodes Search Engine du SDK ont des modèles Request complètement faux vs Postman :
  - **`AddSearchEnginesRequest`** : SDK envoie `{ "search_engines": [...] }`. Postman envoie `{ "type", "url", "apikey", "download_type" }`.
  - **`ModifySearchEnginesRequest`** : SDK envoie `{ "search_engines": [...] }`. Postman envoie `{ "id", "type", "url", "apikey", "download_type" }`.
  - **`ControlSearchEnginesRequest`** : SDK envoie `{ "operation", "search_engines": [...] }`. Postman envoie `{ "operation", "id", "all" }`.
- **Fichiers modèles** :
  - `src/TorBoxSDK/Models/User/AddSearchEnginesRequest.cs` — refonte : propriétés `Type` (string?), `Url` (string?), `Apikey` (string?), `DownloadType` (string?, JSON: `"download_type"`)
  - `src/TorBoxSDK/Models/User/ModifySearchEnginesRequest.cs` — refonte : propriétés `Id` (long), `Type` (string?), `Url` (string?), `Apikey` (string?), `DownloadType` (string?, JSON: `"download_type"`)
  - `src/TorBoxSDK/Models/User/ControlSearchEnginesRequest.cs` — refonte : propriétés `Operation` (string), `Id` (long?), `All` (bool?)

### Step 2.6 — `UserClient` : corriger les User params manquants/faux
- **`AddReferralAsync`** : SDK envoie un JSON body `{ "referral_code" }`. Postman utilise un **query param** `?referral=`. Méthode d'envoi fausse.
  - **Fichier** : `src/TorBoxSDK/Main/User/UserClient.cs` — changer pour envoyer `referral` en query string au lieu de JSON body
  - **Fichier** : `src/TorBoxSDK/Models/User/AddReferralRequest.cs` — peut être supprimé ou transformé en simple param string
- **`StartDeviceAuthAsync`** : Postman a query param `app` (string, nom de l'app). SDK n'en a pas.
  - **Fichier** : `src/TorBoxSDK/Main/User/UserClient.cs` + interface — ajouter `string? app = null`
- **`GetSearchEnginesAsync`** : Postman a query param `id` (long?, optionnel) pour récupérer un search engine spécifique. SDK n'en a pas.
  - **Fichier** : `src/TorBoxSDK/Main/User/UserClient.cs` + interface — ajouter `long? id = null`
- **`GetTransactionPdfAsync`** : Postman utilise `?transaction_id` en query param. SDK utilise `{transactionId}` en path. Route potentiellement fausse.
  - **Fichier** : `src/TorBoxSDK/Main/User/UserClient.cs` — vérifier et corriger vers query param si la route path ne marche pas

### Step 2.7 — `VendorsClient` : corriger content-type et param name
- **`RegisterAsync`** et **`UpdateAccountAsync`** : Postman envoie en **form-data** (`vendor_name`, `vendor_url`). SDK envoie en JSON. Écart de content-type.
  - **Fichier** : `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — changer `TorBoxApiHelper.JsonContent()` en multipart form-data pour ces 2 méthodes
- **`GetAccountByEmailAsync`** : Postman utilise le query param `user_auth_id`. SDK utilise `user_email`. Nom de param potentiellement faux.
  - **Fichier** : `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — renommer le param en `userAuthId` et le query param en `user_auth_id`
  - **Fichier** : `src/TorBoxSDK/Main/Vendors/IVendorsClient.cs` — renommer la méthode en `GetAccountByAuthIdAsync(string userAuthId, ...)` (breaking change)
- **`RegisterUserAsync`** : Postman envoie en **form-data** `user_email`. SDK envoie en JSON. Écart de content-type.
  - **Fichier** : `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — changer en multipart form-data

---

## Phase 3 — Modèles sous-dimensionnés (enrichissement)

### Step 3.1 — `EditSettingsRequest` : enrichir massif
- **Écart MAJEUR** : Postman `Edit User Settings` a ~60+ propriétés. Le SDK n'en a que 5 (`save_magnet_history`, `download_behavior`, `torrent_seed_preference`, `default_torrent_name`, `enable_notifications`).
- **Fichier** : `src/TorBoxSDK/Models/User/EditSettingsRequest.cs` — ajouter TOUTES les propriétés de Postman :
  - Notifications : `email_notifications`, `web_notifications`, `mobile_notifications`, `rss_notifications`, `discord_notifications`, `jdownloader_notifications`, `webhook_notifications`, `telegram_notifications`
  - Webhook/Telegram : `webhook_url`, `telegram_id`, `discord_id`
  - Stremio : `stremio_quality` (int[]?), `stremio_resolution` (int[]?), `stremio_language` (int[]?), `stremio_cache` (int[]?), `stremio_size_lower` (long?), `stremio_size_upper` (long?), `stremio_allow_adult` (bool?), `stremio_seed_torrents` (int?), `stremio_sort` (string?), `stremio_use_custom_search_engines` (bool?), `stremio_result_sort` (string?), `stremio_legacy_your_media` (bool?), `stremio_only_your_media_streams` (bool?), `stremio_disable_your_media_streams` (bool?), `stremio_limit_per_resolution_torrent` (int?), `stremio_limit_per_resolution_usenet` (int?), `stremio_torrent_seeders_cutoff` (int?), `stremio_wait_for_download_usenet` (bool?), `stremio_wait_for_download_torrent` (bool?), `stremio_disable_filtered_note` (bool?), `stremio_emoji_in_description` (bool?)
  - Downloads : `seed_torrents` (int?), `allow_zipped` (bool?), `stremio_allow_zipped` (bool?), `cdn_selection` (string?), `append_filename_to_links` (bool?)
  - Cloud storage : `google_drive_folder_id` (string?), `onedrive_save_path` (string?), `onefichier_folder_id` (string?), `gofile_folder_id` (string?), `pixeldrain_api_key` (string?), `onefichier_api_key` (string?), `gofile_api_key` (string?), `mega_email` (string?), `mega_password` (string?)
  - UI : `download_speed_in_tab` (bool?), `show_tracker_in_torrents` (bool?), `dashboard_filter` (object?), `dashboard_sort` (string?), `patreon_id` (string?)
  - Web Player : `web_player_always_transcode` (bool?), `web_player_always_skip_intro` (bool?), `web_player_audio_preferred_language` (string?), `web_player_subtitle_preferred_language` (string?), `web_player_disable_prestream_selector` (bool?), `web_player_disable_next_up_dialogue` (bool?), `web_player_enable_scrobbling` (bool?)
  - WebDAV : `webdav_use_local_files` (bool?), `webdav_use_folder_view` (bool?), `webdav_flatten` (bool?)

### Step 3.2 — `AddRssRequest` : ajouter les champs manquants
- **Écart** : Postman `Add RSS Feed` a ces champs en plus du SDK : `do_regex` (pas `regex_filter`), `dont_regex` (pas `regex_filter_exclude`), `dont_older_than`, `pass_check`, `torrent_seeding`.
- **Vérification nécessaire** : Confirmer que les JSON names dans le SDK (`regex_filter` / `regex_filter_exclude`) correspondent bien à l'API réelle. Postman utilise `do_regex` / `dont_regex`. Si l'API accepte les deux, pas d'action. Sinon :
  - **Fichier** : `src/TorBoxSDK/Models/Rss/AddRssRequest.cs`
    - Renommer `RegexFilter` JSON name de `"regex_filter"` à `"do_regex"`
    - Renommer `RegexFilterExclude` JSON name de `"regex_filter_exclude"` à `"dont_regex"`
    - Ajouter `DontOlderThan` (int?, JSON: `"dont_older_than"`)
    - Ajouter `PassCheck` (bool?, JSON: `"pass_check"`)
    - Ajouter `TorrentSeeding` (int?, JSON: `"torrent_seeding"`)

---

## Phase 4 — Corrections organisationnelles

### Step 4.1 — Déplacer les Changelogs de NotificationsClient vers GeneralClient
- **Écart** : Postman les classe sous `General`, le SDK dans `NotificationsClient`.
- **Fichier** : `src/TorBoxSDK/Main/Notifications/NotificationsClient.cs` — supprimer `GetChangelogsRssAsync` et `GetChangelogsJsonAsync`
- **Fichier** : `src/TorBoxSDK/Main/General/GeneralClient.cs` — ajouter les deux méthodes
- **Impact** : Breaking change mineur.

### Step 4.2 — Supprimer les doublons queued dans TorrentsClient
- **Écart** : Les routes `torrents/getqueued` et `torrents/controlqueued` n'existent pas dans Postman. QueuedClient utilise les bonnes routes (`queued/getqueued`, `queued/controlqueued`).
- **Fichier** : `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` + interface — supprimer `GetQueuedTorrentsAsync` et `ControlQueuedTorrentsAsync`

---

## Phase 5 — Tests et validation (*depends on toutes les phases*)

### Step 5.1 — Tests unitaires pour les nouveaux paramètres
- Tests pour chaque modèle modifié (Steps 1.1-1.10)
- Tests pour les Stream params (Step 2.1)
- Tests pour QueuedClient params (Step 2.2)
- Tests pour Search Options (Step 2.4)

### Step 5.2 — Adapter les tests existants
- NotificationsClient changelogs → GeneralClient
- TorrentsClient queued → QueuedClient
- StreamClient signature changée → mettre à jour
- GetTorrentInfoByFileAsync signature changée → mettre à jour

### Step 5.3 — Build et validation
- `dotnet build` sans warnings
- `dotnet test` passe à 100%

---

## Résumé quantitatif des écarts trouvés

| Catégorie | Nombre d'écarts | Sévérité |
|-----------|----------------|----------|
| **Propriétés manquantes dans les Request/Options models** | 18 propriétés sur 8 modèles | Haute |
| **Paramètres de query manquants dans les clients** | 11 params sur 4 méthodes | Critique (Stream) |
| **Modèle EditSettingsRequest sous-dimensionné** | ~55 propriétés manquantes | Haute |
| **RSS AddRssRequest JSON names potentiellement faux** | 2 renames + 3 ajouts | Moyenne |
| **Search API Options models inexistants** | 3 nouveaux models + 5 méthodes | Haute |
| **Écart organisationnel (changelogs)** | 2 méthodes mal placées | Basse |
| **Doublons (queued dans Torrents)** | 2 méthodes à supprimer | Basse |
| **GetStreamData signature complètement fausse** | 5 params tous faux | Critique |
| **User Search Engine models totalement faux** | 3 models à refondre | Critique |
| **User params manquants/faux** | 4 méthodes (referral, device, search, pdf) | Haute |
| **Vendor content-type faux (JSON vs form-data)** | 3 méthodes | Haute |
| **Vendor GetAccountByEmail param name faux** | 1 méthode | Haute |

---

## Fichiers concernés

### Modifiés (modèles)
- `src/TorBoxSDK/Models/Torrents/CreateTorrentRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/Torrents/RequestDownloadOptions.cs` — +2 props
- `src/TorBoxSDK/Models/Usenet/CreateUsenetDownloadRequest.cs` — +2 props
- `src/TorBoxSDK/Models/Usenet/RequestUsenetDownloadOptions.cs` — +3 props
- `src/TorBoxSDK/Models/Usenet/EditUsenetDownloadRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/WebDownloads/CreateWebDownloadRequest.cs` — +2 props
- `src/TorBoxSDK/Models/WebDownloads/RequestWebDownloadOptions.cs` — +3 props
- `src/TorBoxSDK/Models/WebDownloads/EditWebDownloadRequest.cs` — +1 prop
- `src/TorBoxSDK/Models/User/EditSettingsRequest.cs` — +~55 props (refonte)
- `src/TorBoxSDK/Models/Rss/AddRssRequest.cs` — +3 props, 2 renames possibles

### Modifiés (clients)
- `src/TorBoxSDK/Main/Torrents/TorrentsClient.cs` — GetTorrentInfo params, GetTorrentInfoByFile refactor, suppression queued
- `src/TorBoxSDK/Main/Torrents/ITorrentsClient.cs` — idem interface
- `src/TorBoxSDK/Main/Stream/StreamClient.cs` — CreateStream +3 params, GetStreamData refonte complète
- `src/TorBoxSDK/Main/Stream/IStreamClient.cs` — idem interface
- `src/TorBoxSDK/Main/Queued/QueuedClient.cs` — GetQueuedAsync +5 params
- `src/TorBoxSDK/Main/Queued/IQueuedClient.cs` — idem interface
- `src/TorBoxSDK/Search/SearchApiClient.cs` — 5 méthodes + Options param
- `src/TorBoxSDK/Search/ISearchApiClient.cs` — idem interface
- `src/TorBoxSDK/Main/Notifications/NotificationsClient.cs` — suppression changelogs
- `src/TorBoxSDK/Main/General/GeneralClient.cs` — ajout changelogs
- `src/TorBoxSDK/Main/User/UserClient.cs` — AddReferral query param, StartDeviceAuth +app, GetSearchEngines +id, GetTransactionPdf routing
- `src/TorBoxSDK/Main/User/IUserClient.cs` — idem interface
- `src/TorBoxSDK/Main/Vendors/VendorsClient.cs` — Register/UpdateAccount form-data, GetAccountByEmail rename, RegisterUser form-data
- `src/TorBoxSDK/Main/Vendors/IVendorsClient.cs` — idem interface

### Modifiés (modèles User - refontes)
- `src/TorBoxSDK/Models/User/AddSearchEnginesRequest.cs` — refonte complète (type, url, apikey, download_type)
- `src/TorBoxSDK/Models/User/ModifySearchEnginesRequest.cs` — refonte complète (+id)
- `src/TorBoxSDK/Models/User/ControlSearchEnginesRequest.cs` — refonte (operation, id, all)
- `src/TorBoxSDK/Models/User/AddReferralRequest.cs` — supprimé ou simplifié

### Créés
- `src/TorBoxSDK/Models/Torrents/TorrentInfoRequest.cs`
- `src/TorBoxSDK/Models/Search/TorrentSearchOptions.cs`
- `src/TorBoxSDK/Models/Search/UsenetSearchOptions.cs`
- `src/TorBoxSDK/Models/Search/MetaSearchOptions.cs`

---

## Décisions

- **Breaking changes acceptés** : SDK pas encore v1.0, on corrige maintenant
- **Options nullable par défaut** : backward-compatible, les méthodes marchent sans options
- **EditSettingsRequest** : toutes les nouvelles propriétés sont nullable pour ne pas casser l'envoi partiel
- **RSS JSON names** : Nécessite test contre l'API réelle. Si `do_regex` et `regex_filter` marchent tous les deux, garder `regex_filter`. Sinon renommer.
- **GetStreamData** : Breaking change au niveau paramètres car la signature actuelle est incorrecte vs l'API Postman
- **GetIntercomHashAsync reste** dans NotificationsClient
- **Get30DayStatsAsync reste** dans GeneralClient
- **Endpoints extras SDK** (OAuth Integrations, Torznab, Newznab, etc.) conservés

## Hors périmètre
- Pas d'ajout de nouveaux endpoints (100% coverage déjà atteinte)
- Pas de mise à jour docs/samples (Phase future)
- Pas de modification des 13 méthodes Integrations OAuth (non vérifiables sans doc Postman)
- Pas de refactoring architectural profond
