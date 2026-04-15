using System.Xml;
using System.Xml.Linq;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Main.Notifications;

/// <summary>
/// Default implementation of <see cref="INotificationsClient"/> for managing
/// notifications through the TorBox Main API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NotificationsClient"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client configured for the Main API.</param>
/// <param name="apiKey">The TorBox API key used to authenticate RSS feed requests.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class NotificationsClient(HttpClient httpClient, string apiKey) : INotificationsClient
{
    private readonly HttpClient _httpClient = Guard.ThrowIfNull(httpClient);
    private readonly string _apiKey = apiKey ?? string.Empty;

    /// <inheritdoc />
    public async Task<TorBoxResponse<NotificationRssFeed>> GetNotificationRssAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("An API key is required to access the notifications RSS feed. Configure the API key via TorBoxClientOptions.ApiKey.");
        }

        string query = TorBoxApiHelper.BuildQuery(("token", _apiKey));
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"notifications/rss{query}");
        using HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
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
            XDocument doc = XDocument.Parse(content);
            XElement? channel = doc.Root?.Element("channel");

            List<NotificationRssItem> items = [];
            if (channel is not null)
            {
                foreach (XElement item in channel.Elements("item"))
                {
                    DateTimeOffset? pubDate = null;
                    string? pubDateStr = item.Element("pubDate")?.Value;
                    if (pubDateStr is not null &&
                        DateTimeOffset.TryParse(pubDateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTimeOffset parsed))
                    {
                        pubDate = parsed.ToUniversalTime();
                    }

                    items.Add(new NotificationRssItem
                    {
                        Title = item.Element("title")?.Value,
                        Description = item.Element("description")?.Value,
                        Guid = item.Element("guid")?.Value,
                        PubDate = pubDate
                    });
                }
            }

            return new TorBoxResponse<NotificationRssFeed>
            {
                Success = true,
                Data = new NotificationRssFeed
                {
                    Title = channel?.Element("title")?.Value,
                    Link = channel?.Element("link")?.Value,
                    Description = channel?.Element("description")?.Value,
                    Items = items.AsReadOnly()
                }
            };
        }
        catch (XmlException ex)
        {
            throw new TorBoxException(
                "Failed to parse notification RSS feed.",
                TorBoxErrorCode.UnknownError,
                content,
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IReadOnlyList<Notification>>> GetMyNotificationsAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "notifications/mynotifications");
        return await TorBoxApiHelper.SendAsync<IReadOnlyList<Notification>>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ClearAllNotificationsAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "notifications/clear");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> ClearNotificationAsync(long notificationId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"notifications/clear/{notificationId}");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse> SendTestNotificationAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "notifications/test");
        return await TorBoxApiHelper.SendAsync(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TorBoxResponse<IntercomHash>> GetIntercomHashAsync(GetIntercomHashOptions options, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.AuthId, nameof(options.AuthId));
        Guard.ThrowIfNullOrEmpty(options.Email, nameof(options.Email));

        string query = TorBoxApiHelper.BuildQuery(
            ("auth_id", options.AuthId),
            ("email", options.Email));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"intercom/hash{query}");
        return await TorBoxApiHelper.SendAsync<IntercomHash>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
