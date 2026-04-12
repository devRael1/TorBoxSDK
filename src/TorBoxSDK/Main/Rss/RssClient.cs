using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Rss;

namespace TorBoxSDK.Main.Rss;

/// <summary>
/// Default implementation of <see cref="IRssClient"/> for managing
/// RSS feeds through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RssClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class RssClient(HttpClient httpClient) : IRssClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TorBoxResponse> AddRssAsync(AddRssRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/addrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ControlRssAsync(ControlRssRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/controlrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ModifyRssAsync(ModifyRssRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rss/modifyrss")
        {
            Content = TorBoxApiHelper.JsonContent(request),
        };
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<RssFeed>>> GetFeedsAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "rss/getfeeds");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<RssFeed>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<RssFeedItem>>> GetFeedItemsAsync(long rssFeedId, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(("rss_feed_id", rssFeedId.ToString()));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"rss/getfeeditems{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<RssFeedItem>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
