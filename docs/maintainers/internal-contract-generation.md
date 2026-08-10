# Génération interne des contrats — V2-200

Ce guide s'adresse aux mainteneurs qui régénèrent ou vérifient les contrats
internes de TorBoxSDK. Il décrit le workflow V2-200 ; il ne définit ni une
nouvelle API publique ni un modèle de réponse public.

> La décision [DEC-003](decisions.md) retient une architecture hybride :
> Kiota produit une couche interne et la façade publique reste manuelle. La
> génération est déterministe à partir des snapshots revus ; elle ne contacte
> pas TorBox et ne transforme pas une dérive amont en changement de SDK.

## Périmètre et artefacts suivis

La génération prend uniquement les sources OpenAPI `captured` de la
[baseline V2-110](contract-baseline.md) :

| Famille | Source de baseline | Sortie suivie dans Git |
|---|---|---|
| Main | `main-openapi` | `src/TorBoxSDK/Internal/Generated/Main` |
| Relay | `relay-openapi` | `src/TorBoxSDK/Internal/Generated/Relay` |

Le script valide avant génération la disponibilité, la taille et le SHA-256
de chaque snapshot déclaré dans `contracts/baseline/manifest.json`. Les
snapshots bruts ne sont jamais réécrits : la copie normalisée est intermédiaire
dans `artifacts/contract-generation`.

Les deux sorties sont volontairement **internal** et sont versionnées dans
Git. Elles constituent une dépendance d'implémentation, pas une surface que
les consommateurs peuvent appeler ou à laquelle ils peuvent se lier. Toute
adaptation vers `TorBoxClient`, les clients API ou les modèles publics est un
travail distinct, à concevoir et à tester explicitement.

Le fichier `contracts/generation/kiota.settings.json` est l'autorité de la
configuration : clients, namespaces, sorties, sérialiseurs, désérialiseurs,
types MIME structurés et règles de normalisation. Le manifeste d'outils local
épingle `microsoft.openapi.kiota` à la version **1.34.1** ; le script vérifie
que cette version est identique à celle des paramètres et du `kiota-lock.json`
produit. Le projet SDK référence séparément `Microsoft.Kiota.Bundle`, qui est
le runtime requis par le code généré ; ce package ne remplace pas l'outil local
Kiota utilisé pour générer les fichiers.

## Normalisation limitée et revue

La fonction `Normalize-OpenApiForKiota` de
`eng/Invoke-GenerateInternalContracts.ps1` est le normaliseur versionné. Elle
est volontairement limitée aux propriétés invalides déclarées directement sur
des réponses `404` :

- pour Main, elle retire uniquement `success`, `error` et `detail` des objets
  de réponse `404`, et exige exactement **279** suppressions ;
- pour Relay, aucune propriété n'est retirée et le nombre attendu est **0**.

Cette opération rend le document acceptable pour Kiota sans modifier le
snapshot source. Elle n'ajoute pas de schéma de réponse, ne corrige pas des
routes, ne fabrique pas de modèle et ne doit pas devenir un overlay général.
Une nouvelle normalisation exige une preuve de contrat, une revue du diff et
une décision appropriée ; elle ne doit pas masquer une divergence relevée par
[DEC-004](decisions.md).

Ne modifiez jamais directement les fichiers sous
`src/TorBoxSDK/Internal/Generated/`. Kiota utilise `--clean-output` : une
modification manuelle serait écrasée à la prochaine génération et doit plutôt
être traitée dans le snapshot revu, la configuration limitée ou un adaptateur
manuel hors de cette sortie.

Après Kiota, le script retire de façon déterministe les espaces de fin de ligne
que cette version du générateur écrit dans certains fichiers C#. Cette étape de
formatage est versionnée dans le script, s'applique à toute sortie générée et
ne change pas la sémantique du contrat.

## Générer et vérifier

Depuis la racine du dépôt, lancer :

```powershell
pwsh ./eng/Invoke-GenerateInternalContracts.ps1
```

Le script restaure les outils locaux, valide les prérequis ci-dessus, génère
Main et Relay avec les types `Internal` et les données supplémentaires activées,
puis écrit les sorties versionnées. Une première restauration d'outil peut
utiliser les sources NuGet configurées si le cache local est vide ; la
génération elle-même ne télécharge aucun contrat TorBox.

Examiner ensuite les modifications, en particulier les fichiers générés et les
`kiota-lock.json`, avant de les ajouter explicitement :

```powershell
git status --short -- src/TorBoxSDK/Internal/Generated
git diff --check
git diff -- src/TorBoxSDK/Internal/Generated
```

Après revue et ajout des sorties concernées, exécuter la vérification :

```powershell
pwsh ./eng/Invoke-GenerateInternalContracts.ps1 -Verify
git diff --cached --check
git diff --cached -- src/TorBoxSDK/Internal/Generated
```

`-Verify` régénère d'abord les sorties, puis échoue si l'une d'elles est
non suivie par Git ou si la génération a modifié l'arbre de travail par rapport
à l'index. Il ne remplace donc ni l'examen du diff indexé ni le commit des
fichiers revus. Lancer cette commande dans un worktree dédié : elle emploie
`--clean-output` et est conçue pour prouver la reproductibilité des sorties,
pas pour préserver des éditions locales non revues.

## Avertissements Kiota connus

Les avertissements suivants sont actuellement attendus et doivent rester
visibles lors d'une régénération. Ils décrivent des limites des snapshots,
non des corrections appliquées silencieusement par le normaliseur.

| Source | Avertissement | Conséquence attendue |
|---|---|---|
| Main | Trois avertissements signalent un corps de requête déclaré sur `GET /v1/api/torrents/checkcached`, `GET /v1/api/webdl/checkcached` et `GET /v1/api/usenet/checkcached`. | Vérifier que le snapshot amont et le comportement effectif justifient ces corps ; ne pas supprimer ou promouvoir une signature publique à partir du seul avertissement. |
| Relay | Deux avertissements signalent que le snapshot ne déclare pas de `servers`/URL de base. | Ne pas introduire d'URL de repli dans la génération ; relever toute correction de contrat dans la baseline et sa revue. |

Le script génère avec un niveau de journalisation `Warning`, affiche ces
messages et arrête le workflow si Kiota retourne un code d'échec. Il échoue
également si la sortie contient un `OpenAPI error`, même lorsque Kiota aurait
retourné un code de succès. Un avertissement nouveau, supprimé ou dont le sens
change doit être analysé avec le diff du snapshot plutôt que considéré comme
normal par défaut.

## Limites explicites

- **Search est exclu.** La baseline enregistre `search-openapi` comme
  indisponible ; aucun contrat Search n'est généré ni substitué. Cela ne rend
  pas Search accessible : conformément à [DEC-005](decisions.md), il demeure
  réservé aux projets approuvés et aux IP sortantes autorisées par TorBox.
- **La façade et les modèles publics sont exclus.** Cette génération ne crée
  ni ne modifie automatiquement `TorBoxClient`, les clients Main/Search/Relay
  publics, leurs interfaces ou leurs modèles exposés.
- **Les fixtures de réponse sont exclues.** La baseline OpenAPI ne prouve pas
  les formes de réponse réelles et V2-200 n'en ajoute aucune. Leur capture,
  anonymisation et conservation restent soumises à [DEC-011](decisions.md).
- **Les téléchargements réseau et l'extension automatique de contrat sont
  exclus.** Une nouvelle capture suit d'abord la procédure de la baseline et
  les décisions de contrat ; la génération seule n'autorise aucune extension
  de l'API.

## Mettre à jour le workflow

1. Mettre à jour une baseline uniquement après la procédure de revue décrite
   dans la [baseline V2-110](contract-baseline.md), avec provenance et hash.
2. Examiner les impacts du snapshot sur la normalisation et les avertissements.
   Une variation du nombre attendu de suppressions est un échec volontaire,
   pas un nombre à ajuster mécaniquement.
3. Si une évolution de Kiota ou de ses options est approuvée, mettre à jour le
   manifeste d'outils et `kiota.settings.json` de manière cohérente, puis
   régénérer, revoir et versionner les sorties et leurs lockfiles.
4. Exécuter `-Verify`, conserver la preuve dans la carte V2-200 et traiter
   séparément toute incidence sur le runtime `Microsoft.Kiota.Bundle`, les
   adaptateurs internes ou la surface publique.

Pour les règles générales de branche, de rebase et de preuves de handoff,
consulter le [guide de développement v2](v2-development-workflow.md).
