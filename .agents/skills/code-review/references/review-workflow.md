# Code Review Workflow — Dimension Checklist

> **Source of rules:** All coding rules live in `.github/instructions/csharp-conventions.instructions.md`.  
> This file contains only the **review structure** (which dimensions to check, in what order). Do not duplicate rules here.

---

## Before You Start

1. Read `.github/instructions/csharp-conventions.instructions.md` in full.
2. Identify the file path to determine which Parts apply:

| File path | Parts to apply |
|-----------|---------------|
| Any `.cs` file | Part 1 — Base C# |
| `src/**/*.cs` | + Part 2 — SDK Architecture |
| `src/**/Models/**/*.cs` | + Part 3 — Models |
| `tests/**/*.cs` | + Part 4 — Tests |
| `samples/**/*.cs` | + Part 5 — Samples |

---

## Dimension Checklist

Work through each dimension in order. For each one: find violations, note line numbers, rate severity using the [severity guide](./severity-guide.md).

### D1 — Correctness
Checks that are NOT in the instruction file (code logic):
- [ ] All conditions use the correct operator
- [ ] No off-by-one on index access
- [ ] No unreachable code after `return`, `throw`, `break`
- [ ] No fire-and-forget `Task` without `_ =` or explicit intent comment
- [ ] Switch expressions have a `_` default case
- [ ] No integer overflow on arithmetic with API-provided ids or sizes

### D2 — Naming
→ Rules in **Part 1 § Naming**

### D3 — Nullability
→ Rules in **Part 1 § Nullability**

### D4 — Type Inference
→ Rules in **Part 1 § Type Inference**

### D5 — Modern C#
→ Rules in **Part 1 § Modern C# Features**

### D6 — Async/Await
→ Rules in **Part 1 § Async/Await**  
Note: `.Result` / `.Wait()` in `src/` is always **CRITICAL**.

### D7 — SDK Architecture *(src/ only)*
→ Rules in **Part 2** — client hierarchy, HTTP, auth, DI

### D8 — Response Handling *(src/ only)*
→ Rules in **Part 2 § Response Handling**

### D9 — Security
→ Rules in **Part 2 § Security**  
Note: hardcoded secrets and log injection are always **CRITICAL**.

### D10 — Performance
→ Rules in **Part 2 § Performance**

### D11 — File Organization
→ Rules in **Part 1 § File Organization**

### D12 — XML Documentation *(src/ only)*
→ Rules in **Part 2 § XML Documentation**

### D13 — Model Contracts *(src/**/Models/ only)*
→ Rules in **Part 3** — response shape, record vs class, JSON, immutability, enums, request validation

### D14 — Test Quality *(tests/ only)*
→ Rules in **Part 4** — naming, AAA, attributes, assertions, HttpClient mocking, isolation

### D15 — Sample Quality *(samples/ only)*
→ Rules in **Part 5** — API key handling, DI, error handling, no magic values, prohibited patterns

---

## Dimension Status Table (paste into report)

Copy this into the Dimension Check section of your report and fill in ✅ or ⚠️:

```
| D1  Correctness         | ✅ |
| D2  Naming              | ✅ |
| D3  Nullability         | ✅ |
| D4  Type inference      | ✅ |
| D5  Modern C#           | ✅ |
| D6  Async/Await         | ✅ |
| D7  Architecture        | ✅ |   ← src/ only
| D8  Response handling   | ✅ |   ← src/ only
| D9  Security            | ✅ |
| D10 Performance         | ✅ |
| D11 File organization   | ✅ |
| D12 XML documentation   | ✅ |   ← src/ only
| D13 Model contracts     | ✅ |   ← Models/ only
| D14 Test quality        | ✅ |   ← tests/ only
| D15 Sample quality      | ✅ |   ← samples/ only
```
