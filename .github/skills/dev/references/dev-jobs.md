# Dev Jobs Reference

Définition complète des 6 types de jobs récurrents pour TorBoxSDK, avec leur cadence, ordre d'exécution, sortie attendue et checklist de complétion.

> Ce fichier ne duplique pas le contenu des skills spécialisés. Il définit le rôle de chaque job dans la chaîne de développement et le passage de relais entre jobs.

---

## Vue synthétique

| Job | Sortie principale | Job suivant habituel |
|-----|-------------------|----------------------|
| J1 Architecture | structure cible et placement des responsabilités | J2 ou J6 |
| J2 Endpoint | code fonctionnel d'un endpoint ou d'une tranche | J3 |
| J3 Tests | filet de sécurité et validation de comportement | J4 |
| J4 Code Review | verdict de validation avant merge | J5 ou fin de tâche |
| J5 Docs & Packaging | surface utilisateur et release readiness | fin de tâche ou release |
| J6 Foundation | socle buildable et injectable du SDK | J1 puis J2 |

---

## J1 — Architecture

**Quand :** avant d'écrire du code sur un nouveau composant, ou quand la structure existante est refactorisée.

**Skill à charger :** `.github/skills/architecture/SKILL.md`

**Jobs récurrents :**
- Définir ou adapter la hiérarchie `TorBoxClient → Main/Search/Relay → resource clients`
- Décider la surface publique d'un nouveau resource client (interface + implémentation)
- Concevoir un nouveau type d'exception ou d'erreur
- Refactorer le DI (`AddTorBox()`) quand les dépendances changent
- Statuer sur le découpage d'un namespace ou la création d'un sous-package

**Checklist de complétion J1 :**
- [ ] La hiérarchie client est respectée (aucun endpoint sur `TorBoxClient` directement)
- [ ] Les interfaces publiques sont définies avant les implémentations
- [ ] Pas de duplication de logique de transport entre les resource clients
- [ ] Les espaces de noms suivent la convention `TorBoxSDK.<Layer>.<Resource>`
- [ ] Le multi-target net6.0→net10.0 est préservé (pas d'API runtime non disponible sur net6.0)

---

## J2 — Endpoint Implementation

**Quand :** à chaque endpoint TorBox à ajouter selon le plan de développement (`docs/TODO.md`).

**Workflow à utiliser :** `J2` dans `/dev`, avec les références :
- `./endpoint-placement-and-naming.md`
- `./endpoint-implementation-checklist.md`

**Ordre de développement recommandé (par phase, conforme à `docs/TODO.md`) :**

### Phase 1 — Fondations (avant tout endpoint)
1. Multi-targeting `net6.0;net7.0;net8.0;net9.0;net10.0`
2. `Directory.Build.props` + `.editorconfig`
3. `TorBoxClient`, `ITorBoxClient`, `TorBoxClientOptions`
4. `MainApiClient`, `SearchApiClient`, `RelayApiClient`
5. Resource clients vides : `TorrentsClient`, `UsenetClient`, `WebDownloadsClient`, `UserClient`, `NotificationsClient`, `RssClient`, `StreamClient`, `IntegrationsClient`, `VendorsClient`, `QueuedClient`
6. `TorBoxResponse<T>`, `TorBoxResponse`, `TorBoxException`, `TorBoxErrorCode`
7. `AuthHandler` (DelegatingHandler), `AddTorBox()` DI

### Phase 2 — Modèles (par famille de ressource)
Implémenter les modèles dans cet ordre : **Common → Torrents → Usenet → WebDownloads → User → General → Notifications → RSS → Stream → Integrations → Vendors → Queued**

Pour chaque ressource :
- [ ] Modèle principal (ex : `Torrent`)
- [ ] Request models (ex : `CreateTorrentRequest`, `ControlTorrentRequest`)
- [ ] Enums spécifiques si nécessaire

### Phase 3 — Services Main API (par resource client)
Pour chaque resource client, ordre recommandé : **Torrents → Usenet → WebDownloads → User → General → Notifications → RSS → Stream → Integrations → Vendors → Queued**

Pour chaque endpoint :
- [ ] Méthode sur l'interface `ITorrentsClient`
- [ ] Implémentation dans `TorrentsClient`
- [ ] Tests unitaires (J3) immédiatement après

### Phase 4 — Search API
- Modèles : `SearchResult`, `TorrentSearchResult`, `UsenetSearchResult`, `MetaSearchResult`, enums
- Endpoints : `SearchTorrentsAsync`, `SearchUsenetAsync`, `SearchMetaAsync`, torznab/newznab

### Phase 5 — Relay API
- Modèles : `InactiveCheckResult`
- Endpoints : `GetStatusAsync`, `CheckForInactiveAsync`

**Checklist de complétion J2 (par endpoint) :**
- [ ] Modèle request créé (si paramètres complexes) avec `[JsonPropertyName]` sur chaque propriété
- [ ] Modèle response créé (ou réutilisation justifiée) avec `init` properties
- [ ] Méthode ajoutée à l'interface avec XML doc complète
- [ ] Implémentation dans le bon resource client, sans URL hardcodée
- [ ] `CancellationToken ct = default` en dernier paramètre, propagé à `_httpClient`
- [ ] Désérialisation via `TorBoxResponse<T>`
- [ ] Erreurs API mappées sur `TorBoxException` avec `TorBoxErrorCode`
- [ ] Validation des paramètres obligatoires en entrée (`ArgumentNullException.ThrowIfNull`)

---

## J3 — Tests

**Quand :** immédiatement après chaque endpoint implémenté (J2), ou pour améliorer la couverture existante.

**Skill à charger :** `.github/skills/tests/SKILL.md`

**Jobs récurrents et cadence :**

| Type | Quand | Emplacement |
|------|-------|-------------|
| Tests unitaires | Après chaque implémentation J2 | `tests/TorboxSDK.UnitTests/` |
| Tests de sérialisation | Après chaque nouveau modèle (J2 Phase 2) | `tests/TorboxSDK.UnitTests/Models/` |
| Tests d'intégration | Après stabilisation d'un resource client complet | `tests/TorBoxSDK.IntegrationTests/` |
| Tests de performance | Sur les chemins critiques (sérialisation, HTTP) | `tests/TorBoxSDK.PerformanceTests/` |

**Pour chaque endpoint (tests unitaires) :**
- [ ] Happy path : réponse `success: true` désérialisée correctement
- [ ] Erreur API : `success: false` → `TorBoxException` levée avec bon `ErrorCode`
- [ ] HTTP 4xx/5xx → exception typée
- [ ] Paramètres null obligatoires → `ArgumentNullException`
- [ ] URL + méthode HTTP corrects (vérifiés via `MockHttpMessageHandler`)
- [ ] Headers d'authentification présents dans la requête

**Checklist de complétion J3 :**
- [ ] Aucun test ne fait d'appel HTTP réel (`DelegatingHandler` mocké)
- [ ] Chaque test respecte `MethodName_State_ExpectedBehavior`
- [ ] Structure AAA avec commentaires `// Arrange` / `// Act` / `// Assert`
- [ ] Tests d'intégration marqués `[Trait("Category", "Integration")]` et skippés sans `TORBOX_API_KEY`
- [ ] `dotnet test` passe sans erreur

---

## J4 — Code Review

**Quand :** avant de considérer un développement comme terminé. **Toujours après J2 et J3.**

**Skill à charger :** `.github/skills/code-review/SKILL.md`

**Jobs récurrents :**
- Review d'un fichier modifié avant merge
- Audit complet d'un resource client après implémentation
- Vérification de conformité après refactoring

**Cadence recommandée :**
- Après chaque endpoint (J2) : review du fichier resource client + modèles
- Après chaque tranche de tests (J3) : review du fichier test
- Avant chaque phase (ex. passage Phase 2 → Phase 3) : audit du dossier concerné

**Checklist de complétion J4 :**
- [ ] Verdict `APPROVED` ou `APPROVED WITH MINOR ISSUES` obtenu sur tous les fichiers modifiés
- [ ] Aucun finding CRITICAL ou MAJOR non résolu

---

## J5 — Docs & Packaging

**Quand :** après la stabilisation d'un resource client complet, et obligatoirement avant toute release NuGet.

**Skill à charger :** `.github/skills/docs/SKILL.md`

**Jobs récurrents :**
- Mise à jour README après chaque phase terminée
- Création ou mise à jour d'un sample après chaque resource client stable
- Vérification du XML doc sur la surface publique avant release
- Préparation du NuGet metadata (PackageId, version, icon, tags)

**Checklist de complétion J5 :**
- [ ] README reflète les APIs couvertes
- [ ] Au moins un sample compilable par cas d'usage majeur
- [ ] `GenerateDocumentationFile` activé dans le csproj
- [ ] Tous les membres `public` ont `<summary>`, `<param>`, `<returns>`
- [ ] `PackageId`, `Description`, `Tags`, `RepositoryUrl`, `License` configurés dans le csproj
- [ ] SourceLink configuré (`Microsoft.SourceLink.GitHub`)

---

## J6 — Foundation Jobs

**Quand :** Phase 1 du projet, ou lors d'une restructuration de l'infrastructure.

Pas de skill dédié — suivre directement les tâches de la Phase 1 dans `docs/TODO.md`.

**Checklist Phase 1 :**

### 1.1 — Configuration projet
- [ ] `TorBoxSDK.csproj` : multi-targeting `net6.0;net7.0;net8.0;net9.0;net10.0`
- [ ] `Directory.Build.props` : nullable, implicit usings, version, authors, license, `TreatWarningsAsErrors`
- [ ] `.editorconfig` : conventions C# (s'appuyer sur `csharp-conventions.instructions.md`)
- [ ] Propriétés NuGet dans le csproj : `PackageId`, `Description`, `Tags`, `RepositoryUrl`, `LicenseExpression`, `Icon`
- [ ] SourceLink : `Microsoft.SourceLink.GitHub`

### 1.2 — Architecture de base
- [ ] `TorBoxClient` + `ITorBoxClient` + `TorBoxClientOptions`
- [ ] `MainApiClient`, `SearchApiClient`, `RelayApiClient` (façades)
- [ ] Resource clients vides exposés via `Main.*` (Torrents, Usenet, WebDownloads, User, Notifications, Rss, Stream, Integrations, Vendors, Queued)
- [ ] `AuthHandler` (DelegatingHandler Bearer token)
- [ ] `HttpClient` interne sans dépendance externe

### 1.3 — Réponse standard et erreurs
- [ ] `TorBoxResponse<T>` et `TorBoxResponse` (non-générique)
- [ ] `TorBoxException` avec `ErrorCode` et `Detail`
- [ ] `TorBoxErrorCode` enum complet (tous les codes API documentés)
- [ ] Désérialisation System.Text.Json avec `DateTimeOffset` UTC

### 1.4 — Injection de dépendances
- [ ] `AddTorBox(Action<TorBoxClientOptions>)` sur `IServiceCollection`
- [ ] `IHttpClientFactory` pour la gestion du `HttpClient` via des clients nommés
- [ ] Support `IConfiguration` section `TorBox`
- [ ] Seul `ITorBoxClient` est enregistré dans le conteneur DI
- [ ] Les sous-clients (`MainApiClient`, `SearchApiClient`, `RelayApiClient`, resource clients) sont `internal` et instanciés par `TorBoxClient` — pas enregistrés individuellement

**Critères de sortie Phase 1 :**
- `dotnet build` passe sans erreur ni warning sur tous les targets
- `services.AddTorBox(o => o.ApiKey = "...")` compile
- `provider.GetRequiredService<ITorBoxClient>()` résout le client
- `client.Main.Torrents` est accessible (même si vide)
- Les sous-clients ne sont pas résolvables directement depuis le conteneur DI

---

## Ordre de développement global recommandé

```
J6 Foundation (Phase 1)
  └─ J1 Architecture (valider la hiérarchie)
       └─ J2 Endpoint: Phase 2 modèles communs
            └─ J2 Endpoint: Phase 2 modèles Torrents
                 └─ J2 Endpoint: Phase 3.1 TorrentsClient
                      └─ J3 Tests unitaires TorrentsClient
                           └─ J4 Review TorrentsClient + tests
                                └─ (répéter J2→J3→J4 pour chaque resource client)
                                     └─ J2+J3+J4 Search API
                                          └─ J2+J3+J4 Relay API
                                               └─ J5 Docs + samples
                                                    └─ J5 NuGet + release
```
