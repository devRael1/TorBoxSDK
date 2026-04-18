# Plan : Documentation DocFX pour TorBoxSDK

## TL;DR
Mettre en place un site de documentation DocFX dans le dossier `docs/` existant, avec le template Material, la référence API auto-générée depuis les commentaires XML `///`, et les docfx-companion-tools (DocFxTocGenerator + DocLinkChecker). Déploiement sur GitHub Pages via un workflow `workflow_dispatch`.

---

## Phase 1 — Structure DocFX et configuration

### Étape 1 : Réorganiser le dossier `docs/`
Les 5 fichiers markdown existants deviennent les articles du site. Nouvelle structure :

```
docs/
  docfx.json              # Config principale DocFX
  index.md                # Landing page du site
  toc.yml                 # Navigation top-level (Articles, API Reference)
  articles/
    toc.yml               # TOC des guides (auto-générable via DocFxTocGenerator)
    getting-started.md    # ← déplacé depuis docs/
    configuration.md      # ← déplacé depuis docs/
    error-handling.md     # ← déplacé depuis docs/
    architecture.md       # ← déplacé depuis docs/
    api-reference.md      # ← déplacé depuis docs/ (guide manuel, complémente l'API auto-générée)
  api/
    .gitignore            # Ignorer les fichiers auto-générés par `docfx metadata`
    index.md              # Page d'intro de la référence API
  templates/
    material/
      public/
        main.css          # CSS du template Material (copié depuis le release)
  _site/                  # Output DocFX (gitignored)
```

**Fichiers déplacés** :
- `docs/getting-started.md` → `docs/articles/getting-started.md`
- `docs/configuration.md` → `docs/articles/configuration.md`
- `docs/error-handling.md` → `docs/articles/error-handling.md`
- `docs/architecture.md` → `docs/articles/architecture.md`
- `docs/api-reference.md` → `docs/articles/api-reference.md`

### Étape 2 : Créer `docs/docfx.json`
Configuration principale incluant :
- **metadata** : pointe vers `../src/TorBoxSDK/TorBoxSDK.csproj` pour générer la référence API
  - `filter` : exclure les types `internal` (seuls les types publics dans la doc)
  - `dest` : `api/` 
- **build** :
  - `content` : articles + api + toc.yml + index.md
  - `resource` : images, assets
  - `template` : `["default", "modern", "templates/material"]`
  - `globalMetadata` :
    - `_appTitle` : "TorBoxSDK"
    - `_appFooter` : "TorBoxSDK — Unofficial C# SDK for TorBox API"
    - `_enableSearch` : true
    - `_gitContribute` : lien vers le repo GitHub
  - `postProcessors` : `["ExtractSearchIndex"]`
  - `dest` : `_site`

### Étape 3 : Créer `docs/index.md`
Landing page avec :
- Titre et description du SDK
- Badges (NuGet, License, .NET)
- Liens rapides vers Getting Started, API Reference, Architecture
- Inspiré du README.md existant mais adapté pour le site de doc

### Étape 4 : Créer `docs/toc.yml`
Navigation top-level :
```yaml
- name: Articles
  href: articles/
- name: API Reference
  href: api/
```

### Étape 5 : Créer `docs/articles/toc.yml`
Table des matières des guides :
```yaml
- name: Getting Started
  href: getting-started.md
- name: Configuration
  href: configuration.md
- name: Error Handling
  href: error-handling.md
- name: Architecture
  href: architecture.md
- name: API Reference Guide
  href: api-reference.md
```

### Étape 6 : Créer `docs/api/index.md`
Page d'introduction de la référence API auto-générée, expliquant la structure (ITorBoxClient → Main/Search/Relay).

---

## Phase 2 — Template Material

### Étape 7 : Installer le template Material
- Télécharger le dossier `material/` depuis le release v1.0.0 de `ovasquez/docfx-material`
- Copier dans `docs/templates/material/`
- Le dossier contient `public/main.css` (customisable)
- Vérifier que `docfx.json` référence `["default", "modern", "templates/material"]`

### Étape 8 : Personnaliser les couleurs (optionnel)
- Modifier `docs/templates/material/public/main.css` pour adapter les couleurs au branding TorBoxSDK si souhaité

---

## Phase 3 — docfx-companion-tools

### Étape 9 : Installer les companion tools
Installer via dotnet tool (global ou manifest) :
```bash
dotnet tool install DocFxTocGenerator -g
dotnet tool install DocLinkChecker -g
```

Créer un fichier `.config/dotnet-tools.json` (tool manifest) pour versionner les outils :
```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "docfx": { "version": "2.78.5", "commands": ["docfx"] },
    "docfxtocgenerator": { "version": "1.40.0", "commands": ["DocFxTocGenerator"] },
    "doclinkchecker": { "version": "1.40.0", "commands": ["DocLinkChecker"] }
  }
}
```

### Étape 10 : Configurer DocFxTocGenerator
- Utilisable pour régénérer automatiquement `docs/articles/toc.yml` depuis la structure du dossier
- Commande : `DocFxTocGenerator -d docs/articles -vs`
- Optionnel : ajouter un fichier `.order` dans `docs/articles/` pour contrôler l'ordre des entrées

### Étape 11 : Configurer DocLinkChecker
- Valide les liens dans les fichiers markdown et détecte les attachments orphelins
- Commande : `DocLinkChecker -d docs -va`
- Intégrable au workflow CI ou au workflow de documentation

---

## Phase 4 — .gitignore et build local

### Étape 12 : Mettre à jour `.gitignore`
Ajouter :
```
# DocFX
docs/_site/
docs/api/*.yml
docs/api/.manifest
```
- `_site/` : output HTML généré
- `api/*.yml` : fichiers YAML de métadonnées auto-générés par `docfx metadata`

### Étape 13 : Tester le build local
Commandes de vérification :
```bash
dotnet tool restore
docfx docs/docfx.json --serve
```
- Vérifie que le site se construit sans erreurs
- Preview sur http://localhost:8080

---

## Phase 5 — GitHub Actions Workflow

### Étape 14 : Créer `.github/workflows/docs.yml`
Workflow de déploiement **manual** (`workflow_dispatch`) :

```yaml
name: Documentation
on: workflow_dispatch

permissions:
  pages: write
  id-token: write

concurrency:
  group: "pages"
  cancel-in-progress: false

jobs:
  build-docs:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'
      - run: dotnet tool restore
      - run: dotnet tool run docfx docs/docfx.json
      - uses: actions/upload-pages-artifact@v3
        with:
          path: docs/_site
  
  deploy:
    needs: build-docs
    runs-on: ubuntu-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    steps:
      - id: deployment
        uses: actions/deploy-pages@v4
```

**Prérequis** : activer GitHub Pages en mode "GitHub Actions" dans les settings du repo.

---

## Fichiers concernés

| Fichier | Action |
|---------|--------|
| `docs/docfx.json` | **Nouveau** — Configuration DocFX principale |
| `docs/index.md` | **Nouveau** — Landing page du site |
| `docs/toc.yml` | **Nouveau** — Navigation top-level |
| `docs/articles/toc.yml` | **Nouveau** — TOC des guides |
| `docs/articles/getting-started.md` | **Déplacé** depuis `docs/` |
| `docs/articles/configuration.md` | **Déplacé** depuis `docs/` |
| `docs/articles/error-handling.md` | **Déplacé** depuis `docs/` |
| `docs/articles/architecture.md` | **Déplacé** depuis `docs/` |
| `docs/articles/api-reference.md` | **Déplacé** depuis `docs/` |
| `docs/api/index.md` | **Nouveau** — Intro référence API |
| `docs/api/.gitignore` | **Nouveau** — Ignorer les fichiers auto-générés |
| `docs/templates/material/public/main.css` | **Nouveau** — Template Material (copié depuis release) |
| `.config/dotnet-tools.json` | **Nouveau** — Tool manifest (docfx + companion tools) |
| `.gitignore` | **Modifié** — Ajouter `docs/_site/`, `docs/api/*.yml` |
| `.github/workflows/docs.yml` | **Nouveau** — Workflow déploiement GitHub Pages |

## Vérification

1. `dotnet tool restore` — installe docfx et les companion tools depuis le manifest
2. `docfx docs/docfx.json --serve` — le site se construit et s'affiche sur localhost:8080
3. Vérifier que la référence API auto-générée liste les types publics : `ITorBoxClient`, `TorBoxClient`, `TorBoxClientOptions`, `IMainApiClient`, `ISearchApiClient`, `IRelayApiClient`, et tous les modèles publics
4. Vérifier que les 5 articles existants sont accessibles dans la navigation
5. Vérifier que le template Material est appliqué (design Material)
6. `DocLinkChecker -d docs -va` — aucun lien cassé dans les articles
7. `DocFxTocGenerator -d docs/articles -vs` — TOC généré correspond à la structure
8. Déclencher le workflow `docs.yml` via `workflow_dispatch` et vérifier le déploiement sur GitHub Pages

## Phase 6 — Mise à jour des skills, agent et contenu

### Étape 15 : Mettre à jour le skill docs (`SKILL.md`)
Ajouter à `.github/skills/docs/SKILL.md` un nouveau job type (ex: **D7 — DocFX Documentation Site**) couvrant :
- Les commandes DocFX (`docfx metadata`, `docfx build`, `docfx serve`)
- Les companion tools (`DocFxTocGenerator`, `DocLinkChecker`)
- La structure attendue du dossier `docs/` (articles, api, templates, toc.yml, docfx.json)
- Les conventions de contribution aux articles (frontmatter, liens relatifs, images)
- La relation entre les commentaires XML `///` et la référence API auto-générée
- Le workflow de déploiement GitHub Pages (`docs.yml`)

### Étape 16 : Mettre à jour l'agent docs (`docs.agent.md`)
Enrichir `.github/agents/docs.agent.md` avec :
- **Focus** : ajouter la gestion du site DocFX (build, preview, structure des articles et de l'API auto-générée)
- **Scope** : ajouter les tâches DocFX (création/mise à jour d'articles dans `docs/articles/`, gestion de `docfx.json`, `toc.yml`, template Material, companion tools)
- **Workflow** : intégrer les étapes DocFX (vérifier le build local avec `docfx --serve`, valider les liens avec `DocLinkChecker`, régénérer les TOC avec `DocFxTocGenerator`)
- **Constraints** : ne pas modifier les fichiers auto-générés dans `docs/api/`, ne pas déployer sans passer par le workflow `docs.yml`
- **Mermaid Guidance** : indiquer que les diagrammes Mermaid sont supportés nativement par DocFX dans les articles

### Étape 17 : Analyser et adapter le contenu des articles existants
Pour chaque fichier déplacé dans `docs/articles/`, vérifier et adapter :
- **Frontmatter** : ajouter un header YAML DocFX (`uid`, `title`, `description`) si nécessaire
- **Liens internes** : mettre à jour les chemins relatifs (les fichiers ont changé de niveau, ex: `error-handling.md` → reste local, mais les liens vers d'autres fichiers ou vers l'API doivent être vérifiés)
- **Cross-références API** : convertir les mentions de types SDK (`ITorBoxClient`, `TorBoxClientOptions`, etc.) en cross-références DocFX (`@TorBoxSDK.ITorBoxClient` ou `<xref:TorBoxSDK.ITorBoxClient>`) pour créer des liens vers la référence API auto-générée
- **Images et assets** : vérifier que les chemins vers d'éventuelles images sont corrects après le déplacement
- **Cohérence** : s'assurer que le contenu est à jour par rapport au SDK actuel (modèles, clients, options)

---

## Décisions

- Les fichiers markdown existants sont **déplacés** dans `docs/articles/` (pas copiés) pour éviter la duplication
- `docs/api-reference.md` (guide manuel) est conservé car il documente les méthodes et paramètres de façon narrative — il complète l'API auto-générée qui est plus technique
- Le template Material est committé dans le repo (`docs/templates/material/`) plutôt qu'installé dynamiquement, conformément aux instructions officielles du template
- Les outils (docfx + companion tools) sont déclarés dans un **tool manifest** (`.config/dotnet-tools.json`) pour reproductibilité
- Le workflow est **manual** (`workflow_dispatch`) comme demandé — pas de déploiement automatique sur push
- Les fichiers auto-générés par `docfx metadata` (YAML dans `api/`) sont gitignorés car ils sont régénérés à chaque build

## Considérations

1. **Liens internes dans les articles** : Les fichiers markdown existants peuvent contenir des liens relatifs entre eux (ex: `[configuration](configuration.md)`). Après le déplacement dans `articles/`, ces liens resteront valides car ils sont dans le même dossier. Les liens depuis le README.md vers `docs/` devront éventuellement être mis à jour vers `docs/articles/`.

2. **`docs/api-reference.md` vs API auto-générée** : Le guide API manuel existant est complet et bien écrit. On pourrait le garder comme un "API Overview" narratif dans les articles, tandis que la section API auto-générée fournit la doc technique exhaustive. Alternative : supprimer le guide manuel au profit de l'auto-généré uniquement.

3. **Filtrage des types internes** : DocFX génèrera par défaut la doc pour tous les types `public`. Les types `internal` (comme `AuthHandler`, `TorBoxApiHelper`, etc.) seront exclus via un `filterConfig.yml`. À vérifier que seule la surface publique du SDK apparaît.
