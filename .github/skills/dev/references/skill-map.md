# Skill Map — Quel skill pour quel travail

Référence rapide pour savoir quel skill charger selon la nature du travail.

> Règle par défaut : entrer via `/dev` quand la demande couvre plusieurs étapes, quand le prochain job n'est pas clair, ou quand un handoff entre skills devra être orchestré.

---

## Vue d'ensemble

```
dev (skill orchestrateur)
├── architecture       → structure client, DI, namespaces
├── J2 endpoint workflow → modèles + méthode client + câblage HTTP
├── tests              → tests unitaires, intégration, perf
├── code-review        → audit et validation avant merge
└── docs               → README, docs pages, samples, XML doc, diagrammes, NuGet
```

Le skill `dev` ne remplace pas ces skills. Il choisit lequel lancer, dans quel ordre, et quand faire passer le travail au skill suivant.

---

## Tableau de décision

| Demande | Skill | Fichier |
|---------|-------|---------|
| "Concevoir la hiérarchie client" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Refactorer le DI" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Définir un nouveau namespace" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Implémenter l'endpoint X" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Ajouter un modèle request/response" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Étendre une interface de resource client" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Écrire des tests unitaires" | `tests` | `.github/skills/tests/SKILL.md` |
| "Écrire des tests d'intégration" | `tests` | `.github/skills/tests/SKILL.md` |
| "Ajouter des benchmarks" | `tests` | `.github/skills/tests/SKILL.md` |
| "Relire ce fichier" | `code-review` | `.github/skills/code-review/SKILL.md` |
| "Auditer src/ avant merge" | `code-review` | `.github/skills/code-review/SKILL.md` |
| "Améliorer le README" | `docs` | `.github/skills/docs/SKILL.md` |
| "Créer un sample" | `docs` | `.github/skills/docs/SKILL.md` |
| "Préparer la release NuGet" | `docs` | `.github/skills/docs/SKILL.md` |
| "Ajouter des XML docs" | `docs` | `.github/skills/docs/SKILL.md` |
| "Créer un diagramme Mermaid" | `docs` | `.github/skills/docs/SKILL.md` |
| "Scaffolder la Phase 1" | `dev` (J6) | `.github/skills/dev/references/dev-jobs.md#j6` |
| "Que faire ensuite ?" | `dev` | `.github/skills/dev/SKILL.md` |
| "Planifier la prochaine tranche de travail" | `dev` | `.github/skills/dev/SKILL.md` |
| "Enchaîner implémentation + tests + review" | `dev` | `.github/skills/dev/references/development-playbooks.md` |

---

## Règles de priorité en cas de chevauchement

### J2 endpoint workflow vs architecture
- L'implémentation d'un endpoint qui nécessite un **nouveau resource client** → charger `architecture` **d'abord**, puis exécuter `J2` dans `/dev`.
- L'implémentation d'un endpoint dans un client existant → exécuter `J2` dans `/dev` directement.

### tests vs code-review
- Écriture de nouveaux tests → `tests`.
- Revue de tests existants pour qualité/conformité → `code-review` (Part 4 de l'instruction file).
- Les deux peuvent s'enchaîner : `tests` pour écrire, `code-review` pour valider.

### J2 endpoint workflow vs tests
- Toujours enchaîner : `J2` dans `/dev` → `tests` → `code-review`.
- Ne pas considérer un endpoint comme terminé sans avoir exécuté les trois.

### dev vs tous les autres
- Si la demande contient plusieurs verbes d'action ou plusieurs livrables, commencer par `dev`.
- Si la demande correspond exactement à un seul skill spécialisé et sans ambiguïté, le skill spécialisé peut être invoqué directement.
- Si `/dev` a déjà classé le job, les skills spécialisés prennent le relais pour l'exécution détaillée.

### docs vs J2 endpoint workflow
- Les samples (`docs`) doivent refléter des endpoints déjà implémentés et stables.
- Ne pas écrire de sample avant que le resource client correspondant soit validé par `code-review`.

---

## Agents disponibles

Les agents suivants sont configurés pour orchestrer directement ces skills :

| Agent | Rôle | Fichier |
|-------|------|---------|
| `Dev` | Développement SDK et orchestration des jobs | `.github/agents/dev.agent.md` |
| `Tests` | Écriture de tests (J3) | `.github/agents/tests.agent.md` |
| `Code Reviewer` | Review et audit (J4) | `.github/agents/code-reviewer.agent.md` |
| `Docs` | Documentation et packaging (J5) | `.github/agents/docs.agent.md` |

---

## Fichiers de référence

| Fichier | Rôle |
|---------|------|
| `.github/instructions/csharp-conventions.instructions.md` | Source de vérité des règles C# (5 Parts) |
| `.github/skills/dev/references/dev-jobs.md` | Définition et checklists des 6 job types |
| `docs/TODO.md` | Roadmap complète par phase (Phase 1→8) |
