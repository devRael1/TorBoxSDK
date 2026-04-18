# Endpoint Implementation Checklist

Use this checklist when executing **J2 — Endpoint** inside `/dev`.

## Contract

- Confirm HTTP method and route.
- Confirm auth requirement.
- Confirm whether input is query, path, JSON, form-urlencoded, or multipart.
- Confirm the response envelope and the inner `data` contract.
- Note any binary, RSS, redirect, or raw-content behavior.

## Surface

- Choose the owning API client.
- Choose the owning resource client.
- Name the public method.
- Decide whether the endpoint uses scalars, a request type, or an options type.
- Choose a typed response model.

## Implementation

- Build the request with the correct content type.
- Pass `CancellationToken` through every async call.
- Reuse shared auth and serialization helpers.
- Map API failure responses to `TorBoxException` and `TorBoxErrorCode`.
- Keep endpoint logic in the owning client, not in the root client.

## Verification

- Add unit tests for success.
- Add unit tests for failure.
- Assert the route, method, and payload.
- Register new models in `SchemaModelMapping.SchemaToType` for schema validation.
- Run static schema tests to verify field and type coverage.
- Update docs if the public API changed.
