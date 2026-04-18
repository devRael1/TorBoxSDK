---
name: tests
description: 'Use when writing or reviewing tests for TorBoxSDK: unit tests for HttpClient-based services, integration tests against TorBox, schema validation tests against the OpenAPI spec, and serialization tests.'
argument-hint: 'Describe the service, model, or behavior to test, such as TorrentsService request mapping or TorBoxResponse deserialization.'
user-invocable: true
disable-model-invocation: false
---

# Tests

## Purpose

Produce targeted tests for the project with emphasis on transport correctness, response mapping, and regression resistance.

## Use When

Use this skill when you need to:
- write unit tests for a service that uses HttpClient
- validate JSON serialization or deserialization
- add integration tests that hit the real TorBox API
- add or update schema validation tests against the TorBox OpenAPI specification
- review whether a new endpoint implementation is sufficiently tested

## Strategy

Use the lightest test type that gives confidence:
- unit tests for request construction, serialization, and error mapping
- schema validation tests for bidirectional model consistency with the OpenAPI spec
- integration tests for real API behavior and auth-sensitive flows

## Workflow

1. Identify the risk.
Decide whether the main risk is request shape, response mapping, error handling, auth behavior, or nullable handling.

2. Start with unit tests.
Prefer unit tests using a mocked or fake HttpMessageHandler for:
- URL and HTTP method validation
- query and body serialization
- header behavior including auth
- success and failure response mapping
- cancellation token propagation where practical

3. Cover the TorBox response envelope.
For typed responses, verify:
- success, error, detail, and data handling
- empty or null data behavior
- mapping from string API errors to TorBoxErrorCode

4. Add integration tests only for real value.
Use integration tests for:
- auth flows
- endpoint behavior that is hard to simulate accurately
- confirming contract assumptions against the live API

5. Keep integration tests isolated.
Mark them clearly so they can be excluded in default local and CI runs when credentials are unavailable.

6. Update schema validation tests when models change.
When adding or modifying SDK models:
- add or update mappings in `SchemaModelMapping.SchemaToType`
- register any intentional field or type discrepancies in the appropriate known exclusion sets
- ensure static schema tests pass by running with `--filter "Category!=Live"`
- add live schema tests when a new endpoint is mapped, using `SchemaAssert.FindUnmappedFieldsAsync<T>()`

The OpenAPI specification is fetched from `https://api.torbox.app/openapi.json` at test time — no local file is versioned.

## Checks

A test set is sufficient when:
- the public behavior is validated, not just internals
- success and failure paths both exist
- serialization assumptions are pinned down
- schema validation mappings are current for any new or changed models
- tests remain deterministic unless explicitly integration-based
- the tests follow the repo xUnit conventions

## References

- [HTTP client unit testing patterns](./references/http-client-unit-testing-patterns.md)
- [Integration testing guidance](./references/integration-testing-guidance.md)
- [Schema validation testing guidance](./references/schema-validation-testing-guidance.md)
