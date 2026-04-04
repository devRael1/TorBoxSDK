---
name: tests
description: 'Use when writing or reviewing tests for TorBoxSDK: unit tests for HttpClient-based services, integration tests against TorBox, serialization tests, and performance validation.'
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
- review whether a new endpoint implementation is sufficiently tested
- add performance or throughput-oriented validation

## Strategy

Use the lightest test type that gives confidence:
- unit tests for request construction, serialization, and error mapping
- integration tests for real API behavior and auth-sensitive flows
- performance tests only for code paths where allocation or throughput matters

## Workflow

1. Identify the risk.
Decide whether the main risk is request shape, response mapping, error handling, auth behavior, nullable handling, or performance.

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

6. Add performance tests selectively.
Use them for:
- heavy serialization paths
- bulk list deserialization
- repeated request construction where allocation matters

## Checks

A test set is sufficient when:
- the public behavior is validated, not just internals
- success and failure paths both exist
- serialization assumptions are pinned down
- tests remain deterministic unless explicitly integration-based
- the tests follow the repo xUnit conventions

## References

- [HTTP client unit testing patterns](./references/http-client-unit-testing-patterns.md)
- [Integration and performance testing guidance](./references/integration-and-performance-testing-guidance.md)
