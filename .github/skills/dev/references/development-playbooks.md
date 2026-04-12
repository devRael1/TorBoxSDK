# Development Playbooks

Playbooks standardize the most common multi-skill sequences in TorBoxSDK. They avoid having to decide the workflow chain on every request.

> Business and technical rules remain in the specialized skills. This file defines only the order, handoffs, and exit criteria.

---

## Playbook A — Implement an endpoint in an existing client

**When to use:** an endpoint needs to be added to an already existing resource client.

**Sequence:**
1. `/dev` executes J2 with [endpoint-placement-and-naming.md](./endpoint-placement-and-naming.md) and [endpoint-implementation-checklist.md](./endpoint-implementation-checklist.md)
2. `tests`
3. `code-review`
4. `docs` if the endpoint is user-facing or warrants a sample/README update

**Expected inputs:**
- API contract or TorBox doc
- target resource client already identified

**Expected outputs:**
- request/response models
- interface method and client implementation
- unit tests for success/failure/mapping
- review with no CRITICAL or MAJOR finding

**Exit criteria:**
- build OK
- tests OK
- review verdict: `APPROVED` or `APPROVED WITH MINOR ISSUES`

---

## Playbook B — Add a new resource slice

**When to use:** a new capability family requires structure + models + endpoints + tests.

**Sequence:**
1. `architecture`
2. `/dev` executes J2 endpoint workflow
3. `tests`
4. `code-review`
5. `docs`

**Expected inputs:**
- functional requirements of the resource
- relevant endpoints

**Expected outputs:**
- validated public surface (`Main`/`Search`/`Relay` + resource client)
- models grouped in the correct location
- endpoints implemented in the correct hierarchy
- non-regression tests
- documentation or sample if the capability is publicly exposed

**Key handoff:**
- do not launch `docs` until `code-review` has validated the slice

---

## Playbook C — Refactor an existing area

**When to use:** change of structure, conventions, DI, or hierarchy without a major endpoint addition.

**Sequence:**
1. `architecture`
2. `tests` (update / add protective tests)
3. `code-review`
4. `docs` if the public API or samples change

**Expected outputs:**
- clarified structure
- public surface compatibility preserved or breaking change made explicit
- tests covering sensitive behaviors

---

## Playbook D — Stabilize a roadmap phase

**When to use:** end of a phase in `docs/TODO.md`.

**Sequence:**
1. `/dev` triages the remaining jobs of the phase
2. `tests` to fill in missing coverage
3. `code-review` on modified files or the relevant directory
4. `docs` to align README, samples, XML docs

**Expected outputs:**
- phase technically complete
- minimum coverage present
- documentation aligned with actual code

---

## Playbook E — Prepare a NuGet release

**When to use:** before publishing or freezing a version.

**Sequence:**
1. `tests`
2. `code-review`
3. `docs`

**Mandatory checks:**
- `dotnet build` warning-clean
- `dotnet test` OK
- XML docs generated
- NuGet metadata complete
- README and samples up to date

---

## Playbook F — Build the project foundations

**When to use:** Phase 1 or deep infrastructure restructuring.

**Sequence:**
1. follow `J6` in [dev-jobs.md](./dev-jobs.md)
2. `architecture` to validate the overall hierarchy
3. `tests` for minimum bootstrap coverage if relevant
4. `code-review`
5. `docs` if the bootstrap changes how the SDK is used

**Expected outputs:**
- solution buildable on all targets
- root client and DI usable
- baseline conventions established

---

## Skill handoff rules

- `architecture` delivers a target structure and placement constraints.
- `/dev` in J2 delivers compilable endpoint code, typed models, and correct placement in the client hierarchy.
- `tests` delivers tests that lock down public behavior.
- `code-review` delivers an actionable verdict to decide merge or rework.
- `docs` delivers a user-facing surface consistent with the actually shipped code.

If a skill cannot produce its minimum output, return to the previous skill instead of moving forward.