---
title: TorBoxSDK V2 Design
status: Approved for planning
date: 2026-08-12
---

# TorBoxSDK V2 Design

## Purpose

TorBoxSDK V2 is a clean major-version redesign of the open-source C# SDK for the TorBox API. Its primary goal is an ergonomic, predictable .NET API while retaining complete coverage of the TorBox Main, Search, and Relay API families.

V2 is not an incremental repair of the earlier V2 work. It keeps only the useful delivery safeguards: a versioned API contract, deterministic verification, explicit live-validation evidence, and safe streaming. It does not reuse generated clients, compatibility layers, or architecture chosen for tooling rather than for SDK users.

## Goals

- Publish `TorBoxSDK` 2.0.0 as a stable, breaking major release under the existing package identity.
- Provide complete coverage of all documented Main, Search, and Relay endpoints.
- Make the public API idiomatic and easy to discover through C# IntelliSense.
- Support consumers as broadly as possible through `netstandard2.0`, while exercising modern runtimes through .NET 6 to .NET 10.
- Keep all endpoint and model code manually authored.
- Make API coverage, transport behavior, and release evidence reproducible and reviewable.
- Support dependency-injection-first applications without imposing DI dependencies on every consumer.

## Non-goals

- No source-compatible V1 facade, obsolete adapter layer, or dual V1/V2 public API.
- No generated client or generated model source in the shipped SDK.
- No generic runtime endpoint engine or descriptor-driven public surface.
- No automatic retries, backoff, rate-limit policy, or hidden resilience behavior.
- No third-party production dependency in the core package.
- No requirement to provide a secret to run deterministic checks.

## Package and compatibility design

### Packages

| Package | Responsibility |
| --- | --- |
| `TorBoxSDK` | Portable core: public client hierarchy, options, models, response types, transport, authentication, serialization, and streaming. |
| `TorBoxSDK.DependencyInjection` | Optional `IServiceCollection` integration, named HTTP-client registration, options binding, and `AddTorBox(...)`. |

`TorBoxSDK` remains the only required package. The DI package is installed only by applications that use `Microsoft.Extensions.DependencyInjection`.

### Target frameworks and dependencies

Both packages target `netstandard2.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0`.

The core has no third-party production dependencies. Its sole compatibility exception is the Microsoft `System.Text.Json` package for the `netstandard2.0` target. Modern target frameworks use the serializer included by their runtime. The core must not rely on APIs unavailable to `netstandard2.0` unless an equivalent, tested implementation is supplied for that target.

The DI package may depend only on the Microsoft.Extensions packages required for DI and HTTP-client integration.

## Public API design

### Root hierarchy

`TorBoxClient` implements `ITorBoxClient`. Both direct usage and DI expose the same root interface:

```text
ITorBoxClient
|- Main
|  |- General
|  |- Torrents
|  |- Usenet
|  |- WebDownloads
|  |- User
|  |- Notifications
|  |- Rss
|  |- Stream
|  |- Integrations
|  |- Vendors
|  `- Queued
|- Search
`- Relay
```

The API families remain visible because they reflect TorBox's real topology. `Main` groups the Main API resource clients. `Search` and `Relay` are direct family clients. Every public resource has a focused interface; concrete resource implementations and transport details remain internal.

`ITorBoxClient` is the only SDK client service registered by `AddTorBox(...)`. Consumers navigate from it to all families and resources. Direct usage remains available through `TorBoxClient` constructors accepting an API key or `TorBoxClientOptions`.

### Public conventions

- Public asynchronous methods use the `Async` suffix and accept `CancellationToken` as their final parameter.
- Endpoint methods use domain-oriented names and request types, not generated operation names.
- Request and response models are public, manually authored, immutable, and grouped by API family and resource.
- The public surface never exposes `HttpClient`, `HttpRequestMessage`, or `HttpResponseMessage`.

## Responses, errors, and streams

### JSON endpoints

Every JSON endpoint returns `Task<TorBoxResponse<T>>`; endpoints without a data payload return `Task<TorBoxResponse>`. The response keeps the TorBox envelope visible to callers:

- `Success`
- `Data`, when the endpoint has a data payload
- `Error`
- `Detail`
- the associated HTTP status code

A valid TorBox error envelope returns `Success = false`, including a structured 4xx or 5xx response. It does not become an exception merely because the API reports a known business or authentication error.

`OperationCanceledException` represents cancellation, `HttpRequestException` represents a network failure, and `TorBoxProtocolException` represents an HTTP response that cannot be interpreted according to the endpoint contract. Error details read from invalid or failed responses are bounded before they are retained, so a hostile or unexpectedly large response body cannot cause unbounded memory use.

### Streaming endpoints

Endpoints that deliver a file, a binary payload, or a redirect-oriented result return `TorBoxStreamResponse`. It owns the underlying response and implements `IDisposable`. It exposes the content stream, media type, content length when supplied, file name when supplied, HTTP status, and applicable response metadata.

The SDK never buffers a stream response into memory implicitly. The caller disposes `TorBoxStreamResponse` after consuming the stream.

## Configuration, authentication, and transport

`TorBoxClientOptions` contains the API key, the Main/Search/Relay base URLs, and the global timeout. Construction and DI configuration validate all values before a request can be sent.

The SDK keeps one internal `HttpClient` pipeline per API family because their base URLs can differ. An internal authentication handler adds the Bearer token. The key is never an endpoint-method parameter and is never written to logs.

A narrow internal transport component centralizes request sending, JSON serialization, bounded error reading, and stream ownership. It does not determine domain endpoint shapes and does not create a generic public endpoint abstraction.

The SDK performs no retry, backoff, or rate-limit recovery automatically. Applications own their resilience policy, including any policy configured around the DI-provided HTTP pipelines.

## Models and contract coverage

Models are handwritten and organized as follows:

- shared transport and envelope types in `Models/Common`;
- Main API models beneath their resource domain;
- Search API models beneath `Models/Search`;
- Relay API models beneath `Models/Relay`.

Every JSON member has an explicit wire-name mapping. Nullability, collection defaults, and numeric/date types describe observed TorBox data rather than optimistic assumptions.

### Versioned contract baseline

A reviewed TorBox contract snapshot is stored in the repository with a manifest containing its source, retrieval date, version identity when available, and SHA-256 hash. Builds and tests use the checked-in snapshot only; they never fetch an OpenAPI document from the network.

A machine-readable coverage manifest maps every contract operation to:

- its HTTP method and path;
- its API family and resource;
- its public V2 method;
- its request type and response or stream type;
- its expected TorBox envelope behavior; and
- any documented divergence.

Contract tests compare the snapshot and coverage manifest, then verify the linked public types and explicit JSON mappings. No operation, parameter, response field, or known divergence can be omitted silently.

When TorBox behavior differs from the versioned contract, the difference is recorded in a reviewed divergence register with evidence and the intended V2 behavior. A divergence is never silently normalized by generated code or undocumented transport logic.

### Multi-source contract resolution

The V2 contract inventory has three explicitly named sources. Main continues to use the official `https://api.torbox.app/openapi.json` snapshot. Relay uses its independently published official `https://relay.torbox.app/openapi.json` snapshot. Search uses a reviewed, normalized operation inventory sourced from TorBox's public Postman documentation at `https://www.postman.com/torbox/torbox-api/documentation/u47iwao/search-api` when a public collection export is unavailable.

Each source is versioned, manifest-backed, and hash-validated locally. Coverage records identify both their source and their `METHOD path`, so similarly named routes from different API families cannot collide. The Search manifest must explicitly retain its documentation-only status and require controlled live validation before a stable release can claim Search runtime fidelity. Its unavailable or unresolvable live host is not silently converted into a removed API family or an invented endpoint.

## Verification strategy

### Deterministic checks

The normal validation path requires no API key and blocks delivery on failure. It includes:

- locked restore and Release builds for every target framework;
- unit tests for every resource and endpoint using a simulated HTTP handler;
- request-path, query, header, body, envelope, cancellation, protocol-error, redirect, and stream-lifetime tests;
- static contract and coverage-manifest tests;
- package creation and inspection of the exact package intended for publication; and
- documentation, example, link, and formatting checks.

The test projects execute on .NET 6 through .NET 10. The `netstandard2.0` target is compiled and packed as part of the same deterministic gate.

### Controlled live validation

Live tests are isolated from deterministic tests and run only with a dedicated TorBox test account and an environment-provided secret. The secret is never committed or printed. Mutating tests use uniquely identifiable resources and clean them up.

Live results are release evidence, not a substitute for deterministic checks. If no authorized live evidence exists, release documentation states that fact and does not claim live validation. A known live discrepancy must be either fixed, covered by the divergence register, or block publication.

## Documentation, examples, and migration

The existing documentation structure is updated rather than replaced with a second site or navigation system. V2 documentation includes:

- direct and DI quick starts;
- configuration and authentication guidance;
- guides for Main, Search, and Relay;
- response, error, cancellation, and stream-lifetime guidance;
- public API reference sourced from complete XML documentation; and
- runnable examples for configuration, torrent management, downloads/streams, search, Relay, and failure handling.

The V1-to-V2 migration guide includes a concrete old-to-new API table, the reason for every material break, and a migration checklist. It explicitly covers installation, optional DI package installation, client navigation, `TorBoxResponse` handling, error handling, and stream disposal.

Existing V1 package versions remain historical releases. V2 does not contain a V1 compatibility layer and does not imply a continuing V1 maintenance commitment.

## Release criteria for `TorBoxSDK` 2.0.0

`TorBoxSDK` 2.0.0 is published once, as a stable release, under the existing package identity. There are no public alpha, beta, release-candidate, or parallel V2 package releases.

Publication requires all of the following:

1. Every documented Main, Search, and Relay operation is covered by the versioned contract baseline and coverage manifest; every documentation-only source has the required controlled live-validation evidence before stable publication.
2. Every mapped endpoint has its handwritten implementation, models, deterministic unit tests, and documented divergence behavior where applicable.
3. All deterministic checks and exact-package validation pass.
4. Documentation, examples, XML API reference, and the V1-to-V2 migration guide are complete and verified.
5. The controlled live-validation report is attached when authorized test credentials are available; no unproven live behavior is presented as validated.
6. A final independent review finds no unresolved correctness, public-API, security, documentation, or packaging blocker.

## Implementation boundaries

The implementation plan will preserve this design and split the work into independently reviewable slices:

1. Package foundation, target-framework support, public primitives, options, DI package, and deterministic test harness.
2. Shared transport, authentication, response parsing, bounded failures, and stream response support.
3. Handwritten Main API resources and their contract mappings/tests.
4. Handwritten Search and Relay resources and their contract mappings/tests.
5. Contract completeness, controlled live validation, public documentation, runnable examples, migration guide, package validation, and release review.

Each slice can be implemented and reviewed independently, but none changes the approved public hierarchy or response/error policy without a new design decision.
