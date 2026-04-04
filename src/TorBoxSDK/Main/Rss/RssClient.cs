using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;

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

    /// <inheritdoc />
    public async Task<TorBoxResponse> AddRssAsync(AddRssRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/addrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlRssAsync(ControlRssRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/controlrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ModifyRssAsync(ModifyRssRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/modifyrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<RssFeed>>> GetFeedsAsync(CancellationToken ct = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "rss/getfeeds");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<RssFeed>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<RssFeedItem>>> GetFeedItemsAsync(long rssFeedId, CancellationToken ct = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("rss_feed_id", rssFeedId.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"rss/getfeeditems{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<RssFeedItem>>(_httpClient, httpRequest, ct).ConfigureAwait(false);
    }
}
