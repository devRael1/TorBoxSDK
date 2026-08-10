# Décisions à valider

Ce registre empêche qu'un choix d'architecture ou de produit soit pris implicitement pendant l'implémentation. Seul le propriétaire du projet peut passer une décision de `En attente` à `Validée`.

Pour valider une décision, renseigner : option choisie, date, auteur, justification et éventuelles conditions. Une option peut aussi être rejetée ou renvoyée en expérimentation.

## Tableau de suivi

| ID | Sujet | État | Bloque |
|---|---|---|---|
| DEC-001 | Compatibilité de l'API publique et version majeure | Validée : 2.0 cassante | modèles, couverture, release |
| DEC-002 | Frameworks .NET ciblés et pris en charge | Validée : portée maximale | projet, CI, package |
| DEC-003 | Modèles manuels, générés ou hybrides | Validée : hybride interne | architecture des contrats |
| DEC-004 | Source canonique face aux variantes OpenAPI | Validée : comportement réel vérifié | instantanés, génération, surveillance |
| DEC-005 | Statut de l'API Search | Validée : conservée et documentée restreinte | surface publique, packaging |
| DEC-006 | Champs inconnus et sémantique absent/null | En attente | modèles et sérialisation |
| DEC-007 | Un package ou packages séparés | En attente | organisation NuGet |
| DEC-008 | Langue de la documentation | En attente | documentation publique |
| DEC-009 | Canal de release candidate | En attente | validation de publication |
| DEC-010 | Numéro et cadence de la prochaine version | En attente | release finale |
| DEC-011 | Conservation de fixtures de réponses | En attente | tests de modèles |
| DEC-012 | Engagement trimming et Native AOT | En attente | compatibilité et CI |
| DEC-013 | Dépendances Microsoft.Extensions et découpage DI | En attente | TFMs, packaging |
| DEC-014 | Extension des interfaces publiques | En attente | ajout des opérations |
| DEC-015 | Source de version et AssemblyVersion | En attente | release |
| DEC-016 | Authentification et approbation NuGet | En attente | publication |
| DEC-017 | Périmètre CI, couverture et tests live | En attente | barrières qualité |
| DEC-018 | Politique Git, tags et intégration des worktrees | En attente | intégration/release |
| DEC-019 | Identité et métadonnées du package | En attente | package/documentation |
| DEC-020 | Priorité entre OpenAPI, Postman et observations live | Validée : live arbitre | contrat effectif |
| DEC-021 | Types publics pour binaire et objet/liste | En attente | signatures de réponse |
| DEC-022 | Endpoints historiques, non documentés et Relay déprécié | En attente | couverture/migration |

## DEC-001 — Compatibilité publique et version majeure

**Décision du propriétaire — 10 août 2026.** Option B validée : la prochaine ligne est une **2.0** autorisant les corrections incompatibles nécessaires. Un guide de migration depuis `1.0.0` et le rapport ApiCompat restent obligatoires ; cette validation n'autorise pas une rupture sans justification ni test.

**Contexte.** Plusieurs corrections nécessaires peuvent changer les types de retour, la nullabilité ou la forme des modèles. Le package `1.0.0` est déjà public.

**Options.**

- **A — Évolution additive en 1.x.** Conserver les signatures existantes, ajouter les méthodes corrigées, marquer progressivement les anciennes comme obsolètes, et réserver leur retrait à une future version majeure.
- **B — Nettoyage immédiat en 2.0.** Corriger les signatures et modèles même lorsqu'ils sont incompatibles, avec guide de migration exhaustif.
- **C — Deux étapes.** Publier d'abord une 1.x additive couvrant les urgences, puis préparer une 2.0 de simplification.

**À renseigner.** Option, tolérance aux API obsolètes, durée de transition et règle SemVer appliquée aux changements de nullabilité.

## DEC-002 — Frameworks .NET ciblés et pris en charge

**Décision du propriétaire — 10 août 2026.** Rechercher la portée maximale avec .NET Standard, des actifs .NET 6 à .NET 10 et les runtimes encore supportés par Microsoft.

**Choix technique délégué et arrêté — 10 août 2026.** La matrice visée est :

```text
netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0
```

`netstandard2.0` est retenu plutôt que `netstandard2.1`, car il est la dernière version prise en charge par .NET Framework et la version recommandée par Microsoft pour une bibliothèque réutilisable à portée maximale. Les consommateurs `netstandard2.1` et .NET modernes peuvent sélectionner cet actif ou un actif .NET plus spécifique.

La baseline .NET Framework officiellement testée commence à **.NET Framework 4.7.2**, conformément à la recommandation Microsoft pour consommer les bibliothèques `netstandard2.0`. Les smoke tests Windows couvrent `net472`, `net48` et `net481`. NuGet peut considérer `net461` à `net471` compatibles, mais TorBoxSDK ne les annonce pas comme environnements testés ou supportés.

.NET 6 et .NET 7 conservent des actifs et des tests de compatibilité legacy. Cette décision ne présente pas leurs runtimes, hors support Microsoft, comme sécurisés ou supportés par Microsoft. .NET 8, 9 et 10 suivent le calendrier Microsoft ; la matrice est réévaluée à chaque fin de support et nouvelle version stable.

**Contexte.** Les cibles actuelles sont `net6.0` à `net10.0`. Deux sont hors support Microsoft et deux autres passent hors support le 10 novembre 2026. La compatibilité la plus large pourrait nécessiter `netstandard2.0` ou une cible .NET Framework explicite.

**Options.**

- **A — Conserver `net6.0;net7.0;net8.0;net9.0;net10.0`.** Aucun changement immédiat pour les actifs existants, mais maintien explicite de runtimes hors support.
- **B — `netstandard2.0;net8.0;net10.0`.** Portée large via .NET Standard, avec actifs modernes optimisés et audit de compatibilité nécessaire.
- **C — `net8.0;net9.0;net10.0`.** Cibles Microsoft encore supportées à la date de l'audit, à réviser dès novembre 2026.
- **D — `netstandard2.0;net462;net10.0`.** Prise en charge explicite de .NET Framework ancien et de .NET moderne, au prix d'une matrice et de branches conditionnelles plus importantes.
- **E — Autre matrice définie par le propriétaire.** À documenter avec date de fin de support par cible.

**À renseigner.** Cibles compilées, environnements seulement compatibles, environnements officiellement supportés, date de révision et politique concernant les runtimes hors support Microsoft.

## DEC-003 — Stratégie de modèles

**Décision du propriétaire — 10 août 2026.** Option C validée : génération interne à partir du contrat effectif, avec façade publique manuelle. Le choix du générateur et la propriété des overlays restent à valider par prototype ; aucun type généré ne devient public implicitement.

**Contexte.** Le document OpenAPI décrit mieux les requêtes que les réponses. Une génération intégrale ne peut donc pas produire seule un SDK fidèle.

**Options.**

- **A — Modèles et clients manuels renforcés.** L'OpenAPI sert de contrôle, le code public reste écrit à la main.
- **B — Génération complète.** Le client généré devient la surface principale ; exige auparavant des overlays et des schémas de réponse maintenus par le projet.
- **C — Architecture hybride.** Générer une couche interne à partir d'un contrat corrigé, puis conserver une façade publique stable et manuelle.

**À renseigner.** Option, outil éventuel, visibilité de la couche générée, politique d'édition du code généré et responsabilité des overlays.

## DEC-004 — Source canonique du contrat

**Décision du propriétaire — 10 août 2026.** Ne pas inventer, fusionner ou construire l'union de variantes. Le contrat effectif du SDK représente uniquement le comportement réellement disponible par l'API et vérifié par des tests live ciblés.

Les documents OpenAPI amont sont conservés sans modification comme preuves datées. Lorsqu'une route ou une forme apparaît dans une variante mais n'est pas disponible de manière reproductible sur l'API déployée, elle reste dans le [référentiel de divergences](api-divergences.md) et ne devient pas automatiquement une capacité stable du SDK.

**Contexte.** La même URL officielle a servi au moins deux variantes. Elles diffèrent sur quatre routes et plusieurs schémas.

**Options.**

- **A — Union contrôlée.** Construire un contrat projet contenant l'union des variantes, avec marquage expérimental des éléments non constants.
- **B — Intersection stable.** Le cœur stable ne contient que les éléments présents dans toutes les observations ; le reste est isolé.
- **C — Instantané choisi.** Épingler une variante identifiée et ne la changer qu'après revue.
- **D — Attendre une confirmation TorBox.** Ne pas exposer les différences tant que l'équipe TorBox n'a pas désigné le contrat canonique.

**À renseigner.** Option, nombre d'observations requis, durée de surveillance et autorité qui approuve une modification du snapshot.

## DEC-005 — Statut de Search

**Résultat de recherche — 10 août 2026.** L'annonce TorBox du 20 mai 2026 indique que Search est accessible uniquement aux IP autorisées et invite les hébergeurs d'instances publiques et développeurs à demander une autorisation par ticket. La documentation Postman reste visible, mais le domaine public ne publie actuellement aucun enregistrement A/AAAA depuis l'environnement d'audit, Cloudflare DNS ou Google DNS. La page de statut TorBox ne liste pas Search comme composant.

**Décision du propriétaire — 10 août 2026.** Conserver la surface Search et documenter explicitement sa restriction dans la documentation XML du code et la documentation du projet. Le SDK ne promet pas que l'API est accessible : le projet consommateur et son IP sortante doivent être autorisés par TorBox.

**Contexte.** L'API Search apparaît dans une variante OpenAPI et dans le journal de changements TorBox, mais pas dans l'autre variante. Le nom `search-api.torbox.app` n'a pas été résolu depuis l'environnement d'audit et la route de réglages a alterné entre existence et absence.

**Options.**

- **A — Surface stable du package principal.** Engagement de compatibilité identique au reste du SDK.
- **B — Surface expérimentale du package principal.** Namespace et documentation indiquent l'absence de garantie jusqu'à stabilisation.
- **C — Package séparé/prérelease.** Cycle et niveau de stabilité indépendants.
- **D — Retrait temporaire de la documentation publique.** Conserver éventuellement le code existant pour compatibilité, sans étendre la surface avant confirmation.

**À renseigner.** Option, URL canonique, comportement attendu en cas de `404`/DNS et critères de promotion en stable.

## DEC-006 — Évolution des données JSON

**Contexte.** Ignorer les champs inconnus évite les erreurs, mais les rend inaccessibles. Ignorer globalement les valeurs nulles à l'écriture empêche de distinguer « ne pas modifier » de « effacer la valeur ».

**Options pouvant être combinées.**

- conserver les propriétés inconnues dans un dictionnaire de `JsonElement` ;
- introduire un type optionnel à trois états : absent, valeur, null explicite ;
- limiter ces mécanismes aux modèles de réponse ou de mise à jour ;
- garder le comportement actuel et créer des modèles spécifiques seulement pour les opérations concernées.

**À renseigner.** Règles choisies, impact public accepté et politique de sérialisation par opération.

## DEC-007 — Organisation des packages

**Options.**

- un seul package `TorBoxSDK` pour Main, Search et Relay ;
- un cœur plus des packages `TorBoxSDK.Search` et/ou `TorBoxSDK.Relay` ;
- package unique maintenant, séparation réservée à une version majeure.

**À renseigner.** Option, règles de versionnement entre packages et migration des consommateurs.

## DEC-008 — Langue de la documentation

**Options.** Anglais public avec notes mainteneur françaises ; documentation entièrement anglaise ; documentation bilingue ; autre règle explicite.

## DEC-009 — Canal de release candidate

**Options.** `.nupkg` local uniquement ; flux NuGet.org en préversion ; registre GitHub Packages ; combinaison définie avec critères de promotion.

## DEC-010 — Numéro et cadence

À décider seulement après le rapport de compatibilité publique. Renseigner le numéro visé, la règle SemVer, la date cible, le rythme de surveillance OpenAPI et la cadence de correctifs.

## DEC-011 — Fixtures de réponses

**Contexte.** Les réponses sont insuffisamment décrites par OpenAPI. Des exemples réels anonymisés sont nécessaires pour vérifier les modèles.

**Options.** Fixtures synthétiques uniquement ; fixtures réelles anonymisées et revues ; fixtures chiffrées hors dépôt ; combinaison selon la sensibilité.

## DEC-012 — Trimming et Native AOT

**Options.** Aucune promesse ; compatibilité testée sans engagement ; support documenté avec génération de métadonnées JSON et tests dédiés.

## DEC-013 — Dépendances Microsoft.Extensions et découpage DI

**Contexte.** Les actifs `net8.0` et `net9.0` imposent actuellement des dépendances `Microsoft.Extensions.* 10.0.6`, tandis que `net6.0` et `net7.0` utilisent la ligne 8.x. Un consommateur .NET 8 peut donc voir ses dépendances élevées à la version 10 alors que le cœur HTTP n'a peut-être pas besoin de cette contrainte.

**Options de version.** Version minimale compatible ; alignement 8/9/10 par TFM ; ligne 10 unique ; autre plage vérifiée par tests consommateurs.

**Options de structure.** Garder DI/configuration dans `TorBoxSDK` ; séparer un cœur HTTP et `TorBoxSDK.Extensions.DependencyInjection` ; effectuer une transition avec package façade/métapackage.

**À renseigner.** Politique de versions minimales, packages créés, compatibilité de migration et cadence des mises à jour.

## DEC-014 — Extension des interfaces publiques

**Contexte.** Ajouter un membre à une interface publique peut casser les consommateurs qui l'implémentent ou la simulent, même si l'opération est additive du point de vue fonctionnel.

**Options.** Étendre directement en version majeure ; créer des interfaces de capacité/version supplémentaires ; exposer les nouveautés uniquement sur les classes concrètes ; fournir des méthodes d'extension lorsque leur sémantique le permet ; autre stratégie documentée.

**À renseigner.** Règle pour la ligne 1.x, stratégie de mocks et migration vers une éventuelle 2.0.

## DEC-015 — Source de version et AssemblyVersion

**Options de source.** Propriété modifiée dans le dépôt avec vérification ; version dérivée d'un tag strict ; paramètre de pipeline candidat ; MinVer ; Nerdbank.GitVersioning ; autre outil approuvé.

**Options d'assembly.** Suivre chaque version NuGet ; maintenir `AssemblyVersion` stable pendant une version majeure et utiliser `FileVersion`/`InformationalVersion` pour le détail.

**Invariant obligatoire.** Le pipeline échoue si tag, `.nuspec`, nom du package et versions d'assembly attendues sont incohérents.

## DEC-016 — Authentification et approbation NuGet

**Options d'authentification.** Trusted Publishing/OIDC avec jeton temporaire ; clé API limitée au seul package, au push et avec expiration/rotation.

**À renseigner.** Nombre et identité des approbateurs de l'environnement `release`, propriétaires habilités à pousser/déprécier/délister, délai d'approbation et procédure d'urgence.

## DEC-017 — Périmètre CI, couverture et tests live

**À décider.** OS obligatoires ; TFMs obligatoires ; seuil fixe ou ratchet de couverture ; éventuel mutation testing ; fréquence de surveillance OpenAPI ; autorisation, compte et budget des tests live de lecture/écriture/destructifs ; gestion d'une panne TorBox ; durée de conservation des rapports.

## DEC-018 — Politique Git, tags et worktrees

**À décider.** Squash, rebase ou merge commits ; branche de release ou intégration continue ; tags annotés et/ou signés ; personne autorisée à créer le tag ; règle de nettoyage des worktrees après intégration.

## DEC-019 — Identité et métadonnées du package

**Contexte.** Le package se présente comme non officiel alors que `Company` vaut actuellement `TorBox`. Les tags NuGet ne sont pas publiés à cause du nom de propriété utilisé. Le titre, l'icône et les notes de version ne sont pas renseignés.

**À décider.** Valeur de `Company`, titre, icône, texte « non officiel », tags, notes de version, URL d'incidents et branding autorisé.

## DEC-020 — Arbitrage des sources officielles

**Décision du propriétaire — 10 août 2026.** L'OpenAPI est la source principale. La documentation officielle publiée à `api-docs.torbox.app` et l'espace Postman TorBox complètent les descriptions, exemples et réponses absents d'OpenAPI.

Lorsqu'elles se contredisent, un test live ciblé arbitre ce qui est réellement accepté ou retourné par le service. Le résultat, ses preuves, sa date et sa portée sont inscrits dans le [référentiel de divergences](api-divergences.md). Un overlay ne corrige le contrat effectif qu'à partir de cette preuve ; il ne fusionne pas les possibilités documentaires.

**Contexte.** OpenAPI et la collection Postman TorBox ne décrivent pas toujours le même type de contenu ; les réponses live peuvent encore différer selon le déploiement.

**Options.** OpenAPI prioritaire ; Postman prioritaire ; observation live prioritaire ; hiérarchie par élément de contrat ; confirmation TorBox obligatoire en cas de conflit.

**À renseigner.** Ordre d'autorité, preuves acceptables, durée de validité d'une observation et personne qui approuve un overlay.

## DEC-021 — Forme des réponses publiques

**À décider.** `Stream`, `byte[]` ou objet réponse disposant d'un flux et de métadonnées pour les binaires ; méthodes distinctes ou union discriminée pour objet/liste ; comportement face aux redirections automatiques ; taille maximale des corps d'erreur conservés.

## DEC-022 — Endpoints historiques ou non documentés

**Contexte.** Deux alias Torrent queued sont officiels mais redondants, Intercom est exposé localement mais absent d'OpenAPI, et Relay `inactivecheck` paraît déprécié avec un contrat incompatible.

**À décider.** Ajouter, maintenir, marquer obsolète, isoler en expérimental ou retirer en version majeure chaque cas ; durée de transition et preuve attendue de TorBox.

## Modèle de validation

```text
Décision : DEC-___
État : Validée | Rejetée | Expérimentation
Option :
Date :
Auteur :
Justification :
Conditions / échéance de révision :
```
