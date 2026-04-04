namespace TorBoxSDK.Main.Queued;

/// <summary>
/// Default implementation of <see cref="IQueuedClient"/> for managing
/// queued items through the TorBox Main API.
/// </summary>
public sealed class QueuedClient : IQueuedClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueuedClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public QueuedClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
