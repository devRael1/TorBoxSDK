namespace TorBoxSDK.Main.Rss;

/// <summary>
/// Default implementation of <see cref="IRssClient"/> for managing
/// RSS feeds through the TorBox Main API.
/// </summary>
public sealed class RssClient : IRssClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RssClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public RssClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
