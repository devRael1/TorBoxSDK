namespace TorBoxSDK.Main.WebDownloads;

/// <summary>
/// Default implementation of <see cref="IWebDownloadsClient"/> for managing
/// web downloads through the TorBox Main API.
/// </summary>
public sealed class WebDownloadsClient : IWebDownloadsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDownloadsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public WebDownloadsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
