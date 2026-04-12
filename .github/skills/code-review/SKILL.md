---
name: code-review
description: "Perform an exhaustive TorBoxSDK code review. Use when: reviewing C# files, auditing SDK code, check my code, code review, validate a development, check my PR, inspect src/ tests/ or samples/. Produces a structured report with severity-rated findings and a final verdict."
argument-hint: "File path, folder, or description of what to review (e.g. 'src/TorBoxSDK/Clients/TorrentsClient.cs')"
---

# TorBoxSDK Code Review Skill

## Purpose

Produce an exhaustive, structured code review for any C# file in TorBoxSDK. The review must be thorough, uncompromising, and actionable. Every finding references an exact line number and explains the impact and expected fix.

## When to Use

- Reviewing one or more `.cs` files before merging
- Auditing a complete folder (`src/`, `tests/`, or `samples/`)
- Validating a new feature implementation against the SDK conventions
- Checking that a generated model matches the TorBox API shape

## Step-by-Step Workflow

### Step 1 — Identify the target

Determine what to review:
1. If a file path is given, review that file.
2. If a folder is given, identify all `.cs` files within it and review them in logical order: models → services → clients → tests → samples.
3. If unclear, ask: _"Which file or folder should I review?"_

### Step 2 — Load the coding standards

Before writing a single line of the review, read `.github/instructions/csharp-conventions.instructions.md` in full. Then use [the instruction map](./references/instruction-map.md) to identify which Parts of that file apply to the target (Part 1 always applies; Parts 2–5 depend on file path).

Do NOT skip this step.

### Step 3 — Read all lines of the target file

Read the entire file. Do not skim. For each of the following dimensions, actively scan and note violations:

1. **Correctness** — logic, null dereferences, wrong returns, `async void`
2. **Naming** — PascalCase, camelCase, `_field`, `Async` suffix, interface `I` prefix
3. **Nullability** — explicit annotations, no unjustified `!` suppressions
4. **Type inference** — prefer explicit types; `var` only when obvious
5. **Modern C#** — file-scoped ns, primary constructors, expression-bodied, pattern matching
6. **Async patterns** — `ConfigureAwait(false)`, `CancellationToken`, no `.Result`/`.Wait()`
7. **Architecture** — client hierarchy, no hardcoded URLs, no manual auth
8. **Response handling** — `TorBoxResponse<T>`, typed exceptions on failure
9. **Security** — no hardcoded secrets, no log injection, input validation at boundaries
10. **Performance** — no sync I/O, no unnecessary allocations, `HttpResponseMessage` disposed
11. **File organization** — one type per file, filename matches type name
12. **XML documentation** — all public members documented

For tests, also check: naming pattern, AAA structure, `[Fact]`/`[Theory]` usage, `HttpClient` mocking, test isolation.  
For samples, also check: no hardcoded API key, `AddTorBox()` DI, error handling shown, self-explanatory code.

See [full severity guide](./references/severity-guide.md) to rate each finding.

### Step 4 — Produce the report

Use the [report template](./references/report-template.md) exactly. Do not invent a different format.

Rules:
- Reference **exact line numbers** for every finding
- Rate every finding with the correct severity: 🔴 CRITICAL / 🟠 MAJOR / 🟡 MINOR / 🔵 NITPICK
- If a dimension has zero findings, add a one-line ✅ confirmation that it was checked
- End with a **Verdict** using one of: `APPROVED` / `APPROVED WITH MINOR ISSUES` / `CHANGES REQUESTED` / `BLOCKED`

See [verdict guide](./references/severity-guide.md#verdict-rules) for verdict selection rules.

### Step 5 — Multi-file reviews

When reviewing multiple files:
1. Review them one by one, in order: models → services → clients → tests → samples
2. Produce one report section per file
3. Add a global summary at the end with aggregate verdict

## Resources

- [Instruction map](./references/instruction-map.md) — which instructions to load per file path
- [Severity guide](./references/severity-guide.md) — CRITICAL/MAJOR/MINOR/NITPICK definitions with examples
- [Report template](./references/report-template.md) — exact output format to use
- [Review workflow](./references/review-workflow.md) — detailed per-dimension checklists

## Constraints

- DO NOT edit or rewrite any code.  
- DO NOT approve code that has CRITICAL or MAJOR findings.  
- DO NOT skip any dimension because it "looks fine" — confirm it was checked explicitly.  
- ONLY review C# files (`.cs`, `.csproj`). Decline non-C# reviews politely.
