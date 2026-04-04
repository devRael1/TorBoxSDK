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
}
