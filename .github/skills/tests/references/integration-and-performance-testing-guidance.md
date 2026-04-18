# Integration and Performance Testing

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
- The OpenAPI spec is fetched from `https://api.torbox.app/openapi.json` — no local file.
- Static tests (field coverage, type mapping) run without API key.
- Live tests (unmapped field detection) require `TORBOX_API_KEY` and skip gracefully when absent.
- When adding new models, register them in `SchemaModelMapping.SchemaToType`.

## Performance

Use performance tests selectively for:

- large response deserialization
- repeated request construction
- hot serialization paths

Performance tests should answer a concrete question. Do not add benchmarks with no decision behind them.
