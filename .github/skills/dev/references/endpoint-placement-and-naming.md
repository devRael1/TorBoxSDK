# Endpoint Placement and Naming

Use these rules when executing **J2 — Endpoint** inside `/dev`.

## Ownership

Use these ownership rules consistently:

- Main API endpoint: `TorBoxClient.Main.<ResourceClient>`
- Search API endpoint: `TorBoxClient.Search.<ResourceClientOrArea>`
- Relay API endpoint: `TorBoxClient.Relay.<ResourceClientOrArea>`

Do not place normal endpoint methods directly on `TorBoxClient`.

## Naming

- Client names end with `Client`.
- Public async methods end with `Async`.
- Request types end with `Request` when they represent input bodies.
- Option types end with `Options` when they represent query or behavior configuration.
- Response types should be domain-driven first, not transport-driven, unless the transport shape is the real contract.

## Placement

Prefer colocating types by API family and resource:

- `Main/Torrents/TorrentsClient.cs`
- `Main/Torrents/Requests/CreateTorrentRequest.cs`
- `Main/Torrents/Responses/TorrentResponse.cs`
- `Search/Meta/SearchMetaClient.cs`
- `Relay/RelayApiClient.cs`

Shared primitives belong in shared folders only when they are truly reused across multiple resource areas.

## Method Shape

Prefer these method design rules:

- Small number of scalars for very simple endpoints
- Request model for body-heavy or expanding endpoints
- `CancellationToken` as the last parameter
- One public method per API operation unless there is a clear overload case
