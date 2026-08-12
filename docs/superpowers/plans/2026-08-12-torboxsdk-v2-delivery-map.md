# TorBoxSDK V2 Delivery Map

## Why V2 is split into plans

The approved V2 design covers four independently reviewable subsystems: the package and contract foundation, the Main API, Search and Relay, and the public release surface. Implementing them through one large plan would force endpoint work to guess at an API contract that has not yet been frozen.

The first plan creates the V2 projects, captures the reviewed contract baseline, and builds the deterministic contract gate. That makes every subsequent endpoint plan exact: its operation list comes from the committed baseline rather than from a live URL or stale memory.

## Execution order

| Order | Plan | Independently testable outcome | Required gate before the next plan |
| --- | --- | --- | --- |
| 1 | `2026-08-12-torboxsdk-v2-foundation-and-contract-plan.md` | A buildable, side-by-side V2 core with its final package identities, a frozen contract baseline, a deterministic contract test project, response/stream primitives, direct construction, and the optional DI package. | V2 solution builds across its target matrix; unit and contract tests are offline; no V2 test downloads OpenAPI. |
| 2 | Main API resource plans | Every Main resource listed in the coverage manifest has handwritten models, interfaces, client methods, unit tests, and a completed coverage record. | The contract gate reports no unimplemented Main operation and each resource passes review. |
| 3 | Search and Relay plan | Search and Relay have the same handwritten, contract-backed coverage as Main, including non-standard stream or status behavior. | The contract gate reports no unimplemented Search or Relay operation. |
| 4 | Documentation, migration, live validation, and cutover plan | V2 is moved from side-by-side projects to the final `TorBoxSDK` and `TorBoxSDK.DependencyInjection` projects, then packaged as 2.0.0. The existing DocFX structure receives the V2 reference, migration guide, and runnable examples; V1 documentation is explicitly historical. | Exact-package validation, docs/examples, migration guide, controlled live evidence, and release evidence meet the approved V2 criteria. |

## Side-by-side migration rule

Until the cutover plan, V2 lives in dedicated project paths such as `src/TorBoxSDK.V2/` while keeping the final assembly and NuGet identities. The V2 solution never references the V1 project, so both code lines can compile independently. V1 is removed only during the final cutover after V2 has passed its deterministic gates.

This is an implementation-safety boundary only. It does not create a public `TorBoxSDK.V2` package or a permanent parallel product.
