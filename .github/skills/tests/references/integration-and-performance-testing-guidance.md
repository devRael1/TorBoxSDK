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

## Performance

Use performance tests selectively for:

- large response deserialization
- repeated request construction
- hot serialization paths

Performance tests should answer a concrete question. Do not add benchmarks with no decision behind them.
