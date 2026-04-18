# Skill Map — Which Skill for Which Job

Quick reference to determine which skill to load based on the nature of the work.

> Default rule: enter via `/dev` when the request covers multiple steps, when the next job is unclear, or when a handoff between skills needs to be orchestrated.

---

## Overview

```
dev (orchestrator skill)
├── architecture       → client structure, DI, namespaces
├── J2 endpoint workflow → models + client method + HTTP wiring
├── tests              → unit tests, schema validation, integration, perf
├── code-review        → audit and validation before merge
└── docs               → README, docs pages, samples, XML doc, diagrams, NuGet
```

The `dev` skill does not replace these skills. It chooses which one to launch, in what order, and when to hand off work to the next skill.

---

## Decision Table

| Request | Skill | File |
|---------|-------|------|
| "Design the client hierarchy" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Refactor the DI" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Define a new namespace" | `architecture` | `.github/skills/architecture/SKILL.md` |
| "Implement endpoint X" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Add a request/response model" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Extend a resource client interface" | `dev` (J2) | `.github/skills/dev/SKILL.md` |
| "Write unit tests" | `tests` | `.github/skills/tests/SKILL.md` |
| "Write integration tests" | `tests` | `.github/skills/tests/SKILL.md` |
| "Update schema validation tests" | `tests` | `.github/skills/tests/SKILL.md` |
| "Add benchmarks" | `tests` | `.github/skills/tests/SKILL.md` |
| "Review this file" | `code-review` | `.github/skills/code-review/SKILL.md` |
| "Audit src/ before merge" | `code-review` | `.github/skills/code-review/SKILL.md` |
| "Improve the README" | `docs` | `.github/skills/docs/SKILL.md` |
| "Create a sample" | `docs` | `.github/skills/docs/SKILL.md` |
| "Prepare the NuGet release" | `docs` | `.github/skills/docs/SKILL.md` |
| "Add XML docs" | `docs` | `.github/skills/docs/SKILL.md` |
| "Create a Mermaid diagram" | `docs` | `.github/skills/docs/SKILL.md` |
| "Scaffold Phase 1" | `dev` (J6) | `.github/skills/dev/references/dev-jobs.md#j6` |
| "What should I do next?" | `dev` | `.github/skills/dev/SKILL.md` |
| "Plan the next batch of work" | `dev` | `.github/skills/dev/SKILL.md` |
| "Chain implementation + tests + review" | `dev` | `.github/skills/dev/references/development-playbooks.md` |

---

## Priority Rules for Overlapping Concerns

### J2 endpoint workflow vs architecture
- Implementing an endpoint that requires a **new resource client** → load `architecture` **first**, then run `J2` in `/dev`.
- Implementing an endpoint in an existing client → run `J2` in `/dev` directly.

### tests vs code-review
- Writing new tests → `tests`.
- Reviewing existing tests for quality/compliance → `code-review` (Part 4 of the instruction file).
- Both can be chained: `tests` to write, `code-review` to validate.

### J2 endpoint workflow vs tests
- Always chain: `J2` in `/dev` → `tests` → `code-review`.
- Do not consider an endpoint complete without running all three.

### dev vs all others
- If the request contains multiple action verbs or multiple deliverables, start with `dev`.
- If the request maps exactly to a single specialized skill without ambiguity, the specialized skill can be invoked directly.
- If `/dev` has already classified the job, the specialized skills take over for detailed execution.

### docs vs J2 endpoint workflow
- Samples (`docs`) must reflect endpoints that are already implemented and stable.
- Do not write a sample before the corresponding resource client has been validated by `code-review`.

---

## Available Agents

The following agents are configured to directly orchestrate these skills:

| Agent | Role | File |
|-------|------|------|
| `Dev` | SDK development and job orchestration | `.github/agents/dev.agent.md` |
| `Tests` | Test writing (J3) | `.github/agents/tests.agent.md` |
| `Code Reviewer` | Review and audit (J4) | `.github/agents/code-reviewer.agent.md` |
| `Docs` | Documentation and packaging (J5) | `.github/agents/docs.agent.md` |

---

## Reference Files

| File | Role |
|------|------|
| `.github/instructions/csharp-conventions.instructions.md` | Source of truth for C# rules (5 Parts) |
| `.github/skills/dev/references/dev-jobs.md` | Definition and checklists for the 6 job types |
| `docs/TODO.md` | Complete roadmap by phase (Phase 1→8) |
