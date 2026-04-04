---
name: docs
description: 'Use when documenting, packaging, or preparing TorBoxSDK for release: README updates, docs pages, samples guidance, XML documentation, Mermaid diagrams, NuGet metadata, and release readiness. Global entry point for all documentation jobs.'
argument-hint: 'Describe the documentation task, such as README quick start, XML docs for a client, architecture diagram, sample guidance, or NuGet release readiness.'
user-invocable: true
disable-model-invocation: false
---

# Docs

## Purpose

Global entry point for all documentation-related work in TorBoxSDK.

This skill owns the full documentation lifecycle:
- README and docs pages
- usage guides and architecture explanations
- samples guidance
- XML documentation on the public API
- package metadata and release-readiness documentation

## When to Use

Use this skill when you need to:
- improve the README
- create or update Markdown files under `docs/`
- add XML documentation comments to the public API
- document endpoint usage or SDK workflows
- create Mermaid diagrams for hierarchy, flow, or architecture
- review samples from a documentation and usability perspective
- prepare NuGet metadata and package documentation for release
- decide what documentation task should be done next

## Documentation Job Types

| Job | Trigger | Reference |
|-----|---------|-----------|
| **D1 — README** | installation, quick start, feature overview, badges | [README guidance](./references/readme-guidance.md) |
| **D2 — Docs Pages** | guides in `docs/`, architecture notes, contributor docs | [Docs pages guidance](./references/docs-pages-guidance.md) |
| **D3 — Samples** | sample scenarios, sample usefulness, sample documentation alignment | [Samples guidance](./references/samples-guidance.md) |
| **D4 — XML Docs** | `<summary>`, `<param>`, `<returns>`, public API discoverability | [XML docs guidance](./references/xml-docs-guidance.md) |
| **D5 — Packaging & Release** | NuGet metadata, SourceLink, release readiness, open-source quality | [Packaging and release checklist](./references/packaging-and-release-checklist.md) |
| **D6 — Architecture & Diagrams** | Mermaid diagrams, client hierarchy, request flows | [Architecture docs and diagrams](./references/architecture-docs-and-diagrams.md) |

## Workflow

1. Identify the audience.
Choose whether the task serves:
- first-time SDK consumers
- application developers integrating with DI
- contributors to the SDK
- maintainers preparing a release

2. Classify the documentation job.
Map the request to one or more D1–D6 jobs. If multiple jobs apply, execute them in this order:

```
D6 Architecture context → D1 README / D2 Docs Pages → D3 Samples → D4 XML Docs → D5 Packaging & Release
```

3. Read the relevant code and roadmap first.
Documentation must reflect the actual or explicitly planned SDK surface. Inspect code, instructions, and `docs/TODO.md` before writing.

4. Load only the relevant references.
Use the job table above. Do not load every reference by default.

5. Write from the public API.
Prefer examples and explanations based on:
- `TorBoxClient`
- `Main`, `Search`, `Relay`
- real request and response models
- real error handling (`TorBoxException`)

6. Validate documentation accuracy.
Confirm that:
- examples compile conceptually against the current or planned SDK shape
- docs do not mention non-existent APIs
- diagrams match the actual hierarchy
- packaging notes match the csproj and release setup

## Output Contract

Every `/docs` execution should end with:

1. **Job classification** — which D1–D6 jobs apply
2. **Audience** — who the documentation is for
3. **Deliverables** — files or sections to add/update
4. **Validation notes** — what was verified against code, roadmap, or packaging settings

## References

- [README guidance](./references/readme-guidance.md)
- [Docs pages guidance](./references/docs-pages-guidance.md)
- [Samples guidance](./references/samples-guidance.md)
- [XML docs guidance](./references/xml-docs-guidance.md)
- [Packaging and release checklist](./references/packaging-and-release-checklist.md)
- [Architecture docs and diagrams](./references/architecture-docs-and-diagrams.md)
