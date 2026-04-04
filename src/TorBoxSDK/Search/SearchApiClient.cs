namespace TorBoxSDK.Search;

/// <summary>
/// Default implementation of <see cref="ISearchApiClient"/> for querying
/// torrent and usenet indexers through the TorBox Search API.
/// </summary>
public sealed class SearchApiClient : ISearchApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Search API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public SearchApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
