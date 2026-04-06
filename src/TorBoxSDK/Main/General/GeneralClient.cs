using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.General;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Main.General;

/// <summary>
/// Default implementation of <see cref="IGeneralClient"/> for general
/// TorBox API operations including status, statistics, and speedtest.
/// </summary>
public sealed class GeneralClient : IGeneralClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public GeneralClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetUpStatusAsync(CancellationToken cancellationToken = default)
    {
        // An empty relative URI resolves to the API base address itself (GET /v1/api/).
        using var request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<Stats>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "stats");
        return await TorBoxApiHelper.SendAsync<Stats>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<Stats>> Get30DayStatsAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "stats/30days");
        return await TorBoxApiHelper.SendAsync<Stats>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<object>> GetSpeedtestFilesAsync(SpeedtestOptions? options = null, CancellationToken cancellationToken = default)
    {
        string query = TorBoxApiHelper.BuildQuery(
            ("user_ip", options?.UserIp),
            ("region", options?.Region),
            ("test_length", options?.TestLength?.ToString()));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"speedtest{query}");
        return await TorBoxApiHelper.SendAsync<object>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetChangelogsRssAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "changelogs/rss");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "changelogs/json");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Changelog>>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }
}
