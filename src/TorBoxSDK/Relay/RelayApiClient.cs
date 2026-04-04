namespace TorBoxSDK.Relay;

/// <summary>
/// Default implementation of <see cref="IRelayApiClient"/> for relay-based
/// operations through the TorBox Relay API.
/// </summary>
public sealed class RelayApiClient : IRelayApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Relay API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public RelayApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
