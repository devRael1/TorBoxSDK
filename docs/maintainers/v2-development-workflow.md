# Développement local de TorBoxSDK v2

Ce guide fixe l'organisation locale du chantier v2. Il applique la méthode
d'intégration validée dans
[DEC-018](decisions.md#dec-018--politique-git-tags-et-worktrees), sans anticiper
les choix de tags et de release encore ouverts. Il ne donne aucune autorisation
de pousser, taguer ou publier.

## État initial

| Dossier | Branche | Rôle |
|---|---|---|
| `TorBoxSDK` | `master` | Référence v1 et état utilisateur à préserver |
| `TorBoxSDK-docs-modernization` | `codex/sdk-modernization-plan` | Historique du plan de modernisation |
| `TorBoxSDK-v2` | `v2.0.0` | Branche locale d'intégration du chantier v2 |
| `TorBoxSDK-v2-program-control` | `codex/v2-program-control` | Contrôle de programme et suivi Kanban |

La branche `v2.0.0` part du commit `67e7253`, qui contient le plan v2, les
décisions déjà rendues et le référentiel initial des divergences API. Aucun
tag n'est associé à cette branche : son nom ne constitue pas une version
NuGet publiable.

Le dossier `.codex/` non suivi présent dans le worktree `master` est un état
local de l'utilisateur. Il ne doit être ni copié, ni ajouté à Git, ni supprimé
par les travaux v2.

## Règles de branchement

- `v2.0.0` sert de point de coordination local pour la v2.
- Toute modification fonctionnelle est réalisée dans une branche focalisée
  et un worktree dédié, créés depuis un commit identifié de `v2.0.0`.
- Les branches créées par Codex utilisent le préfixe `codex/v2-`, suivi d'un
  objet court, par exemple `codex/v2-release-safety`.
- Une branche ne traite qu'un seul lot : sécurité de publication, contrat API,
  transport HTTP, modèles, couverture d'API, compatibilité .NET ou tests.
- Les fichiers transversaux, notamment le projet SDK, la sérialisation JSON et
  les workflows CI, ne sont pas modifiés en parallèle sans coordination.
- Aucune branche ou worktree n'est supprimé tant que son état n'est pas propre
  et que son travail n'est pas intégré ou explicitement abandonné.

L'intégration normale suit `rebase & merge` : la branche du lot est rebasée sur
le dernier `v2.0.0`, validée à nouveau, puis intégrée par avance rapide en
local ou par **Rebase and merge** sur GitHub. Si cette méthode n'est pas
possible, un `merge commit` est autorisé à condition d'enregistrer la cause
dans la carte du lot. Le squash n'est pas utilisé comme méthode normale.

Le [Kanban privé v2](v2-program-control.md) est la source de vérité pour
l'état d'exécution, les agents, les branches et les preuves. Ce dépôt reste la
source de vérité pour le contrat, les décisions et les règles techniques.

## Création d'un lot de travail

Depuis le dépôt principal, vérifier d'abord les worktrees et l'état de la
branche v2 :

```powershell
git worktree list
git -C "D:\Bot discord\TorBoxSDK-v2" status --short --branch
```

Après avoir remplacé `<lot>` et `<dossier>` par des valeurs explicites :

```powershell
git -C "D:\Bot discord\TorBoxSDK" worktree add `
  -b "codex/v2-<lot>" `
  "D:\Bot discord\<dossier>" `
  v2.0.0
```

Avant toute modification, enregistrer le commit de départ dans le compte
rendu du lot :

```powershell
git -C "D:\Bot discord\<dossier>" rev-parse HEAD
git -C "D:\Bot discord\<dossier>" status --short --branch
```

## Discipline des commits

Un commit doit être petit, cohérent et vérifiable indépendamment. Son message
décrit le résultat et utilise un préfixe stable, par exemple :

```text
build: sécuriser la version du package
feat: exposer les statistiques utilisateur
fix: aligner la réponse Relay inactive
test: figer le contrat OpenAPI principal
docs: documenter les restrictions de Search API
```

Avant chaque commit :

1. examiner `git status --short` et `git diff` ;
2. vérifier que seuls les fichiers du lot sont inclus ;
3. exécuter `git diff --check` ;
4. lancer les validations proportionnées au changement ;
5. ajouter explicitement les fichiers concernés, sans wildcard de publication
   ni ajout global non vérifié ;
6. contrôler une dernière fois `git diff --cached` avant le commit.

Les secrets, clés TorBox, fichiers de résultats locaux et artefacts `bin/` ou
`obj/` ne sont jamais commités.

## Contrôles locaux minimaux

Les commandes suivantes constituent le socle d'un lot qui touche le code ou
la configuration de compilation :

```powershell
dotnet restore TorBoxSDK.slnx
dotnet build TorBoxSDK.slnx -c Release --no-restore
dotnet test tests/TorboxSDK.UnitTests/ -c Release --no-build
dotnet docfx docs/docfx.json --warningsAsErrors
```

Les tests de contrat, d'intégration et live sont ajoutés selon le risque du
lot. Une absence de `TORBOX_API_KEY` doit produire un saut explicite, jamais un
succès simulé. Aucun test destructif ou consommateur de quota n'est lancé sans
autorisation et données de test dédiées.

Une modification de la matrice .NET doit en plus construire et tester chaque
TFM déclaré selon la stratégie de
[compatibilité .NET](dotnet-compatibility.md). Une modification de contrat API
doit mettre à jour les instantanés, fixtures et
[divergences observées](api-divergences.md).

## Conditions avant intégration

Le lot est prêt à être présenté pour intégration lorsque :

- son worktree est propre après commit ;
- les changements restent bornés à la responsabilité annoncée ;
- le build, les tests applicables et DocFX réussissent ;
- la documentation XML et les guides reflètent toute évolution publique ;
- le rapport précise le commit de départ, les commandes exécutées, leurs
  résultats et les contrôles non exécutés ;
- aucune décision ouverte n'a été prise implicitement dans le code.

Avant intégration, la branche est rebasée sur le dernier `v2.0.0` puis toutes
les validations applicables sont relancées. Après l'avance rapide ou le merge
commit de repli, les validations finales sont encore relancées sur `v2.0.0` :
un résultat obtenu uniquement sur la branche du lot n'est pas une preuve
suffisante.

## Publication et tags

- Ne jamais créer de tag depuis un worktree de lot.
- Ne jamais utiliser `dotnet nuget push` dans le chantier local.
- Ne jamais déduire une version de package du seul nom `v2.0.0`.
- Ne jamais republier un artefact reconstruit après validation.
- Attendre la sécurisation du pipeline, la décision SemVer, le canal candidat
  et l'approbation de release avant toute publication.

Le processus complet est défini dans
[Tests et publication NuGet](testing-and-release.md).

## Premier ordre de travail

Le premier lot fonctionnel doit sécuriser la construction et la publication
avant toute extension de l'API publique. L'ordre de dépendance reste :

1. reproductibilité de la version et de l'artefact NuGet ;
2. instantanés OpenAPI/Postman et détection des divergences ;
3. corrections du transport HTTP et des modèles existants ;
4. couverture des opérations manquantes ;
5. matrice .NET élargie et validation du package ;
6. release candidate, tests live ciblés, puis publication approuvée.
