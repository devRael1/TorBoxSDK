# Integration Testing

## Integration Tests

Use integration tests only where live behavior matters:

- authentication flows
- endpoints that are hard to simulate faithfully
- confirmation of assumptions from the public TorBox contract

## Integration Rules

- Keep them opt-in when credentials are missing.
- Mark them clearly by category or trait.
- Avoid brittle assertions on rapidly changing remote state.
- Prefer test accounts or isolated resources when available.

## Schema Validation Tests

Schema validation tests compare SDK models against the TorBox OpenAPI specification. See [schema-validation-testing-guidance.md](./schema-validation-testing-guidance.md) for full details.

Key points:
- Deterministic schema tests use the versioned baseline declared by
  `contracts/baseline/manifest.json`, not a remote download.
- Static field-coverage and type-mapping tests run offline with
  `--filter "Category=Contract"`, without an API key.
- Remote contract monitoring is a separate manual opt-in command; it writes a
  report outside `contracts/baseline` and never replaces a snapshot.
  It does not establish a CI schedule or gate while DEC-017 remains open.
- Live tests (unmapped field detection) require `TORBOX_API_KEY` and skip gracefully when absent.
- When adding new models, register them in `SchemaModelMapping.SchemaToType`.
