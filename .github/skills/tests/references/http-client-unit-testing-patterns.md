# HTTP Client Unit Testing

## Purpose

Unit tests should validate observable client behavior without talking to the live TorBox API.

## Focus

- HTTP method
- route and query string
- headers including auth
- request body serialization
- response mapping
- error mapping

## Approach

Use a fake or mocked `HttpMessageHandler` and assert:

- the outgoing `HttpRequestMessage`
- the deserialized public result
- thrown exceptions for failure cases

## Coverage

For each resource client method, try to cover:

- one successful response
- one API failure response
- one nullability or optional-input case when relevant

## Avoid

- tests that only verify internal helper calls
- tests that depend on live API state for ordinary transport assertions
- assertions that ignore route or payload correctness
