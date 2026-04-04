# Samples Guidance

Use this reference for **D3 — Samples**.

## Purpose

Keep sample work realistic, documentation-friendly, and aligned with the actual SDK workflows.

## Sample priorities

Prefer realistic flows over isolated fragments:

- create `TorBoxClient` and call `Main.User`
- configure `AddTorBox()` in a DI container
- create a torrent then inspect status
- call Search API then pass identifiers into Main API workflows

## Sample review questions

- Does the sample reflect the real public surface?
- Is the workflow useful to a real consumer?
- Does it show error handling and async usage correctly?
- Would a new user understand what to copy and what to replace?

## Constraints

- Do not document samples that rely on non-existent APIs.
- Do not treat internal helpers as public sample surface.
- Prefer end-to-end examples over disconnected snippets.
