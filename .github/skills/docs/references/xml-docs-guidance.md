# XML Docs Guidance

Use this reference for **D4 — XML Docs**.

## Purpose

Document the public C# API so consumers can understand intent, parameters, return values, and failure modes directly from IntelliSense.

## Required elements

For public classes, interfaces, methods, and properties:

- `<summary>` on every public type and member
- `<param>` for every non-obvious parameter
- `<returns>` for every `Task<T>` or value-returning method
- `<exception>` when a public method is expected to throw `TorBoxException` or another meaningful public exception

## Style rules

- Describe behavior, not implementation details.
- Prefer consumer language over internal transport language.
- Match parameter names exactly.
- Keep summaries short and specific.
