# TorBoxSDK agent instructions

These instructions apply to the entire repository.

## Program control

- The private GitHub Project
  [TorBoxSDK v2.0.0](https://github.com/users/devRael1/projects/14) tracks
  execution status, ownership, branches, worktrees, dependencies and evidence.
- `docs/maintainers/index.md` is the canonical v2 roadmap.
- `docs/maintainers/decisions.md` is the decision authority. Agents must not
  resolve an open decision implicitly. Move the card to `Needs decision` and
  ask the project owner.
- `docs/maintainers/v2-program-control.md` defines status transitions and
  handoff evidence.

## Skills and roles

- Start multi-step work with `.agents/skills/dev/SKILL.md`.
- Use `.agents/skills/tests/SKILL.md` for test implementation and test review.
- Use `.agents/skills/code-review/SKILL.md` for independent, read-only C#
  review before integration.
- Use `.agents/skills/docs/SKILL.md` for Markdown, DocFX, XML documentation and
  package-facing documentation.
- Agent definitions live in `.codex/agents/`. The implementing agent and the
  reviewing agent must be different for production-code changes.

## Git and worktrees

- Work only in the branch and worktree assigned by the Kanban card.
- Use one focused concern per `codex/v2-*` branch.
- Record the base commit before editing and keep unrelated user changes out of
  the branch.
- Integration normally uses rebase and merge. Rebase on the latest validated
  `v2.0.0`, rerun the applicable checks, then use a fast-forward integration
  locally or **Rebase and merge** on GitHub.
- A merge commit is the documented fallback when rebase is not possible. Do
  not squash as the normal integration method.
- Do not create tags, publish packages or delete worktrees without explicit
  authorization.

## V2 contract and compatibility

- The effective deployed API behavior is the final contract authority.
- OpenAPI is the primary published source. Official TorBox Postman/documentation
  supplements it. When they disagree, use a targeted live test and record the
  divergence; never invent or merge response shapes.
- Generated contracts remain internal. The public facade and public models are
  manually curated.
- Search API remains in the SDK but must be documented as restricted to
  approved projects and whitelisted IPs. Do not claim public availability.
- The target matrix is
  `netstandard2.0;net6.0;net7.0;net8.0;net9.0;net10.0`. Compatibility claims
  require the consumer and package-validation evidence defined in
  `docs/maintainers/dotnet-compatibility.md`.

## Completion evidence

- Run `git diff --check` and inspect the staged diff before each commit.
- Build warning-clean on every applicable target.
- Run deterministic unit, serialization and contract tests appropriate to the
  change. Live tests are separate and require the permissions documented in
  `docs/maintainers/api-divergences.md`.
- Validate DocFX and links for documentation changes.
- Record base commit, final commit, commands, results, skipped checks, risks and
  PR URL in the Kanban card before handoff.
