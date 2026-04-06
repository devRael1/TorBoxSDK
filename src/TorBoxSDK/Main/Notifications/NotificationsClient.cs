using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Notifications;

namespace TorBoxSDK.Main.Notifications;

/// <summary>
/// Default implementation of <see cref="INotificationsClient"/> for managing
/// notifications through the TorBox Main API.
/// </summary>
public sealed class NotificationsClient : INotificationsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public NotificationsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

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
    public async Task<TorBoxResponse<IntercomHash>> GetIntercomHashAsync(string authId, string email, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(authId, nameof(authId));
        Guard.ThrowIfNullOrEmpty(email, nameof(email));

        string query = TorBoxApiHelper.BuildQuery(
            ("auth_id", authId),
            ("email", email));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"intercom/hash{query}");
        return await TorBoxApiHelper.SendAsync<IntercomHash>(_httpClient, httpRequest, cancellationToken).ConfigureAwait(false);
    }
}
