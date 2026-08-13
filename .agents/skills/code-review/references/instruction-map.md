# Instruction Map — Which Rules to Apply

> **Generic rules live in:** `.github/instructions/csharp-conventions.instructions.md`
> This file is organized in 5 Parts. Apply the relevant Parts based on the file path being reviewed.

For V2 paths, also read `.github/instructions/torboxsdk-v2.instructions.md`.
It supersedes only the legacy Part 2 response/exception requirements for
`src/TorBoxSDK.V2/` and `src/TorBoxSDK.DependencyInjection.V2/`; Part 1 and
every other applicable Part remains in force. V1 paths keep all legacy Part 2
rules.

---

## Path → Parts to Apply

| File path pattern | Parts to apply |
|-------------------|----------------|
| Any `.cs` file | **Part 1** — Base C# (naming, nullability, `var`, modern C#, async, file organization) |
| `src/**/*.cs` | + **Part 2** — SDK architecture, HTTP, auth, DI, response handling, performance, security, XML docs |
| `src/**/Models/**/*.cs` | + **Part 3** — Response shape, record vs class, JSON serialization, immutability, enums, request validation |
| `tests/**/*.cs` | + **Part 4** — xUnit naming, AAA, attributes, assertions, HttpClient mocking, test isolation, integration/perf |
| `samples/**/*.cs` | + **Part 5** — API key handling, `AddTorBox()`, error handling, no magic values, prohibited patterns |
| `src/TorBoxSDK.V2/**/*.cs` or `src/TorBoxSDK.DependencyInjection.V2/**/*.cs` | Applicable generic Parts + `torboxsdk-v2.instructions.md` (only its response/exception override takes precedence) |
| `tests/TorBoxSDK.V2.*/**/*.cs` | Part 1 + Part 4 + `torboxsdk-v2.instructions.md` |

---

## Decision Tree

```
Is the file in tests/?
├── YES → Apply Part 1 + Part 4
└── NO
    Is the file in samples/?
    ├── YES → Apply Part 1 + Part 5
    └── NO (file is in src/)
        Is the path under Models/?
        ├── YES → Apply Part 1 + Part 2 + Part 3
        └── NO  → Apply Part 1 + Part 2
```

---

## Quick Reference per Resource Area

| Area | Path | Parts |
|------|------|-------|
| Root client, options, handlers | `src/TorBoxSDK/` | 1 + 2 |
| API clients (Main, Search, Relay) | `src/TorBoxSDK/Clients/` | 1 + 2 |
| Resource clients (Torrents, Usenet…) | `src/TorBoxSDK/Clients/Main/` | 1 + 2 |
| Request/Response/data models | `src/TorBoxSDK/Models/` | 1 + 2 + 3 |
| Exceptions | `src/TorBoxSDK/Exceptions/` | 1 + 2 |
| DI extensions | `src/TorBoxSDK/Extensions/` | 1 + 2 |
| Unit tests | `tests/TorboxSDK.UnitTests/` | 1 + 4 |
| Integration tests | `tests/TorBoxSDK.IntegrationTests/` | 1 + 4 |
| Schema validation tests | `tests/TorBoxSDK.SchemaValidationTests/` | 1 + 4 |
| Samples | `samples/` | 1 + 5 |
