# Analyse de compatibilité .NET

> DEC-002 est validée. Matrice cible : `netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0`. Baseline .NET Framework testée : `net472`, complétée par des smoke tests `net48` et `net481`.

## Situation actuelle

`TorBoxSDK.csproj` cible :

```xml
<TargetFrameworks>net6.0;net7.0;net8.0;net9.0;net10.0</TargetFrameworks>
```

Les dépendances `Microsoft.Extensions.*` sont adaptées conditionnellement pour `net6.0` et `net7.0`, tandis que les cibles plus récentes utilisent la ligne de packages configurée par défaut. Cette matrice offre un actif spécifique pour cinq générations de .NET, mais elle ne couvre pas .NET Framework et maintient des cibles sorties du support Microsoft.

L'audit du code n'a trouvé que deux différences conditionnelles significatives : une garde d'argument à partir de .NET 7 et la politique JSON `snake_case` native à partir de .NET 8. Les XML d'API publique sont identiques entre `net6.0`/`net7.0`, puis entre `net8.0`/`net9.0`/`net10.0`. Les cinq actifs sont donc en grande partie redondants ; ils ne sont utiles séparément que si leurs dépendances, leur comportement ou leur niveau de support le justifient.

La politique actuelle dans `SECURITY.md` annonce des mises à jour de sécurité pour les cinq TFMs. Cette formulation doit être alignée sur DEC-002 : TorBoxSDK peut maintenir un actif compatible, mais ne peut pas remettre un runtime .NET hors support dans un état sécurisé.

## Matrice cible validée

```text
netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0
```

| Environnement consommateur | Actif attendu | Niveau annoncé |
|---|---|---|
| .NET Framework 4.6.1 à 4.7.1 | `netstandard2.0` | compatible selon NuGet, non testé/non supporté |
| .NET Framework 4.7.2 | `netstandard2.0` | baseline Framework testée et supportée par TorBoxSDK |
| .NET Framework 4.8 / 4.8.1 | `netstandard2.0` | smoke tests Windows et support TorBoxSDK |
| .NET Core 2.0 à 3.1 / .NET 5 | `netstandard2.0` | compatibilité à prouver par consommateurs ; runtimes hors support Microsoft |
| .NET 6 / 7 | actif spécifique | compatibilité legacy construite et testée ; runtime hors support Microsoft |
| .NET 8 / 9 / 10 | actif spécifique | construit et testé ; support aligné sur le calendrier Microsoft |
| version .NET stable ultérieure | meilleur actif compatible | smoke test à la sortie ; TFM ajouté seulement si justifié et validé |

`netstandard2.1` n'est pas ajouté : .NET Framework ne le prend pas en charge, tandis que les consommateurs modernes capables de l'utiliser peuvent aussi consommer `netstandard2.0` ou leur actif .NET spécifique. Cette absence augmente donc la portée au lieu de la réduire.

## Compatibilité, support et optimisation

Ces notions doivent être publiées séparément :

- **compatible** : NuGet sélectionne un actif utilisable et un projet consommateur de test compile ;
- **testé** : la CI exécute compilation et scénarios définis dans cet environnement ;
- **supporté par TorBoxSDK** : le projet accepte les incidents et publie des correctifs pour cet environnement ;
- **supporté par Microsoft** : le runtime reçoit encore le niveau de maintenance défini par Microsoft ;
- **optimisé** : un actif spécifique profite d'API ou de performances propres à cette cible.

Un consommateur peut être techniquement compatible grâce à `netstandard2.0` sans que TorBoxSDK promette de tester toutes les implémentations possibles.

## Calendrier Microsoft au 10 août 2026

| Cible actuelle | Canal | État Microsoft | Fin du support |
|---|---|---|---|
| .NET 6 | LTS | hors support | 12 novembre 2024 |
| .NET 7 | STS | hors support | 14 mai 2024 |
| .NET 8 | LTS | maintenance | 10 novembre 2026 |
| .NET 9 | STS | maintenance | 10 novembre 2026 |
| .NET 10 | LTS | actif | 14 novembre 2028 |

La proximité de novembre 2026 est importante : une matrice choisie maintenant doit indiquer si une modification est déjà prévue à cette date.

Pour .NET Framework, Microsoft indique notamment 4.7.2, 4.8 et 4.8.1 comme versions actives liées au cycle de vie de Windows ; 4.6.2 arrive en fin de support le 12 janvier 2027.

## Portée de .NET Standard 2.0

La documentation Microsoft recommande `netstandard2.0` lorsqu'une bibliothèque doit atteindre largement .NET, y compris .NET Framework. La compatibilité NuGet commence à .NET Framework 4.6.1, mais Microsoft recommande 4.7.2 ou plus pour éviter plusieurs problèmes de consommation.

Il n'existe pas de nouvelle version de .NET Standard prévue. Pour les bibliothèques modernes, Microsoft recommande aussi de cibler une version actuelle de .NET, et d'ajouter `netstandard2.0` comme cible lorsque la portée supplémentaire est nécessaire.

Ajouter `netstandard2.0` n'est donc ni automatiquement nécessaire, ni gratuit. Il faut vérifier :

- toutes les API BCL utilisées ;
- la disponibilité et la version de `System.Text.Json` ;
- les packages `Microsoft.Extensions.*` et leurs dépendances transitives ;
- les annotations de nullabilité et la surface publique ;
- les surcharges récentes de `HttpClient`/`HttpContent` avec `CancellationToken` ;
- les APIs génériques d'`Enum` et autres helpers apparus après .NET Standard ;
- les records et le support de compilation requis ;
- la cohérence des comportements JSON entre actifs.

Les adaptations concrètes déjà identifiées pour un prototype comprennent : `IsExternalInit`, `CallerArgumentExpressionAttribute`, `ArgumentNullException.ThrowIfNull`, `Enum.GetValues<T>()`, `Dictionary.TryAdd`, `HttpMethod.Patch`, `ReadAsStringAsync(CancellationToken)`, certaines surcharges de chaînes et une référence explicite à `System.Text.Json`. Cette liste doit être confirmée par compilation ; elle n'est pas une autorisation de polyfiller la surface publique.

## Options étudiées avant la décision

### Option A — Conserver les cinq cibles actuelles

```text
net6.0;net7.0;net8.0;net9.0;net10.0
```

**Portée.** Préserve les actifs NuGet actuels et limite le changement de sélection des assets.

**Coûts/risques.**

- .NET 6 et 7 sont hors support ;
- cinq builds, jeux de dépendances et exécutions de test ;
- promesse de maintenance à définir pour les vulnérabilités sans runtime supporté ;
- pas de .NET Framework.

### Option B — Large portée et actifs modernes

```text
netstandard2.0;net8.0;net10.0
```

**Portée.** `netstandard2.0` fournit le repli large ; `net8.0` et `net10.0` permettent des adaptations modernes. Un projet `net9.0` devrait sélectionner l'actif compatible le plus approprié selon les règles NuGet, à prouver avec le package construit.

**Coûts/risques.**

- portage du code et versions de dépendances ;
- .NET Framework nécessite des projets consommateurs réels, pas seulement la compilation de la bibliothèque ;
- mêmes APIs publiques à conserver entre cibles ;
- la politique de support de `net8.0` doit être revue le 10 novembre 2026.

### Option C — Runtimes encore supportés à la date de l'audit

```text
net8.0;net9.0;net10.0
```

**Portée.** Retire les deux runtimes déjà hors support tout en gardant les trois actifs courants.

**Coûts/risques.**

- modification de la compatibilité de package à évaluer contre `1.0.0` ;
- .NET 8 et 9 quittent le support trois mois après la date de l'audit ;
- pas de .NET Framework ni d'autres implémentations via .NET Standard.

### Option D — Repli large, Framework explicite et LTS moderne

Exemple à évaluer, pas une recommandation :

```text
netstandard2.0;net462;net10.0
```

**Portée.** Un actif Framework explicite peut traiter les différences propres à .NET Framework, tandis que .NET Standard sert d'actif portable et .NET 10 d'actif moderne.

**Coûts/risques.**

- `net462` arrive en fin de support Microsoft en janvier 2027 ;
- complexité des conditions et des références ;
- l'actif `net462` n'est utile que si le projet assume réellement son test et son support ;
- autre cible Framework, telle que `net472`, possible uniquement sur décision explicite.

### Option E — Matrice personnalisée — retenue

Le propriétaire a retenu la matrice `netstandard2.0` plus chaque actif .NET 6 à .NET 10 afin de maximiser la portée tout en conservant les assets déjà publiés.

## Méthode de validation avant implémentation

Un prototype doit prouver la faisabilité de la matrice retenue avant son intégration. Il produit :

1. résultat de `restore`, `build`, tests et `pack` ;
2. dépendances résolues par framework ;
3. rapport de validation du package par rapport à `1.0.0` ;
4. différence de surface publique par actif ;
5. taille et contenu du `.nupkg` ;
6. liste des adaptations conditionnelles nécessaires ;
7. résultats des projets consommateurs ci-dessous ;
8. coût CI et durée de maintenance estimés.

L'expérimentation se fait sur une branche/worktree dédiée. Une impossibilité technique ou une dépendance incompatible rouvre DEC-002 au lieu de réduire silencieusement la matrice.

## Validation du package

Le SDK doit activer la validation de package du SDK .NET dans le lot de sécurité release, sous réserve de compatibilité avec les outils retenus :

```xml
<EnablePackageValidation>true</EnablePackageValidation>
<PackageValidationBaselineVersion>1.0.0</PackageValidationBaselineVersion>
```

Cette validation peut détecter notamment :

- changements binaires incompatibles ;
- suppression d'un framework cible ;
- différences incohérentes de surface entre actifs ;
- trous d'applicabilité dans le package.

Un fichier de suppressions ne doit pas devenir un moyen de faire passer la CI. Chaque suppression doit comporter une justification, être liée à DEC-001 et apparaître dans les notes de migration.

La baseline doit être le package NuGet réellement publié, pas uniquement l'assembly construit depuis le tag local.

## Matrice de projets consommateurs

Les tests doivent installer le `.nupkg` depuis un dossier local isolé. Une référence de projet ne valide ni les actifs NuGet, ni les dépendances déclarées.

### Consommateurs minimaux

Pour la matrice retenue :

- application console pour chaque TFM explicitement supporté ;
- projet `net9.0` lorsqu'il doit consommer un actif de repli ;
- projets .NET Framework `net472`, `net48` et `net481` sous Windows ;
- consommateurs de compatibilité non bloquants pour .NET Core 3.1 et .NET 5, afin de contrôler le repli `netstandard2.0` sans annoncer leurs runtimes comme supportés ;
- application ASP.NET Core avec injection de dépendances ;
- projet de bibliothèque qui expose un type TorBoxSDK dans sa propre API publique ;
- projet avec `PublishTrimmed=true` ou Native AOT uniquement si DEC-012 le demande.

### Scénarios de fumée

Chaque consommateur doit au minimum :

- restaurer sans source publique accidentelle du package testé ;
- construire avec avertissements traités selon la politique ;
- instancier le client principal avec et sans injection de dépendances ;
- sérialiser une requête représentative ;
- désérialiser une fixture représentative ;
- exercer une annulation ;
- vérifier la sélection de l'asset et les versions transitives.

## Cohérence de surface entre frameworks

Les directives conditionnelles doivent être limitées à l'implémentation. Sauf décision contraire, un consommateur ne doit pas voir disparaître une méthode parce qu'il cible un autre actif du même package.

Contrôles requis :

- validation de package entre les actifs ;
- génération d'une liste d'API publique par TFM ;
- comparaison de nullabilité et valeurs par défaut ;
- mêmes noms JSON et mêmes conversions d'enum ;
- tests de comportement pour les polyfills ou chemins conditionnels.

## Dépendances

Pour chaque TFM candidat, produire un tableau verrouillé comprenant :

- dépendance directe et version ;
- plage réellement inscrite dans le `.nuspec` ;
- support du TFM par le package ;
- dépendances transitives importantes ;
- avis de sécurité ;
- date de fin de support de la ligne majeure.

Éviter d'aligner une ancienne cible sur une version moderne par simple hypothèse. Inversement, ne pas conserver une ancienne ligne de packages sans vérifier sa maintenance et ses correctifs.

Le plancher actuel mérite une décision spécifique : `net8.0` et `net9.0` référencent la ligne `Microsoft.Extensions.* 10.0.6`. Une application .NET 8 utilisant la ligne 8 peut donc résoudre la ligne 10 à cause du SDK. Les politiques à comparer dans DEC-013 sont : minimum commun compatible, alignement par TFM, ou ligne récente unique.

Séparer le cœur HTTP de l'intégration DI/configuration pourrait réduire ces dépendances pour les consommateurs qui n'utilisent pas `IHttpClientFactory`. Ce découpage change toutefois l'organisation NuGet et la migration ; il ne doit pas être engagé sans DEC-007 et DEC-013.

## Politique de support à publier

La documentation publique de la 2.0 doit publier la matrice validée sous une forme de ce type :

| Environnement | Compatible | Testé en CI | Supporté par TorBoxSDK | Support Microsoft | Révision prévue |
|---|---|---|---|---|---|
| à renseigner | oui/non | oui/non | oui/non | état/date | date |

Elle doit aussi préciser :

- délai de retrait après fin de support Microsoft ;
- version majeure ou mineure requise pour retirer un TFM ;
- niveau d'effort pour .NET Framework ;
- versions du SDK utilisées pour construire ;
- fréquence de révision, au minimum à chaque release .NET et avant chaque publication TorBoxSDK.

## Critères d'acceptation

- DEC-002 est validée et la matrice publique est rédigée ;
- le package se construit de zéro pour toutes les cibles ;
- les tests fonctionnels pertinents s'exécutent sur chaque actif annoncé ;
- la validation contre `TorBoxSDK 1.0.0` est verte ou chaque différence est approuvée ;
- les projets consommateurs installent uniquement l'artefact candidat ;
- la sélection NuGet des actifs correspond à la documentation ;
- la surface publique est cohérente ;
- aucune cible n'est présentée comme supportée sans CI ni politique de correction ;
- le calendrier de novembre 2026 est explicitement traité.

## Sources officielles

- [Politique de support .NET](https://dotnet.microsoft.com/en-us/platform/support/policy)
- [Politique .NET et .NET Core](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
- [.NET Standard](https://learn.microsoft.com/dotnet/standard/net-standard)
- [Ciblage multiplateforme des bibliothèques](https://learn.microsoft.com/dotnet/standard/library-guidance/cross-platform-targeting)
- [Politique de support .NET Framework](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-framework)
- [Vue d'ensemble de la validation de package](https://learn.microsoft.com/dotnet/fundamentals/apicompat/package-validation/overview)
- [Validation par baseline](https://learn.microsoft.com/dotnet/fundamentals/apicompat/package-validation/baseline-version-validator)
- [Règles de compatibilité NuGet](https://learn.microsoft.com/dotnet/standard/library-guidance/nuget-package-compatibility-rules)
