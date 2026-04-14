using System.IO;
using System.Xml.Serialization;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;

namespace TorBoxSDK.Main.General;

/// <summary>
/// Default implementation of <see cref="IGeneralClient"/> for general
/// TorBox API operations including status, statistics, and speedtest.
/// </summary>
internal sealed class GeneralClient : IGeneralClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _rootUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <param name="baseUrl">The root Main API URL (without version), used for the up-status endpoint.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    internal GeneralClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!string.IsNullOrEmpty(baseUrl))
        {
            _rootUrl = new Uri(baseUrl);
        }
        else if (httpClient.BaseAddress is not null)
        {
            _rootUrl = new Uri($"{httpClient.BaseAddress.Scheme}://{httpClient.BaseAddress.Host}/");
        }
        else
        {
            _rootUrl = new Uri("https://api.torbox.app/");
        }
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetUpStatusAsync(CancellationToken cancellationToken = default)
    {
        // Use the root API URL (without version path) as an absolute URI.
        using var request = new HttpRequestMessage(HttpMethod.Get, _rootUrl);
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<Stats>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "stats");
        return await TorBoxApiHelper.SendAsync<Stats>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<DailyStats>>> Get30DayStatsAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "stats/30days");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<DailyStats>>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<SpeedtestServer>>> GetSpeedtestFilesAsync(SpeedtestOptions? options = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("user_ip", options?.UserIp),
            ("region", options?.Region),
            ("test_length", options?.TestLength));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"speedtest{query}");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<SpeedtestServer>>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<RssFeed>> GetChangelogsRssAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "changelogs/rss");
        using HttpResponseMessage httpResponse = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string content = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new TorBoxException(
                $"HTTP {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}",
                TorBoxErrorCode.ServerError,
                content);
        }

        XmlSerializer serializer = new(typeof(RssFeed));
        using StringReader reader = new(content);
        RssFeed? feed = (RssFeed?)serializer.Deserialize(reader);

        return new TorBoxResponse<RssFeed>
        {
            Success = true,
            Data = feed
        };
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "changelogs/json");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Changelog>>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }
}
