using System.Net.Http.Headers;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Shared xUnit class fixture that provides an <see cref="HttpClient"/> pre-configured to
/// call the live TorBox API for schema validation tests.
/// </summary>
/// <remarks>
/// The client is initialised with the <c>TORBOX_API_KEY</c> environment variable when available.
/// Tests should check <see cref="HasApiKey"/> and skip gracefully when the key is absent.
/// </remarks>
public sealed class SchemaLiveTestFixture : IAsyncLifetime
{
    private static readonly Uri TorBoxBaseUri = new("https://api.torbox.app");

    private HttpClient? _httpClient;

    /// <summary>
    /// Gets a value indicating whether the <c>TORBOX_API_KEY</c> environment variable is set.
    /// </summary>
    public bool HasApiKey { get; }

    /// <summary>
    /// Gets the <see cref="HttpClient"/> configured to call the live TorBox API.
    /// </summary>
    public HttpClient HttpClient =>
        _httpClient ?? throw new InvalidOperationException("Fixture has not been initialised.");

    /// <summary>Initialises the fixture and creates the <see cref="HttpClient"/>.</summary>
    public SchemaLiveTestFixture()
    {
        string? apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY");
        HasApiKey = !string.IsNullOrWhiteSpace(apiKey);

        _httpClient = new HttpClient { BaseAddress = TorBoxBaseUri };

        if (HasApiKey)
        {
            // Non-null because HasApiKey is true only when apiKey is non-empty.
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey!);
        }
    }

    /// <inheritdoc />
    public Task InitializeAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        _httpClient?.Dispose();
        _httpClient = null;
        return Task.CompletedTask;
    }
}
