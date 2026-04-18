---
uid: changelog
title: Changelog
description: Release notes and version history for TorBoxSDK.
---

# Changelog

All notable changes to TorBoxSDK are documented here. This project follows [Semantic Versioning (SemVer)](https://semver.org).

## Unreleased

### Added
- New features or enhancements will be listed here

### Changed
- Breaking changes or significant updates will be noted

### Fixed
- Bug fixes will be documented

## [1.0.0] - YYYY-MM-DD

### Added
- Initial release of TorBoxSDK
- Support for TorBox Main, Search, and Relay APIs
- Dependency injection integration with `IServiceCollection`
- Standalone client mode for non-DI scenarios
- Full XML documentation and SourceLink support
- Comprehensive error handling with typed `TorBoxException`
- Multi-target support for .NET 6 through .NET 10

### Features
- **Main API Clients:**
  - General operations
  - Torrents management
  - Usenet integration
  - Web Downloads
  - User profile management
  - Notifications
  - RSS feeds
  - Stream information
  - Integrations (OAuth, Discord roles)
  - Vendors information
  - Queued operations

- **Search API Client:**
  - Torrent search capabilities

- **Relay API Client:**
  - Relay functionality

## Versioning Convention

TorBoxSDK uses Semantic Versioning:

- **MAJOR** version for incompatible API changes
- **MINOR** version for new functionality in a backward-compatible manner
- **PATCH** version for backward-compatible bug fixes

## Contributing Changes

When contributing to TorBoxSDK:

1. Update the CHANGELOG with your changes under the "Unreleased" section
2. Follow the format: Added, Changed, Fixed, Deprecated, Removed, Security
3. Use present tense ("Add feature" not "Added feature")
4. Reference issues where applicable (e.g., "Fixes #123")

## Release Process

When releasing a new version:

1. Move "Unreleased" changes to a new version section with the date
2. Update version numbers in relevant files
3. Create a git tag with the version
4. Generate release notes from the changelog
