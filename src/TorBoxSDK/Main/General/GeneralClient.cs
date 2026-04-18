using System.Xml;
using System.Xml.Linq;
using TorBoxSDK.Http;
using TorBoxSDK.Http.Validation;
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
        _httpClient = Guard.ThrowIfNull(httpClient);

        if (!string.IsNullOrEmpty(baseUrl))
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? parsedBaseUrl))
            {
                throw new ArgumentException("Base URL must be a valid absolute URI.", nameof(baseUrl));
            }

            _rootUrl = parsedBaseUrl;
        }
        else
        {
            _rootUrl = httpClient.BaseAddress is not null
                ? new Uri(httpClient.BaseAddress, "/")
                : throw new ArgumentException("A base URL must be provided when HttpClient.BaseAddress is not set.", nameof(baseUrl));
        }
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<UpStatus>> GetUpStatusAsync(CancellationToken cancellationToken = default)
    {
        // Use the root API URL (without version path) as an absolute URI.
        using var request = new HttpRequestMessage(HttpMethod.Get, _rootUrl);
        return await TorBoxApiHelper.SendAsync<UpStatus>(_httpClient, request, cancellationToken).ConfigureAwait(false);
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
    public async Task<TorBoxResponse<ChangelogsRssFeed>> GetChangelogsRssAsync(CancellationToken cancellationToken = default)
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

        try
        {
            var doc = XDocument.Parse(content);
            XElement? rss = doc.Root;
            XElement? channel = rss?.Element("channel");
            XNamespace contentNs = "http://purl.org/rss/1.0/modules/content/";

            List<ChangelogsRssItem> items = [];
            if (channel is not null)
            {
                foreach (XElement item in channel.Elements("item"))
                {
                    items.Add(new ChangelogsRssItem
                    {
                        Title = item.Element("title")?.Value ?? string.Empty,
                        Link = item.Element("link")?.Value ?? string.Empty,
                        Description = item.Element("description")?.Value ?? string.Empty,
                        PubDate = item.Element("pubDate")?.Value ?? string.Empty,
                        ContentEncoded = item.Element(contentNs + "encoded")?.Value
                    });
                }
            }

            ChangelogsRssFeed feed = new()
            {
                Version = rss?.Attribute("version")?.Value ?? string.Empty,
                Channel = channel is null
                    ? null
                    : new ChangelogsRssChannel
                    {
                        Title = channel.Element("title")?.Value ?? string.Empty,
                        Link = channel.Element("link")?.Value ?? string.Empty,
                        Description = channel.Element("description")?.Value ?? string.Empty,
                        Language = channel.Element("language")?.Value ?? string.Empty,
                        LastBuildDate = channel.Element("lastBuildDate")?.Value ?? string.Empty,
                        Items = items.AsReadOnly()
                    }
            };

            return new TorBoxResponse<ChangelogsRssFeed>
            {
                Success = true,
                Data = feed
            };
        }
        catch (XmlException ex)
        {
            throw new TorBoxException(
                "Failed to parse changelogs RSS feed.",
                TorBoxErrorCode.UnknownError,
                content,
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Changelog>>> GetChangelogsJsonAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "changelogs/json");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Changelog>>(_httpClient, request, cancellationToken).ConfigureAwait(false);
    }
}
