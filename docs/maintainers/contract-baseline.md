# Baseline de contrat V2-110

Ce guide s'adresse aux mainteneurs qui vérifient ou mettent à jour le contrat
amont de TorBoxSDK. Il décrit la baseline livrée par V2-110, pas une nouvelle
surface publique du SDK.

> La baseline fige des sources de contrat et leur provenance. Elle ne prouve
> pas les formes de réponse, ne crée aucune fixture de réponse et ne transforme
> pas automatiquement une dérive distante en changement de SDK.

## Autorité et contenu

`contracts/baseline/manifest.json` est l'autorité de la baseline. Chaque
source y possède un identifiant, sa famille, son format et son état :

- une source `captured` référence un artefact brut, versionné dans
  `contracts/baseline/` ;
- une source `unavailable` conserve la raison de l'absence, sans fichier de
  substitution ni contrat inventé.

Pour chaque artefact capturé, le manifeste enregistre au minimum l'URL exacte,
la méthode et les en-têtes de requête nécessaires, l'heure de capture UTC, le
statut HTTP, le type de contenu, `ETag` et `Last-Modified` lorsqu'ils sont
disponibles, la taille en octets et le SHA-256. Les octets versionnés doivent
correspondre à la taille et au hash déclarés ; ne les normalisez pas ni ne les
réécrivez hors de la revue qui met aussi à jour le manifeste.

La baseline `2026-08-10` contient :

| Identifiant | Famille | Format | État | Artefact ou raison |
|---|---|---|---|---|
| `main-openapi` | Main | OpenAPI | capturé | `main.openapi.json` |
| `relay-openapi` | Relay | OpenAPI | capturé | `relay.openapi.json` |
| `main-postman` | Main | collection Postman publique | capturé | `main.postman.collection.json` |
| `search-openapi` | Search | OpenAPI | indisponible | aucun endpoint OpenAPI officiel courant vérifié |
| `search-postman` | Search | collection Postman publique | indisponible | aucune exportation brute vérifiée depuis la page publique |
| `relay-postman` | Relay | collection Postman publique | indisponible | aucune exportation brute vérifiée depuis la page publique |

L'absence d'une source ne signifie ni que l'API est supprimée ni qu'elle doit
être reconstruite à partir d'une autre source. Les URL, observations et raisons
complètes restent dans le manifeste afin d'éviter leur duplication et leur
dérive dans cette page.

`ContractBaselineReader.Load` valide le manifeste, les chemins relatifs, la
taille et le SHA-256 avant de charger les artefacts. Les tests de schéma Main
passent ensuite par `OpenApiSchemaReader.ReadFromBaselineAsync`. Ces helpers
sont une infrastructure interne de test, pas une API TorBoxSDK destinée aux
consommateurs.

## Exécution déterministe hors ligne

La catégorie `Contract` ne contacte pas TorBox et ne demande pas
`TORBOX_API_KEY`. Elle charge uniquement les fichiers versionnés. Après une
restauration et une construction, exécuter par exemple :

```powershell
dotnet restore TorBoxSDK.slnx
dotnet build TorBoxSDK.slnx -c Release --no-restore
dotnet test tests/TorBoxSDK.SchemaValidationTests/TorBoxSDK.SchemaValidationTests.csproj `
  -c Release -f net10.0 --no-build --no-restore --filter "Category=Contract"
```

La baseline V2-110 a obtenu **120/120** tests `Category=Contract` sur
`net10.0`. Ce résultat couvre notamment l'intégrité manifeste/hash et les
comparaisons de contrat déterministes ; il ne constitue pas une validation
live, ni une validation de réponses par fixtures.

## Surveillance distante manuelle

La surveillance distante est un signal de dérive séparé. Elle est explicitement
opt-in et s'exécute uniquement avec une sortie située hors de
`contracts/baseline` :

```powershell
pwsh ./eng/Invoke-ContractMonitor.ps1 `
  -AllowNetwork `
  -OutputPath <chemin-hors-contracts/baseline> `
  [-FailOnDrift]
```

Sans `-AllowNetwork`, le script refuse toute requête réseau. Il consulte
seulement les sources `captured`, produit un rapport nouveau sans écraser un
rapport existant, et refuse une sortie dans `contracts/baseline`. Le rapport
indique les métadonnées observées et la dérive de statut, taille ou SHA-256.
Avec `-FailOnDrift`, il échoue après avoir produit le rapport si une dérive est
détectée.

Le monitor ne modifie jamais un artefact, `manifest.json` ou le code. Il ne
constitue pas un test live de comportement de l'API et n'établit pas le
contrat effectif à lui seul.

## Procédure de mise à jour revue

1. Lancer le monitor volontairement et conserver son rapport hors de la
   baseline. Une dérive déclenche une analyse, pas une copie automatique.
2. Comparer la dérive avec la documentation officielle,
   [les divergences observées](api-divergences.md) et, si nécessaire, les
   preuves live autorisées par DEC-004 et DEC-020.
3. Ouvrir un lot de revue dédié. Si une nouvelle capture est approuvée,
   ajouter ou remplacer l'artefact brut et mettre à jour dans le même diff son
   entrée de manifeste : provenance complète, taille, SHA-256 et disponibilité.
   Une source toujours indisponible reste explicitement `unavailable` avec sa
   raison ; aucun substitut n'est fabriqué.
4. Exécuter la catégorie `Contract` hors ligne et examiner le diff de contrat.
   Un hash valide établit l'intégrité des octets, pas la stabilité fonctionnelle
   d'une route.
5. Consigner une divergence réelle ou son évolution dans le référentiel
   approprié. Évaluer séparément les conséquences sur les modèles, la surface
   publique et la documentation.
6. Faire approuver la revue avant de retenir la nouvelle baseline. Aucun
   téléchargement, rapport de monitor ou changement de snapshot ne modifie
   automatiquement le SDK.

## Décisions et limites

- [DEC-004](decisions.md) et [DIV-001](api-divergences.md) imposent de ne pas
  fusionner les variantes. Le `main-openapi` retenu omet les schémas Search
  Engine observés dans une autre variante ; cette exclusion documente une
  divergence, sans demander de supprimer les modèles SDK ni de construire une
  union théorique.
- [DEC-011](decisions.md) reste ouverte. V2-110 n'ajoute ni fixture de
  réponse, ni règle de capture, d'anonymisation ou de conservation des
  réponses. Les artefacts de la baseline ne sont pas des fixtures.
- [DEC-017](decisions.md) reste ouverte. V2-110 n'introduit aucun workflow,
  calendrier, gate CI, politique de tests live ou monitor planifié. Toute
  automatisation future requiert une décision distincte.

Pour le contexte d'architecture, consulter [Contrat API et modèles](api-contract-and-models.md),
[Tests et publication NuGet](testing-and-release.md) et
[Développement de la v2](v2-development-workflow.md).
