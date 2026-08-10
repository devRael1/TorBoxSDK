# Contrat API et stratégie des modèles

> DEC-003 valide la génération interne avec façade publique manuelle. DEC-004 et DEC-020 imposent un contrat fondé sur le comportement live vérifié, sans union de variantes. Les écarts sont suivis dans [Divergences API observées](api-divergences.md).

## Périmètre de l'audit

Sources examinées :

- spécification officielle `https://api.torbox.app/openapi.json` ;
- documentation officielle `https://api-docs.torbox.app/` et interface Swagger ;
- clients, interfaces, modèles et sérialisation du dépôt ;
- tests unitaires, OpenAPI et live existants ;
- spécification Relay et implémentation Search présentes dans le dépôt.

L'audit distingue quatre niveaux de fidélité :

1. **Route** : verbe et chemin existent.
2. **Requête** : paramètres, emplacement, obligatoire/facultatif, contenu et format sont corrects.
3. **Réponse** : statut, type de contenu, enveloppe, forme et champs sont corrects.
4. **Comportement public** : la signature .NET, la nullabilité et les exceptions expriment réellement les trois niveaux précédents.

Une méthode n'est considérée couverte que lorsque les quatre niveaux sont vérifiés.

Sur la seule présence des opérations, le SDK correspond à **90 opérations sur 97**, soit **92,8 %** de la variante Main la plus complète. Cette valeur n'est pas un score de conformité : elle inclut des méthodes dont le transport, le modèle ou la forme de réponse doivent encore être corrigés.

## Instabilité de la spécification officielle

Douze téléchargements réalisés pendant l'audit ont produit deux contenus distincts à la même URL.

| Élément | Variante A | Variante B |
|---|---:|---:|
| Taille observée | 96 355 octets | 100 848 octets |
| Routes | 87 | 91 |
| Opérations | 93 | 97 |
| Schémas de composants | 36 | 42 |
| Routes Search Engine Settings | absentes | 4 présentes |
| Schémas Search Engine Settings | absents | 6 présents |
| `subscribed_email_segments` | présent | absent |
| `airlocked` dans les trois requêtes d'édition | présent | présent |

Les quatre paires GET/POST suivantes partagent en outre un `operationId` dupliqué : Torrent Check Cached, Usenet Check Cached, Web Download Check Cached et OAuth Callback. Un générateur direct peut produire des collisions ou renommer les méthodes de façon dépendante de l'outil.

Les empreintes SHA-256 observées commencent respectivement par `AE0504…` et `B1836…`. Les fichiers complets et leurs métadonnées doivent être enregistrés comme preuves amont séparées. Aucun document fusionné n'est considéré comme la réalité du service ; les tests live alimentent le contrat effectif et le référentiel DIV-001.

Une sonde anonyme répétée sur `GET /v1/api/user/settings/searchengines` a également alterné entre `401` — route reconnue sans authentification — et `404` — route absente. Cela indique une différence de déploiement en amont, pas seulement une fluctuation du fichier documentaire.

### Conséquence pour le SDK

Le SDK ne doit pas générer ou supprimer automatiquement des membres publics à partir du document téléchargé pendant un build. Une telle opération rendrait un même commit non reproductible. Les téléchargements distants doivent alimenter un rapport de dérive ; seuls des instantanés révisés doivent alimenter le build et les tests déterministes.

La [baseline de contrat V2-110](contract-baseline.md) rend cette règle
opérationnelle : son manifeste versionné lie chaque artefact brut à sa
provenance et à son hash, et enregistre les sources indisponibles sans les
fabriquer. Son monitor distant est manuel, opt-in et purement informatif ; il
ne modifie ni la baseline ni le SDK, et ne définit aucune planification CI.

## Limites du contrat de réponse

Dans les variantes observées, les 93 ou 97 opérations déclarent des réponses JSON `200` sans propriétés de schéma utiles. L'OpenAPI permet donc de vérifier de nombreux éléments de requête :

- verbes et chemins ;
- paramètres de chemin et de requête ;
- corps JSON, formulaire ou multipart ;
- schémas de certaines requêtes ;
- champs obligatoires, formats et valeurs par défaut lorsqu'ils sont renseignés.

Il ne permet pas de prouver :

- l'enveloppe exacte des réponses ;
- la différence entre objet unique et tableau ;
- la nullabilité réelle des champs retournés ;
- les champs ajoutés dynamiquement ;
- les réponses binaires ou de redirection lorsqu'elles sont décrites de façon incomplète.

Les réponses doivent être documentées par un second corpus : documentation textuelle, captures de réponses anonymisées, tests live autorisés et comportement constaté. Ce corpus est soumis à DEC-011.

V2-110 ne crée pas ce corpus ni de fixture de réponse : DEC-011 reste ouverte.

## Matrice des opérations

### Opérations de l'API principale observées

| Domaine | Officiel variante B | Présent dans le SDK | Manquant |
|---|---:|---:|---:|
| Racine | 1 | 1 | 0 |
| Changelogs | 2 | 2 | 0 |
| Integrations | 18 | 17 | 1 |
| Notifications | 5 | 5 | 0 |
| Queued | 2 | 2 | 0 |
| RSS | 5 | 5 | 0 |
| Speedtest | 1 | 1 | 0 |
| Stats globales | 2 | 2 | 0 |
| Stream | 2 | 2 | 0 |
| Torrents | 14 | 12 | 2 |
| Usenet | 11 | 8 | 3 |
| User | 17 | 16 | 1 |
| Vendors | 8 | 8 | 0 |
| Web Downloads | 9 | 9 | 0 |
| **Total** | **97** | **90** | **7** |

Cette matrice mesure la présence des opérations, pas leur fidélité au protocole. Une route comptée « présente » peut encore envoyer un contenu incorrect.

### Routes absentes ou conditionnelles à traiter

| Domaine | Écart | Traitement requis avant exposition |
|---|---|---|
| User | statistiques utilisateur absentes | confirmer route, réponse et stabilité ; ajouter test de contrat et fixture |
| Usenet | trois opérations liées aux fournisseurs absentes | relever paramètres, réponses, permissions et erreurs |
| Integration | callback OAuth `POST` absent | distinguer clairement les variantes `GET` et `POST` |
| Torrents | `getqueued` et `controlqueued` historiques absents | déterminer statut actuel face aux routes Queued ; ne pas dupliquer sans DEC-001 |
| User/Search Engines | quatre opérations présentes seulement dans une variante | appliquer DEC-004 et vérifier DIV-001 en live |
| General/Notifications | Intercom exposé par le SDK mais absent des variantes | vérifier s'il s'agit d'une route encore supportée, privée ou obsolète |
| Search API | clients et modèles présents localement | confirmer DNS, base URL, authentification et contrat officiel courant |
| Relay API | deux routes décrites par une spécification séparée | conserver un suivi contractuel séparé de l'API principale |

Les sept opérations officielles absentes sont :

- `POST /v1/api/integration/oauth/{provider}/callback` ;
- `GET /v1/api/torrents/getqueued` ;
- `POST /v1/api/torrents/controlqueued` ;
- `GET /v1/api/usenet/provider/connection` ;
- `GET /v1/api/usenet/provider/account` ;
- `POST /v1/api/usenet/provider/account/resetpw` ;
- `GET /v1/api/user/stats`.

Le SDK expose en supplément `GET /v1/api/intercom/hash`, absent des variantes observées. Le traitement des alias queued, d'Intercom et de Relay déprécié relève de DEC-022.

## Écarts de requête prioritaires

### Web Downloads

Les méthodes de création synchrone et asynchrone envoient actuellement du JSON. L'OpenAPI observé indique `application/x-www-form-urlencoded`, tandis que la collection Postman officielle montre `multipart/form-data` pour certaines opérations. Le JSON actuel ne correspond à aucune de ces deux descriptions. Conformément à DEC-020, un test live ciblé doit arbitrer DIV-003 avant de modifier l'encodage.

Contrôles à ajouter :

- `Content-Type` exact ;
- noms et répétition éventuelle des champs ;
- omission des paramètres absents ;
- encodage des URL et caractères non ASCII ;
- corps identique pour les clients synchrone et asynchrone, hors route concernée.

### Vendors

Les opérations d'enregistrement, d'enregistrement d'utilisateur et de mise à jour de compte sont envoyées en JSON. L'OpenAPI indique un formulaire URL-encodé et Postman montre du multipart pour certaines opérations Vendor. Le test live ciblé de DIV-003 détermine le contrat réellement accepté avant le remplacement des tests actuels.

`removeuser` envoie actuellement un corps JSON contenant `user_email`, alors que la spécification attend un paramètre de requête `user_auth_id`.

Ces corrections peuvent modifier des modèles publics ou nécessiter de nouvelles surcharges. Leur compatibilité dépend de DEC-001.

### Jobs d'intégration

`CreateIntegrationJobRequest` est partagé entre plusieurs fournisseurs et contient principalement `id`, `type`, `file_id` et `zip`. Les opérations Google Drive, Dropbox et OneDrive déclarent des jetons fournisseur requis, mais le mapping actuel ne les transporte pas. `DownloadId` est nullable alors que `id` est requis par le contrat observé.

Le modèle commun masque donc des invariants différents. Alternatives à soumettre dans le cadre de DEC-003 :

- un modèle par fournisseur ;
- une hiérarchie ou union discriminée ;
- un modèle interne généré par opération avec façade publique ;
- conservation temporaire du modèle commun avec nouvelles surcharges et validation runtime.

### Champs et contraintes manquants

Écarts confirmés à intégrer dans les futurs tests de modèle :

- `airlocked` absent des requêtes d'édition Torrent, Usenet et Web Download ;
- `id` d'intégration nullable alors qu'il est requis ;
- `url` Vendor et `type` d'export représentés comme facultatifs malgré le contrat observé ;
- `apikey` et `download_type` des moteurs de recherche représentés comme facultatifs dans la variante qui les déclare requis ;
- Usenet et Web Download omettent notamment `original_url`, `download_id`, `cached`, `cached_at`, `alternative_hashes`, `tags` et `airlocked` ;
- suppression Vendor fondée localement sur l'email, alors que le contrat Main observé attend `user_auth_id`.

Chaque correction doit distinguer ajout compatible, changement de nullabilité, validation à l'exécution et rupture de sérialisation. DEC-001 fixe la ligne de version ; DEC-006 fixe la sémantique absent/null.

### Jeton de téléchargement direct

La documentation de `RequestDownloadOptions.Token` indique qu'une valeur nulle utilise le jeton par défaut du client. Le client principal ne transmet toutefois la clé qu'au client Notifications ; Torrents, Usenet et Web Downloads ne la reçoivent pas. Leurs méthodes de téléchargement utilisent uniquement `options.Token`.

Comme le paramètre `token` est requis par la route observée, le comportement documenté n'est pas assuré. Les tests doivent vérifier :

- priorité du jeton fourni pour l'appel ;
- repli sur le jeton configuré ;
- absence de jeton clairement refusée avant requête ou traitée selon le contrat retenu ;
- absence de fuite du jeton dans les logs et messages d'erreur.

## Écarts de forme et de transport des réponses

### Enveloppe JSON supposée universelle

Le helper HTTP charge le contenu en chaîne puis suppose une enveloppe JSON `success/error/detail/data`. Cette hypothèse ne convient pas aux PDF, fichiers torrent, téléchargements, redirections CDN, réponse directe Relay et erreurs FastAPI `422` dont `detail` peut être un tableau.

Le transport doit d'abord examiner le statut et le type de contenu, puis choisir le désérialiseur ou le flux approprié. Les téléchargements volumineux ne doivent pas être chargés intégralement en mémoire sous forme de texte.

### Objet unique ou collection

La route `mylist` sans identifiant renvoie une collection, tandis que sa variante avec `id` est documentée comme un objet unique. Le SDK désérialise toujours `TorBoxResponse<IReadOnlyList<T>>`.

Une seule signature ne doit pas supposer deux formes JSON incompatibles. Après DEC-001 et DEC-021, créer des opérations distinctes — liste et élément — ou une union/adaptation explicitement compatible.

### Cache

Les routes `checkcached` acceptent des variantes objet/liste. Le modèle local est fixe. Il faut capturer toutes les formes réelles autorisées et exposer un résultat prévisible, sans désérialisation dépendante du premier élément.

### Binaire et redirection

Les téléchargements, exports et redirections ne doivent pas être forcés dans une enveloppe JSON :

- PDF de transaction : le retour actuel `TorBoxResponse<string>` doit être confronté au `Content-Type` et aux octets réellement reçus ;
- téléchargement avec redirection : distinguer l'URL/redirection de la réponse binaire suivie automatiquement par `HttpClient` ;
- fichiers : privilégier `Stream` ou une abstraction de réponse permettant de disposer correctement le contenu ;
- noms de fichiers et types MIME : lire les en-têtes sans faire confiance à un chemin distant.

Le changement de type public dépend de DEC-001 et DEC-021.

### Résultats de création et Stream

Les créations renvoient actuellement les modèles complets `Torrent`, `UsenetDownload` ou `WebDownload`. Les exemples officiels indiquent plutôt des résultats d'opération dédiés, par exemple `hash`, identifiant créé et `auth_id`, avec `jdownloader_id`/`link_list` pour Web Download. Des DTO de résultat dédiés éviteraient de promettre des champs que la création ne renvoie pas.

`CreateStreamAsync` retourne une chaîne, alors que l'exemple officiel décrit un objet contenant identifiants, nom, domaine, URL HLS et métadonnées. Les options locales omettent aussi `scrobbling_enabled`. La forme exacte doit être capturée dans une fixture approuvée avant modification publique.

### Erreurs évolutives

`TorBoxException` conserve principalement une enum locale et un détail. Une erreur inconnue ajoutée par TorBox doit rester diagnostiquable. Après DEC-006 et DEC-021, évaluer la conservation de :

- code brut et code typé lorsqu'il est connu ;
- statut HTTP ;
- détail structuré ou brut ;
- corps brut limité et expurgé ;
- identifiant de requête et en-têtes de diagnostic non sensibles.

## Relay API

La spécification Relay expose deux routes et le SDK appelle les deux. La racine est globalement compatible avec les modèles d'état et de workers.

`GET /v1/inactivecheck/torrent/{auth_id}/{torrent_id}` présente en revanche un écart majeur : la documentation officielle le décrit comme principalement déprécié et renvoie directement un objet proche de `{ status: bool, message: string }`. Le SDK impose l'enveloppe Main et attend un modèle avec `status` chaîne, `is_inactive` et `last_active`. Le test local reproduit l'enveloppe du SDK, pas la réponse officielle.

Une correction additive avec obsolescence ou une correction cassante en version majeure dépend de DEC-001 et DEC-022.

## Search API

Search apparaît encore dans l'espace Postman officiel et le changelog TorBox mentionne `cached_only`. Une annonce TorBox du 20 mai 2026 indique cependant que l'accès est désormais limité aux IP autorisées ; les hébergeurs d'instances publiques et développeurs peuvent demander une inscription en liste blanche par ticket.

Vérifications du 10 août 2026 :

- aucune adresse A ou AAAA n'est publiée pour `search-api.torbox.app` depuis le résolveur local ;
- Cloudflare DNS et Google DNS renvoient également une réponse DNS valide sans adresse ;
- les sondes HTTPS ne peuvent donc pas atteindre la racine ou `/meta/imdb:tt0080684` ;
- la page de statut TorBox ne contient aucun composant Search ;
- la collection Postman reste publique et décrit toujours les routes, ce qui documente le contrat historique mais ne prouve pas l'accès public actuel.

**Conclusion et décision :** Search existe encore pour des projets autorisés, mais un consommateur arbitraire de TorBoxSDK ne peut pas l'utiliser aujourd'hui. Une IP autorisée et une confirmation TorBox sont des prérequis externes. DEC-005 conserve la surface dans la 2.0 avec un avertissement explicite dans la documentation XML et les guides, sans promesse d'accès.

Opérations documentées principales :

- `GET /meta/{id}` et `GET /meta/search/{query}` ;
- `GET /torrents/{id}` et `GET /torrents/search/{query}` ;
- `GET /usenet/{id}` et `GET /usenet/search/{query}`.

Le SDK expose aussi `/usenet/download/{id}/{guid}`, `/torznab/api` et `/newznab/api`, non confirmées dans la documentation Search courante examinée.

Écarts visibles avec les exemples officiels :

- les recherches et accès par identifiant utilisent des wrappers contenant des métadonnées, alors que certaines méthodes locales attendent un élément direct ;
- les wrappers locaux omettent `time_taken`, `cached` et les totaux ;
- les noms et structures des résultats Torrent/Usenet diffèrent largement (`raw_title`, `last_known_seeders`, `tracker`, catégories, fichiers, etc.) ;
- Meta omet plusieurs champs et `releaseYears` peut être un entier ou une plage textuelle ;
- l'enveloppe Search contient un `message` non représenté par l'enveloppe Main locale.

Search ne doit pas être déclaré public ou fidèle sur la seule base de la présence des routes. Même si TorBoxSDK obtenait une autorisation, celle-ci ne rendrait pas automatiquement les applications consommatrices autorisées : l'accès dépend de l'IP qui appelle le service.

## Architecture des modèles

### Règles indépendantes de l'outil choisi

Après validation des décisions, chaque nouveau contrat doit respecter les règles suivantes :

- un modèle de requête n'est réutilisé comme modèle de réponse que si leurs contraintes sont réellement identiques ;
- les champs requis à la création ne deviennent pas artificiellement facultatifs pour permettre la réutilisation ;
- les mises à jour partielles distinguent « absent », « valeur » et « null explicite » si TorBox attribue des sens différents ;
- les types JSON `integer`, dates, URI et identifiants sont mappés après observation de leur plage et format réels ;
- les enums inconnues ne doivent pas rendre toute la réponse illisible lorsqu'une nouvelle valeur serveur apparaît ;
- une collection vide reste valide et ne doit pas empêcher la validation du schéma ;
- les objets de réponse peuvent conserver les champs inconnus si DEC-006 le valide ;
- les réponses binaires restent binaires ;
- les modèles internes et publics ont une règle de conversion testée lorsqu'une façade est utilisée.

### Trois stratégies possibles

| Stratégie | Avantages | Contraintes | Décision |
|---|---|---|---|
| Manuelle renforcée | contrôle fin de la surface publique ; peu de migration immédiate | risque d'oubli ; maintenance des mappings | DEC-003 A |
| Génération complète | couverture mécanique des requêtes ; diff de contrat visible | réponses OpenAPI insuffisantes ; forte incidence publique | DEC-003 B |
| Hybride, génération interne | contrat mécanique et façade stable ; adaptation des réponses | couche de mapping supplémentaire ; discipline d'architecture | DEC-003 C |

Si Kiota est évalué, le prototype doit mesurer au minimum : contenus formulaire/multipart, conservation de données supplémentaires, sérialisation des enums, backing store pour le null explicite, visibilité interne des modèles et reproductibilité via le fichier de verrouillage. Le prototype ne doit pas remplacer la surface publique sans décision.

## Pipeline de contrat proposé

### 1. Acquisition surveillée

Pour chaque source officielle :

- enregistrer URL, heure UTC, statut HTTP, `ETag`/`Last-Modified` disponibles, taille et SHA-256 ;
- effectuer plusieurs lectures séparées pour détecter un déploiement non homogène ;
- ne jamais utiliser directement le dernier téléchargement dans une compilation de release.

### 2. Normalisation

- trier les clés sans modifier la sémantique ;
- résoudre ou conserver les références de façon déterministe ;
- appliquer uniquement des overlays versionnés et revus ;
- associer chaque correction d'overlay à une preuve : documentation, fixture ou réponse autorisée.

### 3. Diff sémantique

Classer au moins :

- route/opération ajoutée ou supprimée ;
- paramètre requis/facultatif, emplacement ou type modifié ;
- contenu accepté ou produit modifié ;
- champ, enum, format ou défaut modifié ;
- modification seulement documentaire.

Le rapport doit séparer changements additifs, potentiellement cassants, clairement cassants et indéterminés.

### 4. Revue

Aucun changement de snapshot ne modifie automatiquement le SDK. La revue confirme :

- disponibilité réellement vérifiée selon DEC-004 et le référentiel de divergences ;
- placement selon DEC-005 et DEC-007 ;
- impact SemVer selon DEC-001 ;
- tests et documentation requis.

### 5. Génération ou contrôle

DEC-003 a validé une génération interne avec façade publique manuelle. Le contrat corrigé alimente donc une couche `internal`; le code généré n'est jamais modifié à la main, et les corrections vivent dans les overlays, adaptateurs ou templates versionnés. Le générateur concret reste soumis à un prototype comparatif.

## Tests de contrat à construire

### Tests de requête par opération

Utiliser un gestionnaire HTTP capturant l'appel et vérifier :

- méthode et URI absolue ;
- chemin et échappement ;
- query string, paramètres répétés et ordre non significatif ;
- en-têtes d'authentification sans révéler le secret ;
- `Content-Type` et corps ;
- annulation et durée de vie des flux ;
- absence de requête lorsque la validation cliente doit échouer.

### Tests de réponse par fixture

Pour chaque forme autorisée :

- succès minimal et complet ;
- champs inconnus ;
- champs absents et explicitement nuls ;
- enum connue et inconnue ;
- objet, tableau vide et tableau multiple ;
- enveloppe d'erreur ;
- contenu binaire et redirection.

### Tests de couverture machine

Le lecteur OpenAPI actuel se limite aux propriétés des schémas de composants et maintient un mapping manuel. Il doit être remplacé ou étendu pour vérifier aussi :

- chaque opération et son identifiant ;
- types de contenu ;
- paramètres et leur emplacement ;
- obligation, nullabilité, défaut et format ;
- items de tableaux et références ;
- codes de réponse et contenus.

Le détecteur de champs non mappés ne doit pas s'arrêter au premier élément d'une collection ; une collection vide doit être un cas testé, non une validation implicite.

## Procédure d'ajout d'une opération

1. Identifier l'opération dans un snapshot approuvé.
2. Vérifier sa stabilité et son package selon DEC-004, DEC-005 et DEC-007.
3. Capturer les preuves manquantes de réponse seulement lorsqu'une décision
   DEC-011 applicable l'autorise.
4. Définir séparément requête, réponse, erreur et transport.
5. Évaluer l'impact public par rapport au package `1.0.0`.
6. Implémenter le client ou l'adaptateur dans un worktree borné.
7. Ajouter les tests de requête, de contrat et de consommation du package ;
   ajouter des fixtures de réponse seulement si DEC-011 les a autorisées.
8. Documenter l'opération, sa stabilité, ses permissions et ses limites.
9. Mettre à jour la matrice de couverture.
10. Faire revoir avant intégration ; ne pas publier depuis le worktree.

## Critères de sortie du chantier API

- 100 % des opérations du contrat approuvé sont classées : supportée, expérimentale, différée ou obsolète, avec motif ;
- 100 % des méthodes publiques ont un test de protocole HTTP ;
- chaque modèle de réponse public est soutenu par un schéma ou une fixture approuvée ;
- aucun test reproductible ne télécharge une spécification fluctuante ;
- chaque route conditionnelle a un niveau de stabilité documenté ;
- les différences avec l'API officielle génèrent un rapport lisible par les mainteneurs ;
- les changements incompatibles sont soit évités, soit approuvés et documentés selon DEC-001.

## Sources

- [OpenAPI TorBox](https://api.torbox.app/openapi.json)
- [Swagger TorBox](https://api.torbox.app/docs)
- [Documentation TorBox](https://api-docs.torbox.app/)
- [Espace Postman officiel TorBox](https://www.postman.com/torbox/torbox-api/overview)
- [OpenAPI Relay](https://relay.torbox.app/openapi.json)
- [Collection Postman Relay](https://www.postman.com/torbox/torbox-api/documentation/tagtekq/relay-api)
- [Collection Postman Search](https://www.postman.com/torbox/torbox-api/documentation/u47iwao/search-api)
- [Annonce TorBox v8.4.4 concernant la liste blanche Search](https://www.reddit.com/r/TorBoxApp/comments/1tj1fku/torbox_v844_update/)
- [Statut des services TorBox](https://status.torbox.app/)
- [Changelog TorBox](https://feedback.torbox.app/changelog)
- [OpenAPI Overlay Specification](https://spec.openapis.org/overlay/v1.0.0.html)
- [Kiota : vue d'ensemble](https://learn.microsoft.com/openapi/kiota/overview)
- [Kiota : génération d'un client](https://learn.microsoft.com/openapi/kiota/using)
- [Kiota : sérialisation et données supplémentaires](https://learn.microsoft.com/openapi/kiota/serialization)
- [Kiota : backing store](https://learn.microsoft.com/openapi/kiota/backing-store)
