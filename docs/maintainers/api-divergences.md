# Référentiel des divergences API

> État : initialisé le 10 août 2026. Une divergence n'est pas résolue par supposition. Elle est close uniquement par une preuve live reproductible ou une confirmation explicite de TorBox.

## Rôle du référentiel

Ce document trace les différences entre :

- l'OpenAPI officiel, source documentaire principale ;
- la documentation officielle `api-docs.torbox.app` et l'espace Postman TorBox ;
- le comportement observé sur l'API déployée ;
- le comportement actuel de TorBoxSDK.

Conformément à DEC-004 et DEC-020, TorBoxSDK ne construit ni union, ni fusion théorique des contrats. Le contrat effectif contient uniquement ce qui a été observé comme réellement disponible. Les documents amont restent archivés sans modification comme preuves ; les overlays reflètent les résultats vérifiés.

## États possibles

| État | Signification |
|---|---|
| À tester | contradiction identifiée, aucune preuve live suffisante |
| Confirmée live | comportement reproduit avec une requête contrôlée |
| Intermittente | plusieurs comportements ont été observés sur le service déployé |
| Accès restreint | capacité réelle mais non accessible au public général |
| Confirmée TorBox | clarification explicite obtenue de TorBox |
| Résolue SDK | contrat effectif, code, tests et documentation sont alignés |
| Différée | preuve insuffisante ou test non autorisé/coûteux |

## Règle de preuve

Un test live d'arbitrage doit enregistrer :

- identifiant de divergence ;
- date et heure UTC ;
- environnement, région et version du SDK de test ;
- URL et opération, sans secret ;
- méthode, emplacement des paramètres et `Content-Type` essayé ;
- statut, `Content-Type` et forme de réponse ;
- répétitions et cohérence du résultat ;
- permissions du compte et catégorie de risque ;
- corps expurgé ou empreinte de fixture ;
- conclusion limitée à ce que le test démontre.

Une réponse `401` peut démontrer l'existence d'une route, mais pas son contrat authentifié. Une réponse `403` ne valide ni les paramètres ni le modèle de succès. Une réponse `422` peut renseigner le schéma de validation sans prouver le comportement métier.

## Catégories de tests live

| Catégorie | Autorisation | Exemples |
|---|---|---|
| Lecture publique | automatique ciblée | racine, stats, changelog, DNS |
| Lecture authentifiée | compte de test requis | listes, paramètres, statistiques utilisateur |
| Validation sans mutation | compte de test et données invalides maîtrisées | type de contenu, champs requis |
| Mutation réversible | autorisation explicite et nettoyage garanti | création puis suppression d'une ressource de test |
| Coûteuse ou destructive | autorisation ponctuelle du propriétaire | téléchargement non caché, suppression de compte, consommation importante |

Le test le moins risqué capable d'arbitrer la divergence est utilisé. Les secrets et données personnelles ne sont jamais conservés dans ce document.

## Registre initial

### DIV-001 — Deux variantes de l'OpenAPI Main

| Champ | Valeur |
|---|---|
| État | Intermittente |
| OpenAPI | variantes 93 opérations/36 schémas et 97 opérations/42 schémas |
| Documentation | quatre opérations Search Engine Settings présentes selon la variante ; `subscribed_email_segments` présent seulement dans l'autre |
| Observation live | la sonde anonyme `GET /v1/api/user/settings/searchengines` a alterné entre `401` et `404` |
| SDK actuel | expose les quatre opérations de réglage Search dans User |
| Contrat effectif | ne pas déduire une union ; vérifier les opérations authentifiées et leur disponibilité répétée avant classement stable |
| Prochaine preuve | test read-only authentifié répété, avec instantané OpenAPI associé à chaque série |

### DIV-002 — Accès à Search API

| Champ | Valeur |
|---|---|
| État | Accès restreint |
| OpenAPI | aucun OpenAPI Search courant validé |
| Documentation | collection Postman toujours publique avec routes et exemples |
| Annonce TorBox | depuis le 20 mai 2026, accès réservé aux projets et IP autorisés |
| Observation live | aucune adresse A/AAAA via résolveur local, Cloudflare DNS ou Google DNS ; HTTPS impossible |
| SDK actuel | client Search et URL par défaut publics |
| Contrat effectif | surface conservée, mais disponibilité non garantie et restriction documentée dans XML et guides |
| Prochaine preuve | uniquement après autorisation d'une IP de test par TorBox |

### DIV-003 — Type de contenu Web Downloads et Vendors

| Champ | Valeur |
|---|---|
| État | À tester |
| OpenAPI | `application/x-www-form-urlencoded` pour les opérations concernées |
| Documentation | Postman montre `multipart/form-data` pour certaines opérations |
| SDK actuel | JSON |
| Contrat effectif | indéterminé ; aucun des changements documentaires n'est appliqué sans preuve live |
| Prochaine preuve | requêtes de validation ciblées par opération, puis mutation réversible seulement si nécessaire et autorisée |

### DIV-004 — Schémas de réponses Main absents

| Champ | Valeur |
|---|---|
| État | À tester par opération |
| OpenAPI | schémas `200` vides |
| Documentation | exemples Postman détaillés pour une partie des opérations |
| SDK actuel | modèles manuels et enveloppe JSON supposée générale |
| Contrat effectif | fixture live expurgée requise pour chaque forme publique de réponse |
| Prochaine preuve | lectures authentifiées, puis fixtures objet/liste/null/erreur/binaire |

### DIV-005 — Réponse objet ou liste

| Champ | Valeur |
|---|---|
| État | À tester |
| Documentation | `mylist` varie avec `id`; `checkcached` varie avec `format` |
| SDK actuel | plusieurs méthodes imposent toujours une liste ou une forme fixe |
| Contrat effectif | types publics distincts seulement après capture des formes live |
| Prochaine preuve | appels read-only avec et sans `id`, puis `format=object` et `format=list` |

### DIV-006 — Binaires et redirections

| Champ | Valeur |
|---|---|
| État | À tester |
| Documentation | PDF, `.torrent`, NZB, export magnet/fichier et redirections CDN |
| SDK actuel | plusieurs retours sont traités comme `TorBoxResponse<string>` |
| Contrat effectif | statut, en-têtes, redirection et octets doivent être capturés avant choix du type public DEC-021 |
| Prochaine preuve | HEAD/GET contrôlés sur une ressource de test déjà disponible, sans téléchargement non caché |

### DIV-007 — Relay `inactivecheck`

| Champ | Valeur |
|---|---|
| État | À tester |
| Documentation | objet direct proche de `{ status: bool, message: string }`, opération principalement dépréciée |
| SDK actuel | enveloppe Main et modèle `status` chaîne/`is_inactive`/`last_active` |
| Contrat effectif | indéterminé tant qu'une ressource Relay de test autorisée n'est pas disponible |
| Prochaine preuve | appel read-only avec identifiants de test, puis décision DEC-022 |

### DIV-008 — Modèles Search historiques

| Champ | Valeur |
|---|---|
| État | Différée — accès restreint |
| Documentation | wrappers, `time_taken`, totaux, résultats Torrent/Usenet et Meta riches |
| SDK actuel | formes et noms largement différents ; certaines routes supplémentaires non confirmées |
| Contrat effectif | ne peut pas être certifié sans IP autorisée |
| Prochaine preuve | fixtures fournies ou appels live après autorisation TorBox |

## Modèle d'une nouvelle entrée

```text
### DIV-___ — Titre

État :
Opération :
OpenAPI :
Documentation officielle :
SDK actuel :
Test live :
Date UTC / environnement :
Preuve ou fixture expurgée :
Contrat effectif retenu :
Impact public / SemVer :
Tests ajoutés :
Documentation mise à jour :
Approbateur :
```

## Cycle de résolution

1. Détecter et enregistrer la divergence sans modifier le contrat effectif.
2. Classer le risque du test live et obtenir l'autorisation nécessaire.
3. Exécuter le test ciblé plusieurs fois lorsque l'infrastructure peut varier.
4. Conserver une preuve expurgée et reproductible.
5. Décrire uniquement le comportement démontré.
6. Mettre à jour l'overlay et la couche générée interne.
7. Adapter la façade publique 2.0 et ajouter les tests wire/fixtures.
8. Relancer le test de contrat et ApiCompat.
9. Faire approuver l'entrée avant de la marquer `Résolue SDK`.

## Sources

- [OpenAPI Main TorBox](https://api.torbox.app/openapi.json)
- [Documentation principale TorBox](https://api-docs.torbox.app/)
- [Espace Postman TorBox](https://www.postman.com/torbox/torbox-api/overview)
- [Documentation Postman Search](https://www.postman.com/torbox/torbox-api/documentation/u47iwao/search-api)
- [Annonce de restriction Search du 20 mai 2026](https://www.reddit.com/r/TorBoxApp/comments/1tj1fku/torbox_v844_update/)
- [Statut TorBox](https://status.torbox.app/)
