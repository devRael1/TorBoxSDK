# Plan : TorBoxClient standalone (sans DI)

## TL;DR
Rendre `TorBoxClient` instanciable directement sans DI en ajoutant 3 constructeurs publics, `IDisposable`, et un `AuthHandler` simplifié. Le chemin DI existant est refactoré mais reste fonctionnel. Pas de breaking changes (SDK non publié).

---

## Phase 1 — Modifications core SDK

### Étape 1 : `ITorBoxClient.cs` — Ajouter `IDisposable`
- `ITorBoxClient` étend `IDisposable`
- Permet `using var client = new TorBoxClient("key");` en mode standalone
- En mode DI, le container gère le cycle de vie automatiquement

**Fichier** : `src/TorBoxSDK/ITorBoxClient.cs`

### Étape 2 : `TorBoxClientOptions.cs` — Rendre les URLs versionnées publiques
- `MainApiVersionedUrl` : `internal` → `public`
- `RelayApiVersionedUrl` : `internal` → `public`
- Permet aux utilisateurs d'inspecter les URLs résolues
- Les propriétés restent en lecture seule (get-only)

**Fichier** : `src/TorBoxSDK/TorBoxClientOptions.cs`

### Étape 3 : `AuthHandler.cs` — Constructeur string
- Ajouter un constructeur `AuthHandler(string apiKey)` pour le mode standalone
- Conserver le constructeur `AuthHandler(IOptions<TorBoxClientOptions> options)` pour le mode DI
- Remplacer le primary constructor par deux constructeurs traditionnels
- Le champ `_apiKey` stocke la clé directement dans les deux cas

**Fichier** : `src/TorBoxSDK/Http/Handlers/AuthHandler.cs`

### Étape 4 : `TorBoxClient.cs` — Constructeurs publics + IDisposable
Ajouter 3 constructeurs publics standalone + rendre le constructeur DI public :

1. **`TorBoxClient(string apiKey)`** — le plus simple, options par défaut
   - Délègue à `TorBoxClient(TorBoxClientOptions)`
   
2. **`TorBoxClient(TorBoxClientOptions options)`** — contrôle complet
   - Valide les options (ApiKey non vide, URLs valides)
   - Crée 3 `HttpClient` avec `AuthHandler(apiKey)` + `HttpClientHandler` comme InnerHandler
   - Configure `BaseAddress` et `Timeout` sur chaque HttpClient
   - Stocke les HttpClients dans des champs `_ownedXxxClient` pour disposal
   
3. **`TorBoxClient(Action<TorBoxClientOptions> configure)`** — pattern builder
   - Crée un `TorBoxClientOptions`, applique le delegate, délègue au constructeur (2)

4. **`TorBoxClient(IHttpClientFactory, IOptions<TorBoxClientOptions>)`** — DI (existant, rendu `public`)
   - Ne stocke PAS les HttpClients (pas de ownership → `_ownedXxxClient` reste `null`)
   - Marqué `[ActivatorUtilitiesConstructor]` pour guider la résolution DI

**IDisposable** :
- `Dispose()` libère uniquement les `_ownedXxxClient` (mode standalone)
- En mode DI, `_ownedXxxClient` est `null` → `Dispose()` est un no-op
- Champ `_disposed` pour éviter le double-dispose

**Méthode privée statique** `CreateHttpClient(string apiKey, string baseUrl, TimeSpan timeout)` :
- Crée `AuthHandler(apiKey) { InnerHandler = new HttpClientHandler() }`
- Retourne `new HttpClient(handler) { BaseAddress, Timeout }`

**Validation** dans le constructeur `TorBoxClientOptions` :
- `Guard.ThrowIfNullOrEmpty(options.ApiKey)`
- `Uri.TryCreate` sur les base URLs

**Fichier** : `src/TorBoxSDK/TorBoxClient.cs`

### Étape 5 : `TorBoxServiceCollectionExtensions.cs` — Refactoring DI
- Simplifier `RegisterCore` : remplacer le factory lambda par `services.AddScoped<ITorBoxClient, TorBoxClient>()`
  - Le container DI choisira automatiquement le constructeur `(IHttpClientFactory, IOptions<TorBoxClientOptions>)` car c'est le seul dont tous les paramètres sont résolvables
- Conserver l'enregistrement des named HttpClients (MainApi, SearchApi, RelayApi) avec `AddHttpClient`
- Conserver l'enregistrement de `AuthHandler` en transient pour le pipeline DI
- Conserver les deux overloads `AddTorBox(Action<>)` et `AddTorBox(IConfiguration)`

**Fichier** : `src/TorBoxSDK/DependencyInjection/TorBoxServiceCollectionExtensions.cs`

---

## Phase 2 — Tests

### Étape 6 : Tests unitaires standalone (*parallel avec étape 7*)
Créer `tests/TorboxSDK.UnitTests/TorBoxClientTests.cs` :

**Constructeurs** :
- `Constructor_WithApiKey_CreatesClientWithAllSubClients`
- `Constructor_WithOptions_CreatesClientWithAllSubClients`
- `Constructor_WithConfigure_CreatesClientWithAllSubClients`
- `Constructor_WithNullApiKey_ThrowsArgumentException`
- `Constructor_WithEmptyApiKey_ThrowsArgumentException`
- `Constructor_WithNullOptions_ThrowsArgumentNullException`
- `Constructor_WithNullConfigure_ThrowsArgumentNullException`
- `Constructor_WithCustomBaseUrls_UsesProvidedUrls`
- `Constructor_WithCustomTimeout_UsesProvidedTimeout`

**IDisposable** :
- `Dispose_WhenStandalone_DisposesOwnedClients`
- `Dispose_CalledTwice_DoesNotThrow`

**DI (inchangé mais validé)** :
- Les tests existants dans `DependencyInjection/TorBoxServiceCollectionExtensionsTests.cs` restent valides
- Vérifier que le refactoring DI n'a rien cassé

### Étape 7 : Tests unitaires AuthHandler (*parallel avec étape 6*)
Ajouter dans `tests/TorboxSDK.UnitTests/Http/` :

- `AuthHandler_WithStringApiKey_SetsAuthorizationHeader`
- `AuthHandler_WithEmptyApiKey_DoesNotSetAuthorizationHeader`

### Étape 8 : Tests d'intégration standalone
Créer `tests/TorBoxSDK.IntegrationTests/Standalone/StandaloneClientTests.cs` :

- `StandaloneClient_WithValidApiKey_CanCallApi` — `[Trait("Category", "Integration")]`, skip si pas de `TORBOX_API_KEY`
- `StandaloneClient_WithOptions_CanCallApi`

### Étape 9 : Mettre à jour les tests existants si nécessaire
- `TorBoxIntegrationFixture.cs` : vérifier que `DisposeAsync` fonctionne toujours (le DI client fait un Dispose no-op → ok)
- `FrameworkCompatibilityTests.cs` : pas de changement nécessaire
- `ClientTestBase.cs` : pas de changement (utilise la réflexion sur les sous-clients, pas TorBoxClient)

---

## Phase 3 — Samples

### Étape 10 : Sample standalone
Créer `src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs` :

- Montre les 3 constructeurs
- `using var client = new TorBoxClient(apiKey);`
- `using var client = new TorBoxClient(new TorBoxClientOptions { ... });`
- `using var client = new TorBoxClient(opts => { opts.ApiKey = ...; });`
- Error handling avec `TorBoxException`
- `CancellationToken` via `CancellationTokenSource`

Mettre à jour `src/TorBoxSDK.Examples/Program.cs` pour ajouter l'option au menu.

Optionnel : mettre à jour `ExampleHelper.cs` pour offrir un `CreateStandaloneClient()`.

---

## Phase 4 — Documentation

### Étape 11 : `README.md` — Ajouter une section standalone
- Ajouter une section **Standalone Usage** juste après **Quick Start** (qui reste DI)
- Montrer les 3 constructeurs standalone avec `using var client = ...`
- Mentionner `IDisposable` et le pattern `using`
- Garder la section Quick Start DI existante intacte

**Fichier** : `README.md`

### Étape 12 : `docs/getting-started.md` — Section standalone
- Ajouter une section **Use without dependency injection** avant ou après la section DI existante
- Montrer le constructeur simple `new TorBoxClient("key")` et le constructeur options
- Rappeler que `IDisposable` est implémenté → `using` recommandé
- Ajouter un encadré expliquant quand choisir standalone vs DI

**Fichier** : `docs/getting-started.md`

### Étape 13 : `docs/configuration.md` — Options standalone
- Ajouter une section **Configure without DI** montrant l'instanciation directe de `TorBoxClientOptions`
- Documenter les 3 patterns : clé seule, objet options, delegate `Action<TorBoxClientOptions>`
- Documenter les propriétés rendues publiques `MainApiVersionedUrl` et `RelayApiVersionedUrl` dans la table des options
- Conserver les sections DI existantes telles quelles

**Fichier** : `docs/configuration.md`

### Étape 14 : `docs/architecture.md` — Mise à jour instantiation
- Mettre à jour la section **DI and instantiation** pour couvrir les deux modes (DI et standalone)
- Renommer la section en **Instantiation** ou **DI and standalone instantiation**
- Ajouter un diagramme ou tableau comparatif DI vs standalone
- Documenter le comportement de `Dispose()` : no-op en DI, libère les HttpClients en standalone
- Mentionner `[ActivatorUtilitiesConstructor]` pour le constructeur DI

**Fichier** : `docs/architecture.md`

---

## Fichiers concernés

| Fichier | Action |
|---------|--------|
| `src/TorBoxSDK/ITorBoxClient.cs` | Ajouter `: IDisposable` |
| `src/TorBoxSDK/TorBoxClient.cs` | 3 constructeurs publics, DI public, IDisposable, `CreateHttpClient` |
| `src/TorBoxSDK/TorBoxClientOptions.cs` | `MainApiVersionedUrl` et `RelayApiVersionedUrl` → public |
| `src/TorBoxSDK/Http/Handlers/AuthHandler.cs` | Ajout constructeur `AuthHandler(string apiKey)` |
| `src/TorBoxSDK/DependencyInjection/TorBoxServiceCollectionExtensions.cs` | Simplifier `RegisterCore` |
| `tests/TorboxSDK.UnitTests/TorBoxClientTests.cs` | **Nouveau** — tests standalone |
| `tests/TorboxSDK.UnitTests/Http/AuthHandlerTests.cs` | Tests pour le nouveau constructeur (si pas existant) |
| `tests/TorBoxSDK.IntegrationTests/Standalone/StandaloneClientTests.cs` | **Nouveau** — tests intégration standalone |
| `src/TorBoxSDK.Examples/GettingStarted/StandaloneSetupExample.cs` | **Nouveau** — sample standalone |
| `src/TorBoxSDK.Examples/Program.cs` | Ajouter entrée menu |
| `README.md` | Ajouter section standalone usage |
| `docs/getting-started.md` | Ajouter section standalone |
| `docs/configuration.md` | Ajouter configuration standalone + propriétés publiques |
| `docs/architecture.md` | Mettre à jour section instantiation (DI + standalone) |

## Vérification

1. `dotnet build src/TorBoxSDK/TorBoxSDK.csproj` — compilation sans erreurs
2. `dotnet test tests/TorboxSDK.UnitTests/` — tous les tests existants + nouveaux passent (cible ~495+ tests)
3. `dotnet test tests/TorBoxSDK.IntegrationTests/ --filter "Category=Integration"` — tests DI existants passent toujours
4. `dotnet test tests/TorBoxSDK.SchemaValidationTests/ --filter "Category!=Live"` — schema tests inchangés (265 tests)
5. Vérifier manuellement que `new TorBoxClient("test")` compile depuis un projet externe
6. Vérifier que `using var client = new TorBoxClient("key");` fonctionne avec `IDisposable`

## Décisions

- `IDisposable` ajouté à `ITorBoxClient` (pas seulement `TorBoxClient`) pour que le pattern `using` fonctionne via l'interface
- Le constructeur DI reste et est rendu `public` avec `[ActivatorUtilitiesConstructor]` pour guider la résolution
- Les `_ownedXxxClient` ne sont non-null qu'en mode standalone → `Dispose()` est un no-op en mode DI
- Validation des options faite dans le constructeur `TorBoxClient(TorBoxClientOptions)` (pas dans le modèle options lui-même, conformément aux conventions)
- `AuthHandler` reste `internal` — les utilisateurs n'y accèdent jamais directement

## Considérations

1. **SocketsHttpHandler vs HttpClientHandler** : Sur .NET 5+, `SocketsHttpHandler` est le handler par défaut derrière `new HttpClient()`. On pourrait utiliser explicitement `SocketsHttpHandler` pour les TFMs qui le supportent, mais `HttpClientHandler` est portable et suffisant. → Recommandation : rester sur `HttpClientHandler` pour la simplicité cross-TFM.

2. **`MainApiVersionedUrl`/`RelayApiVersionedUrl` publics** : Utile pour le debug mais expose des propriétés computées. → Recommandation : les rendre publics car demandé.
