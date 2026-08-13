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
