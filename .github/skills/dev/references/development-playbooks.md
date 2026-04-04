# Development Playbooks

Playbooks standardisent les séquences multi-skills les plus fréquentes dans TorBoxSDK. Ils évitent de décider la chaîne de travail à chaque demande.

> Les règles métier et techniques restent dans les skills spécialisés. Ce fichier définit uniquement l'ordre, les handoffs et les critères de sortie.

---

## Playbook A — Implémenter un endpoint dans un client existant

**Quand l'utiliser :** un endpoint doit être ajouté dans un resource client déjà en place.

**Séquence :**
1. `/dev` exécute J2 avec [endpoint-placement-and-naming.md](./endpoint-placement-and-naming.md) et [endpoint-implementation-checklist.md](./endpoint-implementation-checklist.md)
2. `tests`
3. `code-review`
4. `docs` si l'endpoint est user-facing ou mérite un sample/README update

**Entrées attendues :**
- contrat d'API ou doc TorBox
- resource client cible déjà identifié

**Sorties attendues :**
- modèles request/response
- méthode d'interface et implémentation du client
- tests unitaires de succès/échec/mapping
- review sans finding CRITICAL ou MAJOR

**Critères de sortie :**
- build OK
- tests OK
- verdict review: `APPROVED` ou `APPROVED WITH MINOR ISSUES`

---

## Playbook B — Ajouter une nouvelle tranche de ressource

**Quand l'utiliser :** une nouvelle famille de capacité nécessite structure + modèles + endpoints + tests.

**Séquence :**
1. `architecture`
2. `/dev` exécute J2 endpoint workflow
3. `tests`
4. `code-review`
5. `docs`

**Entrées attendues :**
- besoins fonctionnels de la ressource
- endpoints concernés

**Sorties attendues :**
- surface publique validée (`Main`/`Search`/`Relay` + resource client)
- modèles regroupés au bon endroit
- endpoints implémentés dans la bonne hiérarchie
- tests de non-régression
- documentation ou sample si la capacité est exposée au public

**Handoff clé :**
- ne pas lancer `docs` tant que `code-review` n'a pas validé la tranche

---

## Playbook C — Refactorer une zone existante

**Quand l'utiliser :** changement de structure, de conventions, de DI, ou de hiérarchie sans ajout majeur d'endpoint.

**Séquence :**
1. `architecture`
2. `tests` (mise à jour / ajout de tests de protection)
3. `code-review`
4. `docs` si l'API publique ou les samples changent

**Sorties attendues :**
- structure clarifiée
- compatibilité de surface publique préservée ou rupture explicitée
- tests couvrant les comportements sensibles

---

## Playbook D — Stabiliser une phase du roadmap

**Quand l'utiliser :** fin d'une phase dans `docs/TODO.md`.

**Séquence :**
1. `/dev` classe les jobs restants de la phase
2. `tests` pour compléter la couverture manquante
3. `code-review` sur les fichiers modifiés ou le dossier concerné
4. `docs` pour aligner README, samples, XML docs

**Sorties attendues :**
- phase techniquement terminée
- couverture minimale présente
- documentation alignée sur le code réel

---

## Playbook E — Préparer une release NuGet

**Quand l'utiliser :** avant publication ou gel d'une version.

**Séquence :**
1. `tests`
2. `code-review`
3. `docs`

**Vérifications obligatoires :**
- `dotnet build` warning-clean
- `dotnet test` OK
- XML docs générées
- métadonnées NuGet complètes
- README et samples à jour

---

## Playbook F — Construire les fondations du projet

**Quand l'utiliser :** Phase 1 ou restructuration profonde de l'infrastructure.

**Séquence :**
1. suivre `J6` dans [dev-jobs.md](./dev-jobs.md)
2. `architecture` pour valider la hiérarchie globale
3. `tests` pour la couverture minimale du bootstrap si pertinent
4. `code-review`
5. `docs` si le bootstrap change la façon d'utiliser le SDK

**Sorties attendues :**
- solution buildable sur tous les targets
- client racine et DI utilisables
- conventions de base posées

---

## Règles de handoff entre skills

- `architecture` remet une structure cible et des contraintes de placement.
- `/dev` en J2 remet du code endpoint compilable, des modèles typés, et le placement correct dans la hiérarchie client.
- `tests` remet des tests qui verrouillent le comportement public.
- `code-review` remet un verdict exploitable pour décider merge ou rework.
- `docs` remet une surface utilisateur cohérente avec le code réellement livré.

Si un skill ne peut pas produire sa sortie minimale, revenir au skill précédent au lieu d'avancer.