# Task 5 report — V2 bounded, envelope-preserving HTTP transport

## Statut et handoff

L'implémentation Task 5 et ses validations déterministes sont terminées. Ce
rapport est un handoff au contrôleur : l'implémenteur n'émet pas de verdict de
relecture indépendante sur son propre changement.

Le commit de livraison associé porte le message :
`feat: add V2 envelope-preserving HTTP transport`.

## Périmètre livré

Fichiers de production créés sous `src/TorBoxSDK.V2/Http/` :

- `ITorBoxApiTransport.cs`
- `TorBoxApiTransport.cs`
- `TorBoxEnvelopeJsonConverterFactory.cs`
- `BoundedDiagnosticReader.cs`
- `HttpContentStreamReader.cs`
- `Handlers/AuthHandler.cs`
- `TorBoxHttpClientHandlerFactory.cs`
- `QueryStringBuilder.cs`

Fichiers de test créés :

- `tests/TorBoxSDK.V2.Testing/RecordingHttpMessageHandler.cs`
- `tests/TorBoxSDK.V2.UnitTests/Http/TorBoxApiTransportTests.cs`
- `tests/TorBoxSDK.V2.UnitTests/Http/HttpContentStreamReaderTests.cs`
- `tests/TorBoxSDK.V2.UnitTests/Http/AuthHandlerTests.cs`
- `tests/TorBoxSDK.V2.UnitTests/Http/QueryStringBuilderTests.cs`

## Décisions d'implémentation

- `TorBoxApiTransport` envoie chaque requête une seule fois avec
  `HttpCompletionOption.ResponseHeadersRead` et transmet le token d'annulation.
- Les enveloppes JSON sont lues de façon incrémentale. Aucune gestion de
  réponse n'utilise `ReadAsStringAsync`.
- Une enveloppe JSON structurée `success:false` devient toujours une valeur
  `TorBoxResponse` ou `TorBoxStreamResponse`, avec le statut HTTP conservé,
  pour les statuts 2xx comme non-2xx.
- `TorBoxProtocolException` est réservé aux réponses invalides, non lisibles ou
  non prises en charge. Ses diagnostics passent par le helper borné Task 4 et
  sont limités à 64 KiB.
- Les erreurs JSON ou d'E/S pendant le parsing sont normalisées en
  `TorBoxProtocolException`; l'annulation n'est pas transformée en erreur de
  protocole.
- Les flux binaires et les réponses de redirection transfèrent leur ownership à
  `TorBoxStreamResponse`. Les réponses HTTP JSON sont disposées une fois
  l'enveloppe lue.
- Le handler HTTP créé par `TorBoxHttpClientHandlerFactory` désactive les
  redirections automatiques afin de préserver la réponse et sa métadonnée
  `Location`.
- `AuthHandler` reste interne et prend `string apiKey`. Il refuse une clé null,
  vide ou blanche avant tout envoi, écrase une autorisation préexistante par un
  unique header `Bearer`, et ne contient pas la clé dans son message d'erreur.
- `QueryStringBuilder` omet les valeurs null et escape les noms et valeurs par
  `Uri.EscapeDataString`.
- `HttpContentStreamReader` conserve la compatibilité `netstandard2.0` pour
  `ReadAsStreamAsync` et utilise l'overload avec token lorsque disponible.

## Preuves TDD — RED puis GREEN

### RED initial valide

Les tests et le handler de recording ont été ajoutés avant les fichiers de
production. La première compilation a d'abord révélé que
`ArgumentException.ThrowIfNullOrWhiteSpace` n'est pas disponible sur net6 et
net7 ; le helper de test a été rendu compatible avant de retenir le RED
fonctionnel.

Commande RED :

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~Http"
```

Résultat observé : échec de compilation `CS0234` dans les quatre fichiers de
tests HTTP (`TorBoxSDK.Http` inexistant), sur net6.0, net7.0, net8.0, net9.0 et
net10.0. Aucun fichier de production HTTP n'existait alors.

### Premiers GREEN et corrections guidées par les tests

Après l'implémentation minimale, le premier passage compilait et exécutait 27
tests par TFM ; deux assertions de tests étaient incorrectes :

- une annulation préexistante est levée synchroniquement par le helper, donc
  l'appel devait être placé dans l'assertion ;
- `HttpClient` fournit au handler terminal un token lié, et non nécessairement
  la même instance. Le test a été remplacé par la preuve que ce token est
  effectivement annulé au handler terminal.

La correction de ces deux tests a conduit à 27/27 tests HTTP verts par TFM.

Un test de lecture bornée a ensuite semblé lire 131072 octets. L'instrumentation
du contenu a confirmé un seul stream et aucune sérialisation ; le double compte
provenait du stream de test, dont `ReadAsync` appelait une implémentation qui
passait déjà par `Read`. Le compteur a été corrigé, sans changer la production,
et le test a prouvé une lecture inférieure ou égale à 65536 octets.

Enfin, le test RED d'un stream JSON qui lève `IOException` exposait l'exception
brute sur les cinq TFM. Le convertisseur d'enveloppe capture désormais
`IOException` avec `JsonException` et les normalise en
`TorBoxProtocolException`. Le test est devenu vert 1/1 par TFM.

## Couverture déterministe exercée

Les tests HTTP couvrent notamment :

- `success:false` JSON pour 200, 401 et 500, avec préservation du statut,
  `error` et `detail`, pour les transports générique, non-générique et stream ;
- l'ordre quelconque des propriétés, notamment `data` avant `success` et
  `detail` avant/après `success` ;
- une propriété ignorée volumineuse et des diagnostics limités à 64 KiB ;
- JSON malformed, stream JSON non lisible et contenu non JSON non pris en
  charge ;
- exactement un envoi, propagation fonctionnelle de l'annulation et disposition
  des réponses JSON ;
- transfert de flux binaire, longueur, type média, nom de fichier et
  redirection ;
- désactivation d'auto-redirection, header Bearer unique, rejet pré-envoi d'une
  clé blanche sans fuite, et construction de query string encodée sans null.

## Validations finales exécutées

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~Http"
```

Résultat : **31 passed, 0 failed** pour chacun de net6.0, net7.0, net8.0,
net9.0 et net10.0.

```powershell
dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore
```

Résultat : build Release vert pour netstandard2.0 et net6.0 à net10.0,
**0 warning, 0 error**.

```powershell
dotnet test tests\TorBoxSDK.V2.ContractTests\TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-build --no-restore
```

Résultat : **39 passed, 0 failed, 1 skipped** par TFM. Le skip est la gate de
complétude de release hors périmètre Task 5 ; aucune requête réseau n'a été
effectuée.

```powershell
dotnet format TorBoxSDK.V2.slnx whitespace --no-restore --include <fichiers-Task5>
dotnet format TorBoxSDK.V2.slnx style --verify-no-changes --no-restore --include <fichiers-Task5>
dotnet format TorBoxSDK.V2.slnx analyzers --verify-no-changes --no-restore --include <fichiers-Task5>
git diff --cached --check
```

Résultat : toutes les commandes de format ont retourné 0 (avec le warning
générique de chargement de workspace) ; le contrôle final d'espaces Git est
vide.

## Contraintes respectées

- Aucun appel réseau vers TorBox, aucune clé réelle et aucun secret.
- Aucun retry, backoff ou suivi de redirection caché.
- Aucun changement de projet, lockfile, plan, spécification ou fichier Task 1
  à 4.
- Toutes les réponses utilisées par les tests sont synthétiques via
  `RecordingHttpMessageHandler`.

## Handoff Task 6

Task 6 peut composer `AuthHandler` interne avec une clé déjà validée dans ses
options, utiliser `TorBoxHttpClientHandlerFactory` pour construire le handler
sans auto-redirection et dépendre de `ITorBoxApiTransport`. Le transport ne
prend pas de dépendance anticipée sur les options publiques Task 6.

## Fix round 1/5 — corrections demandées après relecture

Cette section couvre exclusivement les trois findings MAJOR de
`task-5-contract-review.md`. Les deux findings MINOR consignés au ledger
restent volontairement hors périmètre de ce round.

### 1. `data` avant `success:false` dans une réponse générique

Régression ajoutée :
`SendAsync_WithDataBeforeFailureAndIncompatibleGenericData_ReturnsFailureEnvelopeWithoutDeserializingData`.
Elle envoie, pour `T=int`, l'enveloppe ordonnée ainsi :

```json
{
  "data": "not-an-int",
  "error": "BAD_TOKEN",
  "detail": "Invalid token.",
  "success": false
}
```

RED observé :

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~SendAsync_WithDataBeforeFailureAndIncompatibleGenericData"
```

Le test échouait 1/1 sur net6.0 à net10.0 avec
`TorBoxProtocolException` / `JsonException` depuis `DeserializeData<int>` :
la chaîne n'était pas convertible vers `Int32`.

GREEN observé avec la même commande : 1/1 par TFM. La réponse est maintenant
une `TorBoxResponse<int>` failure, avec `BAD_TOKEN`, `Invalid token.`, le
statut 401 et `Data == default`.

Correction : `DeserializeData<T>` n'est appelé qu'après lecture complète de
l'enveloppe et seulement lorsque `Success` final vaut `true`. Lors de la
lecture de `success:false`, un éventuel buffer déjà capturé est immédiatement
libéré de l'état du reader, et `EnvelopeValues` ne transporte jamais de
`DataJson` pour une failure.

Limite explicitement conservée pour préserver le contrat : lorsque `data`
apparaît avant `success`, le reader ne peut pas encore savoir si l'enveloppe
finira en succès. Il conserve donc temporairement le JSON de `data` afin de
préserver les réponses succès data-first, y compris les grandes réponses
succès. Ce buffer temporaire n'est pas borné dans cette situation ; il est
abandonné dès qu'un `success:false` ultérieur est connu et n'est ni
désérialisé ni conservé dans la valeur de failure. Ce round ne prétend pas
borner un `data` data-first sans casser le support des grands succès ; cette
contrepartie est signalée pour la rerevue ciblée.

### 2. Priorité d'une failure JSON sur une redirection HTTP

Régression ajoutée :
`SendStreamAsync_WithJsonFailureRedirectResponse_ReturnsTheFailureEnvelope`.
Elle simule `307 Temporary Redirect`, un header `Location`,
`application/json`, et `success:false`.

RED observé :

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~SendStreamAsync_WithJsonFailureRedirectResponse"
```

Le test échouait 1/1 sur net6.0 à net10.0 à `Assert.False`: la réponse était
un redirect avec `Success == true`.

GREEN observé avec la même commande : 1/1 par TFM. `SendStreamAsync` analyse
d'abord une réponse dont le content type est JSON ; le `307` JSON devient une
`TorBoxStreamResponse` failure sans `RedirectUri` ni stream, en préservant
statut, erreur et détail. La classification redirect et son transfer
d'ownership restent ensuite réservés aux réponses non JSON ; le test de
redirection non JSON préexistant reste inclus dans la suite HTTP complète.

### 3. Échecs d'E/S de contenu HTTP

Régressions ajoutées :

- `SendAsync_WithJsonContentThatFailsToAcquireStream_ThrowsTorBoxProtocolException`;
- `SendAsync_WithUnsupportedContentThatFailsToAcquireStream_ThrowsTorBoxProtocolException`;
- `SendAsync_WithUnsupportedContentWhoseDiagnosticReadFails_ThrowsTorBoxProtocolException`;
- `SendStreamAsync_WithJsonContentThatFailsToAcquireStream_ThrowsTorBoxProtocolException`.

Les deux premières et la quatrième utilisent un `HttpContent` dont
`CreateContentReadStreamAsync` lève `IOException`. La troisième acquiert son
stream, puis lève `IOException` pendant la lecture bornée du diagnostic.
Toutes vérifient l'URI de requête résolue, le statut HTTP, un diagnostic null
et l'`IOException` interne.

RED observé :

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~ContentThatFailsToAcquireStream|FullyQualifiedName~DiagnosticReadFails"
```

Les quatre tests échouaient 4/4 sur chacun de net6.0 à net10.0, car
`IOException` s'échappait brut : acquisition JSON générique, acquisition
non-JSON, lecture du diagnostic non-JSON et acquisition JSON stream.

GREEN observé avec la même commande : 4/4 par TFM. `ReadJsonContentAsync` et
`ReadDiagnosticAsync` capturent uniquement `IOException` et créent une
`TorBoxProtocolException` avec URI, statut et détail null. Une lecture de
diagnostic qui échoue n'invente donc aucun détail non borné. Il n'y a aucun
catch général : `OperationCanceledException` continue de se propager et
`HttpClient.SendAsync` reste avant les blocs qui traitent le contenu, de sorte
que ses exceptions réseau ne sont pas masquées.

### Validation du round

```powershell
dotnet test tests\TorBoxSDK.V2.UnitTests\TorBoxSDK.V2.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~Http"
dotnet build TorBoxSDK.V2.slnx --configuration Release --no-restore
dotnet test tests\TorBoxSDK.V2.ContractTests\TorBoxSDK.V2.ContractTests.csproj --configuration Release --no-build --no-restore
dotnet format TorBoxSDK.V2.slnx whitespace --no-restore --include src/TorBoxSDK.V2/Http/TorBoxApiTransport.cs src/TorBoxSDK.V2/Http/TorBoxEnvelopeJsonConverterFactory.cs tests/TorBoxSDK.V2.UnitTests/Http/TorBoxApiTransportTests.cs
dotnet format TorBoxSDK.V2.slnx style --verify-no-changes --no-restore --include src/TorBoxSDK.V2/Http/TorBoxApiTransport.cs src/TorBoxSDK.V2/Http/TorBoxEnvelopeJsonConverterFactory.cs tests/TorBoxSDK.V2.UnitTests/Http/TorBoxApiTransportTests.cs
dotnet format TorBoxSDK.V2.slnx analyzers --verify-no-changes --no-restore --include src/TorBoxSDK.V2/Http/TorBoxApiTransport.cs src/TorBoxSDK.V2/Http/TorBoxEnvelopeJsonConverterFactory.cs tests/TorBoxSDK.V2.UnitTests/Http/TorBoxApiTransportTests.cs
git diff --check
git diff --cached --check
```

Résultats observés :

- filtre HTTP : **37 passed, 0 failed** pour net6.0, net7.0, net8.0, net9.0
  et net10.0 ;
- build Release : netstandard2.0 et net6.0 à net10.0, **0 warning, 0 error** ;
- ContractTests hors réseau : **39 passed, 0 failed, 1 skipped** par TFM
  (gate release existante) ;
- formatter ciblé : sorties 0 ; les trois commandes ont uniquement émis
  l'avertissement générique de chargement du workspace.
- contrôles d'espaces Git, avant puis après indexation : aucune sortie.
