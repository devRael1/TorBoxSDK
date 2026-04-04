# Packaging and Release

Use this reference for **D5 — Packaging & Release**.

## Package Quality

- Multi-target support is configured for .NET 6 through .NET 10.
- Nullable and warnings configuration are intentional.
- XML documentation generation is enabled.
- Repository metadata is present.
- License metadata is present.
- SourceLink and symbols are configured.

## Open Source Readiness

- Public APIs have understandable names.
- README installation and quick start are accurate.
- Samples compile and reflect the current SDK surface.
- The package does not expose accidental internal abstractions.

## Release Checks

- Build is clean.
- Unit tests pass.
- Integration tests are either passing or clearly separated.
- NuGet metadata matches the repository and branding.
