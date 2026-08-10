# README Guidance

Use this reference for **D1 — README**.

## Purpose

Keep `README.md` practical, accurate, and easy to scan for a first-time SDK consumer.

## README must answer quickly

- What is this SDK for?
- Which TorBox APIs are covered?
- Which .NET versions are supported?
- How do I install it?
- How do I make the first call?
- How do I use it with dependency injection?

## Recommended README structure

1. Project summary
2. Supported APIs and .NET targets
3. Installation
4. Quick start
5. DI example with `AddTorBox()`
6. Main/Search/Relay overview
7. Error handling basics
8. Links to docs and samples

## Accuracy rules

- Examples must reflect the real public surface.
- Namespace imports must match the actual package layout.
- Async examples should include `CancellationToken` when useful.
- Avoid documenting internal helpers.
