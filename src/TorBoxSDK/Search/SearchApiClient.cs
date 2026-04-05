using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Search;

/// <summary>
/// Default implementation of <see cref="ISearchApiClient"/> for querying
/// torrent and usenet indexers and metadata through the TorBox Search API.
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

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetTorrentSearchTutorialAsync(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "torrents");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<TorrentSearchResult>>> SearchTorrentsAsync(string query, TorrentSearchOptions? options = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(query, nameof(query));

        string queryString = BuildTorrentSearchQuery(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"torrents/search/{Uri.EscapeDataString(query)}{queryString}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<TorrentSearchResult>>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<TorrentSearchResult>> GetTorrentByIdAsync(string id, TorrentSearchOptions? options = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(id, nameof(id));

        string queryString = BuildTorrentSearchQuery(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"torrents/{Uri.EscapeDataString(id)}{queryString}");
        return await TorBoxApiHelper.SendAsync<TorrentSearchResult>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetUsenetSearchTutorialAsync(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "usenet");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<UsenetSearchResult>>> SearchUsenetAsync(string query, UsenetSearchOptions? options = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(query, nameof(query));

        string queryString = BuildUsenetSearchQuery(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"usenet/search/{Uri.EscapeDataString(query)}{queryString}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<UsenetSearchResult>>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<UsenetSearchResult>> GetUsenetByIdAsync(string id, UsenetSearchOptions? options = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(id, nameof(id));

        string queryString = BuildUsenetSearchQuery(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"usenet/{Uri.EscapeDataString(id)}{queryString}");
        return await TorBoxApiHelper.SendAsync<UsenetSearchResult>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> DownloadUsenetAsync(string id, string guid, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(id, nameof(id));
        Guard.ThrowIfNullOrEmpty(guid, nameof(guid));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"usenet/download/{Uri.EscapeDataString(id)}/{Uri.EscapeDataString(guid)}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetMetaSearchTutorialAsync(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "meta");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<MetaSearchResult>>> SearchMetaAsync(string query, MetaSearchOptions? options = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(query, nameof(query));

        string queryString = TorBoxApiHelper.BuildQuery(
            ("type", options?.Type));
        using var request = new HttpRequestMessage(HttpMethod.Get, $"meta/search/{Uri.EscapeDataString(query)}{queryString}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<MetaSearchResult>>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<MetaSearchResult>> GetMetaByIdAsync(string id, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(id, nameof(id));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"meta/{Uri.EscapeDataString(id)}");
        return await TorBoxApiHelper.SendAsync<MetaSearchResult>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> SearchTorznabAsync(string query, string? apiKey = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(query, nameof(query));

        string queryString = TorBoxApiHelper.BuildQuery(
            ("t", "search"),
            ("q", query),
            ("apikey", apiKey));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"torznab/api{queryString}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> SearchNewznabAsync(string query, string? apiKey = null, CancellationToken ct = default)
    {
        Guard.ThrowIfNullOrEmpty(query, nameof(query));

        string queryString = TorBoxApiHelper.BuildQuery(
            ("t", "search"),
            ("q", query),
            ("apikey", apiKey));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"newznab/api{queryString}");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, ct).ConfigureAwait(false);
    }

    private static string BuildTorrentSearchQuery(TorrentSearchOptions? options)
    {
        if (options is null)
        {
            return string.Empty;
        }

        return TorBoxApiHelper.BuildQuery(
            ("metadata", options.Metadata?.ToString().ToLowerInvariant()),
            ("season", options.Season?.ToString()),
            ("episode", options.Episode?.ToString()),
            ("check_cache", options.CheckCache?.ToString().ToLowerInvariant()),
            ("check_owned", options.CheckOwned?.ToString().ToLowerInvariant()),
            ("search_user_engines", options.SearchUserEngines?.ToString().ToLowerInvariant()),
            ("cached_only", options.CachedOnly?.ToString().ToLowerInvariant()));
    }

    private static string BuildUsenetSearchQuery(UsenetSearchOptions? options)
    {
        if (options is null)
        {
            return string.Empty;
        }

        return TorBoxApiHelper.BuildQuery(
            ("metadata", options.Metadata?.ToString().ToLowerInvariant()),
            ("season", options.Season?.ToString()),
            ("episode", options.Episode?.ToString()),
            ("check_cache", options.CheckCache?.ToString().ToLowerInvariant()),
            ("check_owned", options.CheckOwned?.ToString().ToLowerInvariant()),
            ("search_user_engines", options.SearchUserEngines?.ToString().ToLowerInvariant()),
            ("cached_only", options.CachedOnly?.ToString().ToLowerInvariant()));
    }
}
