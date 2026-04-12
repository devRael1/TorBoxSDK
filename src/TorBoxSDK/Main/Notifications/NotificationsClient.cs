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
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
/// </exception>
internal sealed class NotificationsClient(HttpClient httpClient) : INotificationsClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TorBoxResponse<string>> GetNotificationRssAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "notifications/rss");
        return await TorBoxApiHelper.SendAsync<string>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
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
        ArgumentNullException.ThrowIfNull(options);
        Guard.ThrowIfNullOrEmpty(options.AuthId, nameof(options.AuthId));
        Guard.ThrowIfNullOrEmpty(options.Email, nameof(options.Email));

        string query = TorBoxApiHelper.BuildQuery(
            ("auth_id", options.AuthId),
            ("email", options.Email));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"intercom/hash{query}");
        return await TorBoxApiHelper.SendAsync<IntercomHash>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
