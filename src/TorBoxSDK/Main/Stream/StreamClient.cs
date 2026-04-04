namespace TorBoxSDK.Main.Stream;

/// <summary>
/// Default implementation of <see cref="IStreamClient"/> for streaming
/// content through the TorBox Main API.
/// </summary>
public sealed class StreamClient : IStreamClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public StreamClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
