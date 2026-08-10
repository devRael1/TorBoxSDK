# Pilotage Kanban de TorBoxSDK v2

Le chantier est piloté dans le projet GitHub privé
[TorBoxSDK v2.0.0](https://github.com/users/devRael1/projects/14), lié au dépôt
`devRael1/TorBoxSDK`. Une personne sans accès au projet GitHub ne peut pas
consulter ses cartes.

Le projet GitHub est la source de vérité pour l'exécution : état, agent,
reviewer, branche, worktree, dépendances et preuves. Les documents du dépôt
restent la source de vérité pour les décisions, le contrat API, la politique
.NET, les divergences et les critères de release.

## Structure créée

Le projet contient 17 cartes privées, une par lot v2. Ce sont des brouillons
du projet et non des issues publiques du dépôt.

Champs disponibles :

- `Status` ;
- `Phase` ;
- `Type de travail` ;
- `Priorité` ;
- `Risque` ;
- `Agent` et `Agent de revue` ;
- `Branche` et `Worktree` ;
- `Commit de base` ;
- `Dépendances` et `Décisions` ;
- `Preuves`.

Les phases couvrent le programme, la sécurité de release, le contrat, la
génération, le transport et les modèles, la couverture API, la compatibilité
.NET, la documentation et la release candidate.

## Vue Kanban à finaliser dans GitHub

Le CLI GitHub permet de créer le projet, ses champs et ses cartes, mais ne
permet pas de configurer complètement ses vues. Un propriétaire du projet doit
effectuer une fois les opérations suivantes dans l'interface :

1. ouvrir le [projet v2](https://github.com/users/devRael1/projects/14) ;
2. renommer la vue initiale en `Backlog` et la conserver en disposition Table ;
3. créer une vue avec **New view**, choisir **Board**, la nommer `Kanban` et la
   grouper par `Status` ;
4. ouvrir les paramètres du champ `Status`, renommer `Todo` en `Backlog`,
   conserver `In Progress`, renommer `Done` en `Integrated`, puis ajouter
   `Ready`, `Needs decision`, `In review` et `Validated` ;
5. créer une vue Table `Décisions` filtrée sur les cartes dont `Décisions`
   n'est pas vide et dont `Status` n'est pas `Integrated` ;
6. créer une vue Table `Risques` filtrée sur `Risque` égal à `Élevé` ou
   `Critique` ;
7. créer une vue Table `Par phase`, groupée par `Phase` puis triée par
   `Priorité` ;
8. dans **Settings > Workflows**, activer l'ajout automatique seulement si le
   filtre peut être limité aux PR de branches `codex/v2-*`. Ne pas ajouter
   automatiquement toutes les issues du dépôt public.

Les cartes existantes restent en `Backlog` après le renommage de `Todo`.

## Cycle d'une carte

| Status | Condition |
|---|---|
| `Backlog` | lot identifié mais non prêt |
| `Ready` | dépendances intégrées, décisions rendues, critères définis |
| `In Progress` | agent et worktree affectés, développement en cours |
| `Needs decision` | choix du propriétaire requis avant de poursuivre |
| `In review` | commits terminés et remis à un agent de revue indépendant |
| `Validated` | revue acceptée et contrôles réussis après rebase |
| `Integrated` | intégré dans `v2.0.0` et contrôles finaux réussis |

Une carte ne passe jamais directement de `In Progress` à `Integrated`.

## Affectation d'un agent

Avant de démarrer un lot, le coordinateur renseigne :

- `Agent` et `Agent de revue` avec deux agents distincts ;
- `Branche`, `Worktree` et `Commit de base` ;
- les décisions encore ouvertes ;
- les critères d'acceptation dans le corps de la carte ;
- `Status = In Progress` seulement après ces contrôles.

Un agent ne travaille que sur la carte et la branche qui lui sont affectées.
Les modifications hors périmètre sont signalées au coordinateur et ne sont
pas ajoutées implicitement au lot.

## Handoff et preuves

À la fin du développement, l'agent renseigne dans `Preuves` ou dans le corps
de la carte :

- commit de départ et commit final ;
- fichiers ou zones modifiés ;
- commandes de build et de test exécutées ;
- résultats et contrôles non exécutés ;
- risques, divergences et décisions restantes ;
- URL de la PR quand la branche est publiée.

L'agent de revue applique le skill `code-review`. Une carte ne devient
`Validated` qu'avec un verdict sans problème critique ou majeur, un worktree
propre et des contrôles réussis après rebase sur le dernier `v2.0.0`.

## Intégration

La méthode normale est `rebase & merge`. En local :

1. rebaser la branche du lot sur le dernier `v2.0.0` ;
2. résoudre les conflits dans la branche du lot ;
3. relancer les contrôles ;
4. avancer `v2.0.0` avec `git merge --ff-only` ;
5. relancer les contrôles sur `v2.0.0` ;
6. renseigner le commit intégré et passer la carte à `Integrated`.

Si le rebase n'est pas possible, un merge commit est autorisé en repli. La
raison doit être enregistrée dans la carte. Aucun squash implicite n'est
utilisé.

## Ordre de démarrage

Les trois premières cartes sont :

1. `V2-000` — contrôle de programme ;
2. `V2-010` — intégration des agents et skills dans la v2 ;
3. `V2-100` et `V2-110` — sécurité NuGet et contrat reproductible, en parallèle
   seulement après intégration des deux premières cartes.
