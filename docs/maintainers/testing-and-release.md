# Stratégie de tests et publication NuGet

> Aucune commande de publication ne doit être exécutée dans le cadre du chantier local sans validation explicite. V2-100 applique [DEC-009](decisions.md#dec-009--canal-de-release-candidate), [DEC-015](decisions.md#dec-015--source-de-version-et-assemblyversion) et [DEC-016](decisions.md#dec-016--authentification-et-approbation-nuget) ; V2-210 met en œuvre la portée déterministe déjà validée de DEC-017 sans l'étendre. DEC-010 reste nécessaire avant une release finale.

## Diagnostic de la chaîne actuelle

### Publication

Le flux actuel :

1. se déclenche pour tout tag correspondant à `v*` ;
2. restaure, construit et empaquette ;
3. pousse immédiatement les fichiers trouvés vers NuGet avec une wildcard et `--skip-duplicate` ;
4. n'exécute aucun test ;
5. ne compare pas le tag à la version inscrite dans le package.

`Directory.Build.props` fixe `Version` à `1.0.0`. Ainsi, un futur tag `v1.1.0` ne garantit pas la création d'un package `1.1.0`. `--skip-duplicate` peut ensuite transformer cette incohérence en job apparemment réussi.

Autres constats au moment de l'audit :

- la CI générale se déclenche seulement manuellement ;
- la protection de `master` demande une approbation, mais aucun status check obligatoire ;
- le contournement administrateur est permis ;
- aucun environnement GitHub `release` protégé n'est configuré ;
- le secret NuGet est un secret de dépôt de longue durée ;
- le tag `v1.0.0` est léger et non signé ;
- l'artefact est reconstruit dans le job de publication au lieu de promouvoir l'artefact déjà testé.
- aucun `global.json`, lock file NuGet, contrôle de vulnérabilités, seuil de couverture ou validation API/package n'est actuellement imposé ;
- les fichiers de résultats de test portent le même nom sur cinq TFMs et peuvent se remplacer dans les artefacts.

### Historique de preuve de `1.0.0`

La dernière CI normale réussie observée concernait le commit `57395e4` du 18 avril 2026. Le tag `v1.0.0` pointe sur `fd2acda`, qui comporte un ensemble important de changements après ce commit. Les tests live ont échoué le 20 avril, avant la publication du 24 avril.

Cela ne prouve pas que `1.0.0` est défectueux. Cela prouve que l'historique actuel ne permet pas de démontrer que l'artefact publié correspond à un état intégralement validé.

### Métadonnées du package publié

Points positifs observés :

- cinq actifs `net6.0` à `net10.0` présents ;
- package de symboles présent ;
- métadonnées Repository/SourceLink et commit présentes.

Points à corriger ou décider :

- le `.nuspec` publié n'expose ni licence ni tags, car les propriétés de projet utilisées ne sont pas les propriétés NuGet conventionnelles (`PackageLicenseExpression`, `PackageTags`) ;
- pas de notes de version ni d'icône de package ;
- le contenu public du package doit être inspecté automatiquement à chaque candidat.

Ce constat a été vérifié directement dans le `.nuspec` du package `1.0.0` téléchargé depuis NuGet. La présence du fichier `LICENSE` dans le dépôt ou l'affichage d'informations sur le site ne remplace pas une métadonnée de licence dans le package.

## Objectif de la nouvelle chaîne

Un seul artefact immuable doit traverser la chaîne :

```mermaid
flowchart LR
    C["Commit revu"] --> B["Build reproductible"]
    B --> T["Tous les tests"]
    T --> K["Pack une seule fois"]
    K --> V["Validation du nupkg"]
    V --> A["Approbation humaine"]
    A --> N["Push de cet artefact exact"]
    N --> S["Smoke test NuGet"]
```

Le job de publication ne doit jamais reconstruire. Il télécharge l'artefact testé, vérifie son empreinte, sa version et sa provenance, puis le pousse après approbation.

## Chaîne appliquée par V2-100 et V2-210

- `ci.yml` s'exécute sur pull request, sur l'intégration `v2.0.0` et manuellement. Il vérifie la génération interne, restaure en mode verrouillé, construit, puis exécute les tests unitaires déterministes (dont sérialisation et transport simulé) et les tests de schéma `Category=Contract` pour chaque TFM déclaré. Il packe ensuite un unique artefact de validation local : `EnablePackageValidation` le compare à `TorBoxSDK 1.0.0`, puis `Test-NuGetReleaseArtifact.ps1` vérifie les paquets `.nupkg`/`.snupkg`, leurs actifs, XML, PDB, versions, commit de provenance et empreintes. Les TRX, paquets et manifeste restent dans l'artefact CI pendant 14 jours ; ce ne sont pas des candidats de release.
- `publish.yml` est déclenché par un tag `v*`, mais refuse tout tag qui n'est pas exactement `v2.minor.patch` avec un suffixe de préversion NuGet facultatif, ou qui ne cible pas un commit atteignable depuis `v2.0.0`. Le job `candidate` construit, teste et empaquette une seule fois, génère le manifeste et les SHA-256, puis conserve le tout comme artefact interne pendant 14 jours.
- Le job `publish` ne reçoit que cet artefact : il vérifie que le tag pointe toujours vers le commit candidat, recalcule les hashes, contrôle le manifeste et pousse les chemins complets des deux fichiers. Il ne contient ni `dotnet build`, ni `dotnet pack`, wildcard ou `--skip-duplicate`.
- Les tags de préversion restent des candidates locales : le job de publication stable est ignoré. Un tag stable attend l'environnement `release` avant d'obtenir un jeton OIDC NuGet temporaire. Le propriétaire conserve explicitement le bypass administrateur GitHub ; toute utilisation est une dérogation à documenter dans le rapport de release.

### Préconfiguration administrative obligatoire

Avant d'autoriser un tag stable, le propriétaire configure les deux protections externes suivantes :

1. Dans NuGet.org, créer une politique **Trusted Publishing** détenue par `devRael1`, limitée au dépôt `devRael1/TorBoxSDK`, au fichier `publish.yml` et à l'environnement `release`.
2. Dans GitHub, créer l'environnement `release`, ajouter `devRael1` comme approbateur requis et limiter les déploiements aux tags `v2.*`. Configurer ensuite la règle de branche de `v2.0.0` pour exiger le check `CI / Verify` et une pull request revue avant intégration.

La politique NuGet n'utilise aucune clé API de longue durée. Le job OIDC demande seulement `id-token: write` au moment de la promotion ; les jobs de CI et de candidate restent en lecture seule.

### Dépendance transitoire V2-110

Sur le commit de base de V2-100, les tests de schéma lisent encore une spécification OpenAPI distante et peuvent donc échouer sur une variante amont. V2-110 fournit le baseline versionné qui rend ce test reproductible. V2-210 sélectionne explicitement `Category=Contract`, seule suite de schéma hors ligne : ce filtre de frontière ne saute aucun test de contrat et ne masque aucun échec. La preuve finale de CI est relancée après l'intégration de V2-110 et le rebase de V2-100.

## Pyramide de tests

### Niveau 1 — Analyse et compilation

À chaque pull request :

- restauration verrouillée ou contrôlée ;
- compilation Release de chaque TFM retenu ;
- avertissements et analyseurs selon une politique documentée ;
- vérification du format et de la documentation XML ;
- recherche de secrets et dépendances vulnérables ;
- validation des liens internes DocFX et génération de la documentation.

### Niveau 2 — Tests unitaires

Portée : validation, options, sérialisation locale, construction d'URI, gestion des erreurs, annulation, DI et durée de vie des clients.

Exigences :

- aucun réseau ;
- horloge, aléatoire et environnement maîtrisés ;
- cas limites de nullabilité, collections vides, enums inconnues et Unicode ;
- exécution sur chaque actif dont l'implémentation conditionnelle diffère.

Les 491 tests réussis constituent une base, mais leur réussite ne remplace pas les niveaux suivants.

### Niveau 3 — Tests du protocole HTTP

Un faux gestionnaire HTTP capture chaque requête. Pour chaque méthode publique, vérifier :

- verbe, base URL, version et chemin ;
- paramètres de chemin et query string ;
- authentification et jetons de téléchargement ;
- type de contenu et corps exact ;
- multipart, formulaire et JSON ;
- redirections et réponse binaire ;
- erreurs HTTP, annulation et libération des flux.

Ce niveau doit détecter les écarts Vendors, Web Downloads et Integration déjà identifiés.

### Niveau 4 — Tests de contrat reproductibles

Ils utilisent des instantanés OpenAPI approuvés, jamais la version distante téléchargée pendant le job. Ils vérifient :

- couverture des opérations ;
- paramètres, emplacement, obligation et format ;
- types de contenu ;
- schémas de requête ;
- classification documentée de chaque opération non exposée.

La [baseline de contrat V2-110](contract-baseline.md) met en œuvre cette
séparation avec la catégorie offline `Contract`. Son monitor distant est
manuel et opt-in : une dérive produit un rapport de revue, mais ne change ni
le code ni les snapshots. V2-110 n'ajoute aucun job, calendrier ou gate CI ;
ces choix restent soumis à DEC-017.

### Niveau 5 — Fixtures de réponse

Ce niveau est conditionnel à DEC-011 et ne fait pas partie de V2-110. Si des
fixtures synthétiques ou anonymisées sont ultérieurement autorisées, elles
pourront valider :

- modèles minimaux et complets ;
- objet contre collection ;
- champs absents, nuls et inconnus ;
- nouvelles valeurs d'enum ;
- enveloppes d'erreur ;
- binaire, PDF et redirection ;
- modèles Search/Relay séparément du Main.

### Niveau 6 — Validation API et package

- validation de la surface publique et du package contre `TorBoxSDK 1.0.0` ;
- cohérence entre actifs TFM ;
- inspection du `.nuspec`, DLL, XML, PDB, SourceLink et dépendances ;
- absence de fichiers de test, secrets, snapshots bruts sensibles ou artefacts inattendus ;
- correspondance exacte entre version attendue, assembly, `.nuspec`, nom de fichier et éventuel tag.

### Niveau 7 — Projets consommateurs

Des projets temporaires installent le `.nupkg` depuis une source locale vide. Ils couvrent chaque environnement retenu dans DEC-002, l'injection de dépendances, le client autonome et les scénarios de sérialisation.

Ils doivent interdire le repli accidentel sur une version de NuGet.org afin de prouver que le candidat est réellement consommé.

### Niveau 8 — Tests live

Deux catégories :

- **lecture seule**, pouvant être exécutée avec un compte dédié si DEC-017
  définit l'autorisation et le mode d'exécution ;
- **mutante**, seulement manuelle, avec ressources isolées et nettoyage contrôlé.

Chaque test live déclare : permissions, coût potentiel, données créées, stratégie de nettoyage et codes de réponse acceptés. Un `403` ne doit pas être traité comme preuve de conformité du modèle.

Les secrets ne sont jamais imprimés. Si DEC-011 autorise ultérieurement des
fixtures de réponse, les réponses capturées doivent passer par anonymisation
avant d'en devenir.

### Niveau 9 — Tests post-publication

Après disponibilité sur NuGet :

- vérifier identité, version, hash et métadonnées ;
- installer depuis NuGet.org dans un projet vierge ;
- compiler au moins un consommateur représentatif ;
- vérifier SourceLink et symboles ;
- tester un appel public sans mutation et la gestion d'une erreur contrôlée ;
- archiver le rapport de publication.

## Organisation CI proposée

Les noms ci-dessous sont illustratifs et ne constituent ni une décision sur la
plateforme ni un planning d'exécution. V2-110 n'ajoute ni workflow, ni
déclencheur, ni gate : DEC-017 doit d'abord définir le périmètre CI, les
autorisations live et une éventuelle cadence.

### `ci.yml`

Déclenchement sur pull request et intégration. Exécute la restauration verrouillée, le build Release, les tests unitaires sans catégories `Live` ou `Integration`, puis les seuls tests de schéma `Category=Contract` pour chaque TFM déclaré. Il packe et valide aussi un artefact local contre la baseline API/package `1.0.0`, sans secret TorBox ni jeton NuGet. Les tests consommateurs restent du ressort du lot qui les introduira.

### Gate local

Depuis la racine du dépôt :

```powershell
pwsh ./eng/Invoke-DeterministicChecks.ps1 -ResultsDirectory artifacts/test-results
```

Cette commande ne crée ni tag ni publication. Elle produit les TRX par projet et TFM, un `.nupkg`, un `.snupkg` et `package-manifest.json` sous `package-validation/<version>` dans le répertoire de résultats ; cette isolation rend les relances avec des versions différentes indépendantes. Le job `candidate` utilise `-SkipPackageValidation`, puis crée et valide son unique package depuis le tag exact ; il évite ainsi un second `dotnet pack`.

Le check `CI / Verify` devient obligatoire sur `v2.0.0` après le premier run vert incluant le baseline de V2-110.

### `contract-watch.yml`

Scénario possible seulement après décision DEC-017. V2-110 fournit aujourd'hui
un monitor local manuel et opt-in qui compare les sources capturées à la
baseline et produit un rapport hors de celle-ci ; il ne pousse ni snapshot ni
changement de code.

### `live-readonly.yml`

Scénario possible seulement après décision DEC-017. Une éventuelle suite live
de lecture devra alors définir son environnement protégé, son compte dédié, ses
autorisations et le traitement de l'indisponibilité externe.

### `publish.yml`

Le job `candidate` du workflow produit l'artefact unique depuis le tag exact. Le job `publish` télécharge ce candidat, vérifie commit, hash et version, attend l'approbation de l'environnement `release`, puis pousse des chemins de fichiers exacts. Aucune wildcard et aucune reconstruction.

Le comportement face à un package déjà existant est un échec explicite, sauf reprise vérifiée : si le push du `.nupkg` échoue, le job télécharge le package NuGet portant le même id/version et compare son SHA-256 au manifeste candidat. Il ne poursuit vers le `.snupkg` que si les hashes sont identiques ; une absence, une indisponibilité ou une différence échoue. `--skip-duplicate` n'est jamais utilisé.

L'authentification relève de DEC-016 : Trusted Publishing/OIDC fournit un jeton temporaire au seul job de promotion. Aucune clé API NuGet de longue durée n'est admise dans le dépôt, les secrets GitHub ou le workflow.

## Source unique de version

Une seule mécanique doit être choisie et documentée, par exemple :

- version donnée au pipeline candidat, vérifiée puis utilisée par `dotnet pack -p:Version=...` ;
- version dérivée d'un tag strict après que le candidat a été approuvé ;
- outil de versionnement Git configuré et verrouillé.

V2-100 retient le tag strict `v2.<minor>.<patch>` comme source unique. Les invariants sont :

- format SemVer validé ;
- tag éventuel exactement égal à `v` + version du package ;
- version d'assembly et d'information cohérentes selon la politique ;
- aucun fichier du dépôt ne peut réintroduire silencieusement `1.0.0` comme version de release ;
- réexécuter la publication ne fabrique pas un autre binaire.

La publication NuGet de l'archive principale et des symboles n'est pas atomique. Après une interruption entre les deux, relancer le job `publish` échoué du même run est la seule reprise autorisée : il récupère le candidat immuable, prouve que le `.nupkg` déjà visible sur NuGet.org a le hash du manifeste, puis tente exclusivement le `.snupkg` candidat. Une version existante dont le hash diffère reste un incident et bloque la promotion.

La mécanique exacte est validée avec DEC-010.

## Procédure de release candidate

### Préconditions

- décisions applicables validées ;
- changelog et guide de migration préparés ;
- snapshots de contrat approuvés ;
- aucune modification locale non suivie dans le worktree d'intégration ;
- commit signé ou règle d'identité conforme à la politique du projet ;
- version libre sur NuGet.

### Construction

1. Partir du commit d'intégration exact.
2. Restaurer dans un environnement propre.
3. Exécuter les niveaux 1 à 7.
4. Calculer la version depuis la source unique.
5. Construire et packer une seule fois.
6. Valider le package contre `1.0.0`.
7. Inspecter son contenu et ses métadonnées.
8. Installer ce fichier dans la matrice consommateurs.
9. Calculer et publier les hashes dans le rapport candidat.
10. Conserver l'artefact sans le reconstruire.

### Rapport candidat

Le rapport contient :

- commit, version, date et environnement SDK ;
- hash SHA-256 du `.nupkg` et du `.snupkg` ;
- résultats de chaque niveau de test ;
- diff API publique contre `1.0.0` ;
- diff OpenAPI et opérations différées ;
- cibles et dépendances ;
- contenu du package ;
- résultats live et dérogations ;
- notes de migration ;
- approbateurs.

### Promotion

Le canal dépend de DEC-009 : fichier local, prerelease NuGet.org, GitHub Packages ou combinaison. Une promotion en version stable exige une approbation distincte et ne reconstruit pas l'artefact.

## Checklist de publication stable

### Avant l'envoi

- [ ] DEC-001, DEC-002, DEC-009 et DEC-010 validées.
- [ ] Version non utilisée sur NuGet.
- [ ] Commit d'intégration protégé et identifié.
- [ ] Tous les checks obligatoires réussis sur ce commit.
- [ ] Rapport de compatibilité accepté.
- [ ] Tests live requis réussis ou dérogation signée.
- [ ] `.nupkg` et `.snupkg` inspectés.
- [ ] Licence, tags, repository, commit, symboles et notes présents.
- [ ] Documentation et migration publiables.
- [ ] Hashes du candidat approuvés.
- [ ] Aucun secret dans les logs ou artefacts.

### Envoi

- [ ] Approbation de l'environnement release.
- [ ] Version du package égale à la version approuvée.
- [ ] Artefact égal au hash approuvé.
- [ ] Chemin exact du package utilisé, sans wildcard.
- [ ] Résultat NuGet conservé dans le rapport.

### Après l'envoi

- [ ] Version visible et métadonnées correctes.
- [ ] Installation depuis NuGet.org réussie.
- [ ] Smoke tests consommateurs réussis.
- [ ] Symboles et SourceLink vérifiés.
- [ ] Release GitHub et changelog liés au bon tag.
- [ ] Surveillance renforcée activée pour la fenêtre décidée.

## Retour après incident NuGet

NuGet ne permet pas de remplacer une version publiée. En cas de problème :

1. arrêter toute promotion ou annonce automatique ;
2. qualifier l'impact et les frameworks touchés ;
3. déprécier et/ou délister la version selon la gravité et la politique du projet ;
4. publier une information claire avec la version sûre ;
5. préparer un correctif avec un nouveau numéro ;
6. refaire toute la chaîne, sans réutiliser le numéro défectueux ;
7. conserver l'analyse post-incident et ajouter un test empêchant la régression.

Le délistage réduit la découverte mais ne retire pas le package des caches ni des projets qui le référencent déjà. Le plan de retour est donc une nouvelle release, pas l'effacement de l'ancienne.

## Travail local et intégration des worktrees

- Le worktree de documentation ne publie rien et ne crée aucun tag.
- Chaque worktree d'implémentation produit son rapport de test local.
- Le worktree d'intégration reçoit les lots dans l'ordre documenté.
- Les tests finaux sont relancés après intégration ; les résultats d'une branche isolée ne suffisent pas.
- Seul le commit d'intégration approuvé peut devenir candidat.
- Un worktree avec changements non suivis n'est jamais supprimé automatiquement.

## Critères d'acceptation de la chaîne

- aucun push NuGet ne peut se produire avant les tests et l'approbation ;
- le job de publication ne compile ni ne packe ;
- tag, package, assembly et rapport portent une version cohérente ;
- l'artefact publié est byte pour byte celui testé ;
- les checks requis couvrent compilation, contrat, API publique et consommateurs ;
- les tests live sont séparés, autorisés et interprétables ;
- la baseline `1.0.0` vient de NuGet ;
- le package contient les métadonnées décidées ;
- un exercice de retour est documenté avant la version stable.

## Sources officielles

- [Validation de packages .NET](https://learn.microsoft.com/dotnet/fundamentals/apicompat/package-validation/overview)
- [Baseline de validation](https://learn.microsoft.com/dotnet/fundamentals/apicompat/package-validation/baseline-version-validator)
- [Compatibilité des packages NuGet](https://learn.microsoft.com/dotnet/standard/library-guidance/nuget-package-compatibility-rules)
- [Suppression et délistage de packages NuGet](https://learn.microsoft.com/nuget/nuget-org/policies/deleting-packages)
- [Versions de packages NuGet](https://learn.microsoft.com/nuget/concepts/package-versioning)
