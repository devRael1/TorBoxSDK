---
name: dev
description: "Orchestrate TorBoxSDK development work. Use when: starting or planning a development task, choosing the next job, implementing a feature, adding an endpoint, writing tests, reviewing code, updating docs, preparing a release, or when unsure which skill to use. Entry point for all SDK dev jobs and skill orchestration."
argument-hint: "Describe the work to do, e.g. 'implement Torrents.CreateTorrentAsync', 'add unit tests for UserClient', 'prepare NuGet release', 'scaffold Phase 1 foundations'"
---

# TorBoxSDK — Dev Skill

## Purpose

Entry point for all development work on TorBoxSDK. Identifies the job type, selects the appropriate specialized skill, and provides the workflow to execute it correctly.

Every development task falls into one of six recurring job types. This skill tells you which job applies and how to execute it.

This skill does not replace the specialized skills. It orchestrates them, decides the right execution order, and defines the expected outputs and handoffs between them.

## Integrated Skills

The global `/dev` workflow integrates these specialized skills:

| Skill | Role | File |
|------|------|------|
| `architecture` | Client hierarchy, DI, namespaces, cross-cutting design | `.github/skills/architecture/SKILL.md` |
| `tests` | Unit, integration, serialization, performance tests | `.github/skills/tests/SKILL.md` |
| `code-review` | Final validation and severity-rated review | `.github/skills/code-review/SKILL.md` |
| `docs` | README, docs pages, samples guidance, XML docs, diagrams, NuGet/release quality | `.github/skills/docs/SKILL.md` |

Use `/dev` as the default entry point whenever the scope is not already obvious. Invoke a specialized skill directly only when the job type is already certain.

Endpoint implementation is now owned directly by `/dev` as **J2 — Endpoint** through its internal references and playbooks.

## Job Types

| Job | Trigger | Skill to load |
|-----|---------|---------------|
| **J1 — Architecture** | Designing or refactoring client structure, DI, namespacing, cross-cutting concerns | `architecture` skill |
| **J2 — Endpoint** | Adding or extending an API endpoint (models + client method + tests) | Internal `/dev` endpoint workflow |
| **J3 — Tests** | Writing unit, integration, schema validation, or performance tests for existing code | `tests` skill |
| **J4 — Review** | Reviewing or auditing C# code before merge | `code-review` skill |
| **J5 — Docs & Packaging** | README, samples, XML docs, NuGet metadata, release readiness | `docs` skill |
| **J6 — Foundation** | Scaffolding project infrastructure (csproj, build props, editorconfig, DI setup) | See [foundation jobs reference](./references/dev-jobs.md#j6--foundation-jobs) |

## Step-by-Step Workflow

### Step 1 — Classify the job

Read the request and match it to a job type using the table above. When the request spans multiple jobs (e.g., implement endpoint + add tests), execute them in the canonical order:

```
J1 Architecture → J6 Foundation → J2 Endpoint → J3 Tests → J4 Review → J5 Docs
```

### Step 2 — Load the specialized skill or internal workflow

For J1, J3, J4, and J5, load the corresponding skill file and follow its workflow. Do not duplicate specialized skill content here — delegate fully.

For J2, stay inside `/dev` and use these internal references:
- [Endpoint placement and naming](./references/endpoint-placement-and-naming.md)
- [Endpoint implementation checklist](./references/endpoint-implementation-checklist.md)
- [Development playbooks](./references/development-playbooks.md)

For J6, follow the [foundation jobs reference](./references/dev-jobs.md#j6--foundation-jobs) directly.

If the request spans multiple jobs, use the [development playbooks](./references/development-playbooks.md) to determine the exact chaining order and handoff criteria.

### Step 3 — Apply project-wide constraints

Regardless of job type, every change must respect:
- Multi-target: `net6.0;net7.0;net8.0;net9.0;net10.0`
- Nullable reference types enabled
- One public type per file, filename matches type name
- File-scoped namespaces
- Explicit types preferred over `var`
- All public async methods: `CancellationToken ct = default` as last parameter
- All coding rules in `.github/instructions/csharp-conventions.instructions.md`

### Step 4 — Define the expected output before coding

Before implementing, state what this `/dev` run must produce:
- the selected job type(s)
- the specialized skill(s) to load
- the deliverables to create or modify
- the completion checks that must pass before handoff

For standard multi-step work, use the [development playbooks](./references/development-playbooks.md).

### Step 5 — Verify completion

After implementing, verify the task is complete by checking:
1. Does it build without warnings on all targets? → `dotnet build`
2. Do unit tests pass? → `dotnet test`
3. Does it comply with the coding standards? → trigger `code-review` skill on changed files

If the task adds public capability, also decide whether it must hand off to `docs` for README, samples, XML docs, diagrams, or package metadata updates.

## Output Contract

Every `/dev` execution should end with these concrete outputs:

1. **Job classification** — which J1–J6 jobs apply
2. **Execution order** — which specialized skills run, and in what sequence
3. **Deliverables** — files, models, methods, tests, docs, or package changes expected
4. **Exit checks** — build, tests, review, and docs/release checks that close the task

## Resources

- [Job reference](./references/dev-jobs.md) — detailed job definitions, recurring cadence, and per-job checklists
- [Skill map](./references/skill-map.md) — which skill owns which type of work, with overlap resolution rules
- [Development playbooks](./references/development-playbooks.md) — standard multi-skill sequences for recurring work
- [Endpoint placement and naming](./references/endpoint-placement-and-naming.md) — ownership, placement, and naming rules for J2
- [Endpoint implementation checklist](./references/endpoint-implementation-checklist.md) — completion checklist for J2
- [Project roadmap](../../docs/TODO.md) — all pending tasks by phase

## Quick Decision Guide

```
Is this about the client structure, DI, or namespacing?
  YES → J1: load architecture skill

Is this about adding or modifying an API endpoint?
  YES → J2: stay in `/dev`, apply the endpoint references
         then J3: load tests skill for the new endpoint

Is this about writing tests only?
  YES → J3: load tests skill

Is this about reviewing code before merge?
  YES → J4: load code-review skill

Is this about README, samples, XML docs, or NuGet?
  YES → J5: load docs skill

Is this about project infrastructure (csproj, build, editorconfig)?
  YES → J6: follow foundation jobs reference

Is this a broader delivery slice with multiple steps (design + implementation + tests + review + docs)?
  YES → stay in `/dev` and use the development playbooks
```
