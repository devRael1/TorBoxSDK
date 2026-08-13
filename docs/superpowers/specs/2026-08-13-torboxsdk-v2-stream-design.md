---
title: TorBoxSDK V2 Stream Resource Design
status: Design approved; awaiting written-spec review
date: 2026-08-13
---

# TorBoxSDK V2 Stream Resource Design

## Purpose and scope

This design defines the first handwritten Main API resource slice for TorBoxSDK
V2: Stream. It adds only the two documented JSON operations owned by
TorBoxClient.Main.Stream:

| Operation | Route | V2 result |
| --- | --- | --- |
| Create a stream | GET /v1/api/stream/createstream | TorBoxResponse<string> |
| Get stream data | GET /v1/api/stream/getstreamdata | TorBoxResponse<StreamData> |

The checked-in Main OpenAPI snapshot declares application/json for both 200
responses but supplies an empty response schema. The typed data models below
are therefore informed by the V1 models and must be recorded as explicitly
unvalidated response-shape decisions. This slice does not make or imply a live
TorBox API validation claim.

## Audience

This is a contributor and reviewer design record. It describes planned V2
behavior, rather than an end-user guide or a claim that the response data has
been validated against a live account.

## Public surface

IStreamClient exposes exactly one asynchronous method per operation:

~~~
Task<TorBoxResponse<string>> CreateStreamAsync(
    CreateStreamRequest request,
    CancellationToken cancellationToken = default);

Task<TorBoxResponse<StreamData>> GetStreamDataAsync(
    GetStreamDataRequest request,
    CancellationToken cancellationToken = default);
~~~

The methods remain reachable only through ITorBoxClient.Main.Stream.
StreamClient stays an internal sealed resource client. The public surface does
not expose HttpClient, request messages, response messages, or a binary stream
wrapper.

## Request models and query mapping

Both operations use public immutable request records in Models/Stream. They
represent query input, not JSON request bodies.

### CreateStreamRequest

| C# property | Query parameter | Required | Behavior when absent |
| --- | --- | --- | --- |
| DownloadId (long) | id | Yes | N/A |
| FileId (long?) | file_id | No | Omit; TorBox applies its default. |
| Type (string?) | type | No | Omit; TorBox applies its default. |
| ChosenSubtitleIndex (int?) | chosen_subtitle_index | No | Omit. |
| ChosenAudioIndex (int?) | chosen_audio_index | No | Omit. |
| ChosenResolutionIndex (int?) | chosen_resolution_index | No | Omit. |
| ScrobblingEnabled (bool?) | scrobbling_enabled | No | Omit; TorBox applies its default. |

### GetStreamDataRequest

| C# property | Query parameter | Required |
| --- | --- | --- |
| PresignedToken (string) | presigned_token | Yes |
| Token (string) | token | Yes |
| ChosenSubtitleIndex (int?) | chosen_subtitle_index | No |
| ChosenAudioIndex (int?) | chosen_audio_index | No |
| ChosenResolutionIndex (int?) | chosen_resolution_index | No |

The request-object design prevents the two access tokens from being swapped by
positional arguments. Query strings use the existing QueryStringBuilder so all
values, including tokens, are percent-encoded. Numeric values are rendered
with invariant culture; an explicitly supplied Boolean is rendered as lowercase
true or false.

The SDK validates only the explicit API-boundary invariants in this design:

- each request instance must be non-null;
- PresignedToken and Token must be non-empty and non-whitespace;
- Type, when supplied, must be non-empty and non-whitespace.

It does not invent positive-value or range restrictions for download IDs, file
IDs, or track indices because the versioned snapshot does not define them.

## Response models

The response models are immutable sealed records in Models/Stream. They retain
the stable fields already represented by V1:

- StreamData: HLS URL, domain, presigned token, selected track indices, file
  token, stream token, transcoding flags, metadata, and search metadata.
- StreamMetadata: available audio and subtitle tracks, thumbnail, chapters,
  and video metadata.
- AudioTrackInfo and SubtitleTrackInfo: known track fields and selected index.

The snapshot provides no schema for metadata.video or search_metadata. Both
are exposed as nullable JsonElement values rather than guessed object models.
This keeps the raw JSON inspectable, preserves forward compatibility, and
does not create a false typed contract.

CreateStreamAsync returns the documented TorBox envelope with a string data
value. GetStreamDataAsync returns the same envelope with StreamData. All
response-model wire names are explicit JSON property mappings.

## Request flow and error handling

For each invocation, StreamClient validates the request, constructs a relative
GET request for stream/createstream or stream/getstreamdata, appends only
supplied query values, and delegates to the existing JSON
ITorBoxApiTransport.SendAsync<T> path.

The transport already owns HTTP response disposal and applies the V2 response
policy:

- a valid JSON envelope is returned as TorBoxResponse<T>, including
  success: false and structured JSON 4xx/5xx responses;
- cancellation and network failures propagate normally;
- malformed JSON, unsupported content, or another invalid response shape
  raises TorBoxProtocolException;
- neither endpoint retries, follows a separate binary-stream path, logs, nor
  persists access tokens.

Although the resource is called Stream, these two operations are JSON
metadata/control endpoints. They do not return TorBoxStreamResponse.

## Contract coverage and known response-shape uncertainty

The two Main coverage records become Implemented with responseMode json and
their exact public interface, method, ordered parameter types, and Task result
type. Their requestType remains null because the OpenAPI contract declares no
request body.

Two reviewed divergence records are added:

| ID | Operation | Recorded uncertainty |
| --- | --- | --- |
| DIV-STREAM-001 | GET /v1/api/stream/createstream | The snapshot declares JSON but no 200 data schema; the string result is informed by the V1 implementation. |
| DIV-STREAM-002 | GET /v1/api/stream/getstreamdata | The snapshot declares JSON but no 200 data schema; StreamData and its nested models are informed by the V1 implementation. |

Each record will cite the checked-in snapshot and V1 source as local evidence,
state the intended V2 behavior, and make clear that an authorized live result
or a future authoritative schema is required to validate the data shape.

## Deterministic tests

Tests use the existing in-memory HTTP handler and never call TorBox. They cover:

1. exact GET method, relative route, and percent-encoded query values;
2. omission of absent optional values, including TorBox-owned defaults;
3. explicit optional values, invariant numeric rendering, and Boolean
   rendering;
4. local validation before any request is sent;
5. successful typed deserialization, including nested tracks and JsonElement
   preservation;
6. structured failure envelopes returned unchanged for both methods;
7. cancellation forwarding; and
8. coverage-manifest linkage for the two public methods and divergence IDs.

The V2 unit suite remains green on net6.0 through net10.0. The core builds on
netstandard2.0 through net10.0. The offline contract suite and deterministic
gate remain the release-quality checks for this slice.

## Documentation and non-goals

Public types and methods receive complete XML documentation. This slice adds no
runnable sample because a meaningful example requires real, sensitive stream
tokens and live data; it would not be a deterministic SDK example.

Out of scope:

- live TorBox validation or use of a real API key;
- binary playback, media download, redirect handling, or stream ownership;
- V1 changes or a V1 compatibility layer;
- retries, resilience policy, logging, or generic endpoint abstractions; and
- guessing schemas for the two raw JSON portions.

## Acceptance criteria

The Stream slice is ready for review only when the public surface, models,
coverage records, divergence records, and deterministic tests match this
document; builds and tests are warning-clean across their target matrices; the
offline V2 gate passes without a secret; and an independent C# review finds no
unresolved Critical or Major finding.
