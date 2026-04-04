namespace TorBoxSDK.Main.Torrents;

/// <summary>
/// Default implementation of <see cref="ITorrentsClient"/> for managing
/// torrents through the TorBox Main API.
/// </summary>
public sealed class TorrentsClient : ITorrentsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TorrentsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public TorrentsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
