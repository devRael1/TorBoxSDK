---
applyTo: "**/*.cs"
description: "Use when writing, reviewing, or refactoring C# code. Enforces naming conventions, nullability, file-scoped namespaces, expression-bodied members, primary constructors, pattern matching, async conventions, and file organization. Covers TorBoxSDK architecture compliance, HTTP client patterns, DI, response handling, performance, security, XML docs (src/**), model and JSON serialization rules (src/**/Models/**), xUnit test patterns and HttpClient mocking (tests/**), and sample readability rules (samples/**)."
---

# TorBoxSDK — C# Coding Standards

This is the single source of truth for all C# coding rules in this project. Rules are organized by scope:

- **Part 1** — Base rules: apply to every `.cs` file
- **Part 2** — SDK source rules: apply to `src/**/*.cs`
- **Part 3** — Model rules: apply to `src/**/Models/**/*.cs`
- **Part 4** — Test rules: apply to `tests/**/*.cs`
- **Part 5** — Sample rules: apply to `samples/**/*.cs`

---

# Part 1 — Base C# Conventions

## Naming

| Symbol | Convention | Example |
|--------|-----------|---------|
| Types, methods, properties, events | `PascalCase` | `TorrentService`, `GetAsync` |
| Local variables, parameters | `camelCase` | `torrentId`, `cancellationToken` |
| Private fields | `_camelCase` | `_httpClient`, `_options` |
| Constants | `PascalCase` | `DefaultTimeout` |
| Interfaces | `I` prefix + PascalCase | `ITorrentsClient` |
| Async methods | `Async` suffix | `CreateTorrentAsync` |

Reject if:
- Any private field lacks the `_` prefix
- Any async method (returns `Task` or `Task<T>`) is missing the `Async` suffix — including interface declarations
- A type name does not match its filename
- Single-letter variables outside of short LINQ lambdas

## Nullability

Nullable reference types are **enabled** project-wide (`<Nullable>enable</Nullable>`).

- Parameters that may be null: declare `T?`
- Parameters that must not be null: declare `T`
- Return types must reflect true nullability
- No `!` null-forgiving operator without an inline comment proving non-nullability
- No `= null!` or `default!` initialization without explicit justification

```csharp
// REJECT — suppresses warning without explanation
return _cache[key]!;

// ACCEPT — justified suppression
// Guaranteed non-null because AddTorBox() ensures this is registered.
TorBoxClient client = serviceProvider.GetService<TorBoxClient>()!;
```

## Type Inference (`var`)

Prefer explicit types. `var` is acceptable only when the right-hand side **unambiguously reveals the type** to the reader:

```csharp
// ACCEPT — type is obvious
var stream = new MemoryStream();
var dict = new Dictionary<string, int>();

// REJECT — type not obvious
var result = await client.GetAsync(url);
var data = response.Content;
```

## Modern C# Features

### File-scoped namespaces (mandatory)
```csharp
// ACCEPT
namespace TorBoxSDK.Clients;

// REJECT
namespace TorBoxSDK.Clients { ... }
```

### Primary constructors (preferred for DI)
```csharp
// ACCEPT
public sealed class TorrentsClient(HttpClient httpClient, ILogger<TorrentsClient> logger) { }

// REJECT unless justified
public sealed class TorrentsClient
{
    private readonly HttpClient _httpClient;
    public TorrentsClient(HttpClient httpClient) { _httpClient = httpClient; }
}
```

### Expression-bodied members (required for single-expression)
```csharp
// ACCEPT
public string BaseUrl => _options.BaseUrl;
public override string ToString() => $"TorBoxClient({_options.BaseUrl})";

// REJECT
public string BaseUrl { get { return _options.BaseUrl; } }
```

### Pattern matching (preferred over explicit checks)
```csharp
// ACCEPT
if (response is { IsSuccessStatusCode: true })
return errorCode switch
{
    TorBoxErrorCode.BadToken => new UnauthorizedException(detail),
    _ => new TorBoxException(detail)
};

// REJECT
if (response.IsSuccessStatusCode == true) { ... }
```

### Collection expressions (C# 12+)
```csharp
// ACCEPT
IReadOnlyList<Torrent> empty = [];

// REJECT
IReadOnlyList<Torrent> empty = new List<Torrent>();
```

## Async/Await

- Always suffix async methods with `Async`
- Always accept `CancellationToken ct = default` as the last parameter
- Always propagate `ct` to every inner async call — never drop it silently
- Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` — always `await`
- Use `ConfigureAwait(false)` on every `await` in library code (`src/`)
- Elide `async`/`await` when the method is a single `return` with no try/catch

```csharp
// REJECT — deadlock risk
string json = _httpClient.GetStringAsync(url).Result;

// REJECT — missing ConfigureAwait and ct
HttpResponseMessage response = await _httpClient.GetAsync(url);

// ACCEPT
HttpResponseMessage response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
```

## File Organization

- One public type per file — no exceptions
- Filename must exactly match the type name (including casing)
- Nested private types are allowed within their containing type's file
- `using` directives at the top of the file, never inside a namespace block
- `using` order: `System.*` → `Microsoft.*` → third-party → project namespaces
- No `#region` blocks

---

# Part 2 — SDK Architecture (`src/**/*.cs`)

## Client Hierarchy

```
TorBoxClient
├── Main  (MainApiClient)
│   ├── Torrents, Usenet, WebDownloads, User
│   ├── Notifications, Rss, Stream
│   ├── Integrations, Vendors, Queued
├── Search (SearchApiClient)
└── Relay  (RelayApiClient)
```

Reject if:
- `TorBoxClient` directly exposes endpoint methods instead of delegating to resource clients
- An endpoint is implemented in the wrong resource client
- Any API client class is not `sealed` without a justified inheritance chain

## HTTP Client

```csharp
// REJECT — hardcoded URL
await _httpClient.GetAsync("https://api.torbox.app/v1/api/torrents/mylist");

// ACCEPT — relative path, BaseAddress set at registration
await _httpClient.GetAsync("/torrents/mylist", ct).ConfigureAwait(false);
```

## Authentication

The Bearer token must be injected via `DelegatingHandler` — never set manually per request:

```csharp
// REJECT
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

// ACCEPT
public sealed class AuthHandler(TorBoxClientOptions options) : DelegatingHandler { ... }
```

## Dependency Injection

- `TorBoxClient` and all sub-clients registered via `AddTorBox()`
- No `new HttpClient()` in constructors — use `IHttpClientFactory` or injected `HttpClient`
- `TorBoxClientOptions` via the options pattern, not a static class
- All public interfaces registered, not only implementations

## Response Handling

TorBox API standard shape: `{ success, error, detail, data }`.

- All endpoints deserialize via `TorBoxResponse<T>` or `TorBoxResponse`
- `success == false` must throw `TorBoxException` with `ErrorCode` and `Detail`
- HTTP 4xx/5xx translate to typed exceptions
- `JsonSerializerOptions` must be reused (static or DI) — never `new` per call

```csharp
// REJECT
JsonSerializer.Deserialize<TorBoxResponse<Torrent>>(json, new JsonSerializerOptions { ... });

// ACCEPT
JsonSerializer.Deserialize<TorBoxResponse<Torrent>>(json, TorBoxJsonOptions.Default);
```

## Performance

| REJECT pattern | Risk |
|---------------|------|
| `_httpClient.GetStringAsync(url).Result` | Deadlock |
| Missing `using` on `HttpResponseMessage` | Connection pool exhaustion |
| `new JsonSerializerOptions()` per call | Expensive allocation |
| `ToList()` before `foreach` (no intent) | Unnecessary allocation |
| `ToString()` on value types in log strings | Boxing |
| `.ReadAsStringAsync()` on stream endpoints | Excessive buffering |

## Security

Flag as **CRITICAL**:
- Hardcoded API key, token, or password in any string literal
- String interpolation of user input in log messages (log injection)
- `HttpClient.BaseAddress` set from unvalidated user input (SSRF)
- Exception detail with stack trace exposed to callers

```csharp
// REJECT — log injection
_logger.LogInformation($"Processing: {userInput}");

// ACCEPT — structured logging
_logger.LogInformation("Processing torrent: {TorrentId}", torrentId);

// ACCEPT — validated base URL
if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
    throw new ArgumentException("BaseUrl must be an absolute URI.", nameof(options));
```

## XML Documentation

All `public` members must have complete XML doc:

```csharp
/// <summary>Retrieves the authenticated user's torrent list.</summary>
/// <param name="id">Optional torrent ID to retrieve a single torrent.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>A list of torrents, or a single torrent if <paramref name="id"/> is provided.</returns>
/// <exception cref="TorBoxException">Thrown when the API returns an error.</exception>
Task<IReadOnlyList<Torrent>> GetMyListAsync(long? id = null, CancellationToken ct = default);
```

Reject if: `public` method, property, or type has no `<summary>`, `<param>` is absent for non-obvious parameters, or `<returns>` is missing on `Task<T>` methods.

---

# Part 3 — Models (`src/**/Models/**/*.cs`)

## Standard Response Shape

Every TorBox API response follows `{ success, error, detail, data }`:

```csharp
public sealed record TorBoxResponse<T>
{
    [JsonPropertyName("success")] public bool Success { get; init; }
    [JsonPropertyName("error")]   public string? Error { get; init; }
    [JsonPropertyName("detail")]  public string? Detail { get; init; }
    [JsonPropertyName("data")]    public T? Data { get; init; }
}
```

Reject if `error`/`detail` are non-nullable, or `data` is non-nullable in the generic version.

## Record vs Class

| Use case | Type |
|----------|------|
| Response/data models (read-only, deserialized) | `sealed record` with `init` |
| Request models (write-once, serialized) | `sealed record` or `sealed class` with `init` |
| Options/configuration (mutable) | `sealed class` with settable properties |
| API-defined enums | `enum` |

Reject: mutable response model (`set` properties), non-`sealed` model, positional record with more than 3 properties.

## JSON Serialization

All TorBox API fields use `snake_case`. Every JSON property must have an explicit `[JsonPropertyName]`:

```csharp
// ACCEPT
[JsonPropertyName("torrent_id")]
public long TorrentId { get; init; }

// REJECT — implicit name, will silently fail
public long TorrentId { get; init; }
```

Additional rules:
- Enums in JSON: `[JsonConverter(typeof(JsonStringEnumConverter))]` or explicit per-member `[JsonPropertyName]`
- Dates: `DateTimeOffset` — never `DateTime`
- `[JsonIgnore]` on any request property that must not be serialized

## Nullability on Data Contracts

Nullability must reflect what the API **actually returns**:

```csharp
// REJECT — collection should not be nullable
public List<TorrentFile>? Files { get; init; }

// ACCEPT
public IReadOnlyList<TorrentFile> Files { get; init; } = [];

// REJECT — required field marked nullable
public string? Name { get; init; }

// ACCEPT
public string Name { get; init; } = string.Empty;
```

## Immutability

- All properties: `init` (or `get`-only for records)
- No `set` on response models
- Collections: `IReadOnlyList<T>` or `IReadOnlyDictionary<K,V>` — never `List<T>`
- Collection properties default to `[]`, never `null`

## Enum Design

```csharp
public enum TorBoxErrorCode
{
    Unknown = 0,  // catch-all for forward compatibility
    BadToken,
    DatabaseError,
    NoAuthProvided,
    // ... all documented codes
}
```

- Dedicated `enum` type for each API-defined enumeration
- Always an `Unknown = 0` catch-all member
- Paired with `[JsonConverter]` at the deserialization boundary

## Request Model Validation

Request models are dumb data containers. Validation happens in the client method before the HTTP call:

```csharp
// REJECT — validation in constructor
public CreateTorrentRequest(string magnet) { if (string.IsNullOrEmpty(magnet)) throw ...; }

// ACCEPT — validation at boundary
public async Task<TorBoxResponse<Torrent>> CreateTorrentAsync(CreateTorrentRequest request, CancellationToken ct = default)
{
    ArgumentNullException.ThrowIfNull(request);
    ArgumentException.ThrowIfNullOrEmpty(request.Magnet);
    // ...
}
```

## Model Organization

- Single-client models: `Models/{ResourceName}/` (e.g., `Models/Torrents/`)
- Shared models: `Models/Common/` (e.g., `TorBoxResponse<T>`, `TorBoxErrorCode`)
- No model named `Request`, `Response`, or `Data` without a resource-specific prefix

---

# Part 4 — Tests (`tests/**/*.cs`)

## Test Naming

```
MethodName_StateUnderTest_ExpectedBehavior
```

```csharp
// ACCEPT
public async Task GetMyListAsync_WithValidApiKey_ReturnsTorrentList()
public async Task CreateTorrentAsync_WithNullRequest_ThrowsArgumentNullException()

// REJECT
public async Task GetMyListAsync_Test()
public async Task ShouldReturnUser()
```

Test class: `{ClassUnderTest}Tests`, must be `sealed`.

## Arrange-Act-Assert

Every test has exactly three phases with blank lines and `// Arrange` / `// Act` / `// Assert` comments:

```csharp
[Fact]
public async Task GetMyListAsync_WithValidApiKey_ReturnsTorrentList()
{
    // Arrange
    TorrentsClient client = CreateClientWithHandler(jsonResponse);

    // Act
    IReadOnlyList<Torrent> result = await client.GetMyListAsync();

    // Assert
    Assert.NotEmpty(result);
    Assert.Equal("MyTorrent", result[0].Name);
}
```

Reject if: no AAA comments, logic in Act/Assert phase, multiple distinct behaviors in one test.

## Test Attributes

- `[Fact]` for single parameterless scenarios
- `[Theory]` + `[InlineData]` for parameter variations — never duplicate near-identical `[Fact]` tests
- `[Theory]` + `[MemberData]` for complex multi-property data
- Never `[Fact(Skip = "...")]` without a tracking issue reference

## Assertions

- xUnit built-in only (`Assert.*`) — never `if` with `throw`
- `Assert.Equal(expected, actual)` over `Assert.True(actual == expected)`
- `Assert.ThrowsAsync<T>` for exceptions — never `try/catch`

```csharp
// REJECT
try { await client.CreateTorrentAsync(null); }
catch (ArgumentNullException) { Assert.True(true); }

// ACCEPT
await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateTorrentAsync(null));
```

## Async Tests

- Return type must be `Task` — never `void`
- Never `.Result` or `.Wait()` inside tests

## HttpClient Mocking

Unit tests must **never** make real HTTP calls. Mock via a custom `DelegatingHandler`:

```csharp
private static TorrentsClient CreateClient(string json, HttpStatusCode status = HttpStatusCode.OK)
{
    MockHttpMessageHandler handler = new(json, status);
    HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.torbox.app") };
    return new TorrentsClient(httpClient);
}
```

JSON payloads must use the full TorBox response shape and raw string literals:

```csharp
// REJECT — incomplete payload
string json = """{"success":true,"data":{"id":1}}""";

// ACCEPT — realistic full shape
string json = """
    {
      "success": true,
      "error": null,
      "detail": "Found.",
      "data": { "id": 1, "name": "ubuntu.torrent", "status": "active" }
    }
    """;
```

Store large fixtures in `Fixtures/{endpoint}_success.json`.

## Test Isolation

- No shared mutable static state between tests
- No test order dependency
- Each test creates its own client instance
- Cleanup via xUnit class fixtures (`IAsyncLifetime`), not static destructors

## Integration Tests (`TorBoxSDK.IntegrationTests`)

```csharp
[Fact]
[Trait("Category", "Integration")]
public async Task GetMeAsync_WithRealApiKey_ReturnsUser()
{
    string? apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY");
    Skip.If(string.IsNullOrEmpty(apiKey), "TORBOX_API_KEY not set.");
    // ...
}
```

- Always skip gracefully when `TORBOX_API_KEY` is absent
- Always clean up created resources at test end
- `[Trait("Category", "Integration")]` on every integration test

## Performance Tests (`TorBoxSDK.PerformanceTests`)

- BenchmarkDotNet — no xUnit `[Fact]` on benchmark methods
- `[MemoryDiagnoser]` on every benchmark class
- One operation per benchmark
- No I/O in baseline benchmarks unless measuring HTTP overhead
- Do not delete existing benchmarks without justification

---

# Part 5 — Samples (`samples/**/*.cs`)

Samples must be **self-explanatory to an external C# developer** who has never read the SDK source.

## No Magic Values

```csharp
// REJECT
Torrent torrent = await client.Main.Torrents.GetMyListAsync(123);

// ACCEPT
long torrentId = 123; // Replace with your actual torrent ID
Torrent torrent = await client.Main.Torrents.GetMyListAsync(torrentId);
```

## DI Setup — Always `AddTorBox()`

```csharp
// REJECT
TorBoxClient client = new(new TorBoxClientOptions { ApiKey = "my-key" });

// ACCEPT
IServiceCollection services = new ServiceCollection();
services.AddTorBox(options =>
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable."));
ITorBoxClient client = services.BuildServiceProvider().GetRequiredService<ITorBoxClient>();
```

## No Hardcoded API Key

```csharp
// REJECT — credential exposure
options.ApiKey = "tb-prod-abc123def456";

// ACCEPT
options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
    ?? throw new InvalidOperationException("Set the TORBOX_API_KEY environment variable.");
```

## Error Handling

Every API call must demonstrate `TorBoxException` handling:

```csharp
try
{
    Torrent torrent = await client.Main.Torrents.GetMyListAsync(torrentId, cts.Token);
    Console.WriteLine($"Name: {torrent.Name}, Status: {torrent.Status}");
}
catch (TorBoxException ex)
{
    Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail}");
}
```

Reject: API call with no try/catch and no note about exceptions; silently swallowed `catch { }`.

## CancellationToken

All async calls must pass a token:

```csharp
// ACCEPT
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
Torrent torrent = await client.Main.Torrents.GetMyListAsync(id, cts.Token);
```

If using `CancellationToken.None`, add a comment explaining the choice.

## Sample Completeness

- No `TODO` or `NotImplementedException` in final samples
- `using` directives organized: System → Microsoft → TorBoxSDK
- `var` must not be used on SDK types — samples are teaching material

## Prohibited Patterns

| Pattern | Reason |
|---------|--------|
| `result!` without comment | Teaches bad nullability habits |
| `var` on SDK types | Hides the SDK's type surface |
| `Task.Run(() => client.GetAsync(...).Result)` | Deadlock risk |
| `await Task.Delay(...)` as workaround | Hides real async patterns |
| Nested await chains without intermediate variables | Unreadable |
