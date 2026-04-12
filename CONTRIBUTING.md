# Contributing to TorBoxSDK

Thanks for helping improve TorBoxSDK! This repository is an MIT-licensed, community-driven, open-source SDK. Everyone is welcome to contribute — whether it's API coverage, documentation, tests, examples, packaging, or bug fixes.

Please note that this project is released with a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you agree to abide by its terms.

## Before you start

- Check existing [issues](https://github.com/devRael1/TorBoxSDK/issues) before opening a new one
- Use the provided [issue templates](https://github.com/devRael1/TorBoxSDK/issues/new/choose) for bug reports and feature requests
- Open an issue or discussion first for larger changes, new API areas, or behavior changes
- Keep proposals aligned with the current public SDK structure: `TorBoxClient`, `Main`, `Search`, and `Relay`
- For security vulnerabilities, please follow our [Security Policy](SECURITY.md) — do **not** open a public issue

## Development workflow

1. Create a focused branch for your change
2. Keep the change scoped to one concern when possible
3. Update docs, XML documentation, or examples when public behavior changes
4. Run the relevant validation commands locally

```bash
dotnet build
dotnet test tests/TorboxSDK.UnitTests/
dotnet test tests/TorBoxSDK.IntegrationTests/
```

Integration tests use `TORBOX_API_KEY` and should skip gracefully when it is not set.

## Pull request expectations

Please aim for pull requests that:

- explain the user-facing problem being solved
- keep naming and public API behavior consistent with the existing SDK
- include tests for behavior changes when applicable
- update documentation when new features, options, or examples are added
- avoid unrelated cleanup in the same PR

## Documentation contributions

Documentation improvements are first-class contributions.

Helpful areas include:

- README clarity
- guides in `docs/`
- API XML documentation
- examples in `src/TorBoxSDK.Examples/`
- package and release-readiness polish

## Reporting bugs

When reporting a bug, include:

- the TorBoxSDK version
- target framework
- the client or endpoint involved
- a minimal repro snippet if possible
- whether the failure is SDK-side, API-side, or configuration-related

## Security and secrets

- Never commit real API keys or credentials
- Use environment variables such as `TORBOX_API_KEY` for local testing
- Sanitize logs, screenshots, and repro snippets before sharing them publicly

## License

By contributing to this repository, you agree that your contributions will be licensed under the [MIT License](LICENSE).
