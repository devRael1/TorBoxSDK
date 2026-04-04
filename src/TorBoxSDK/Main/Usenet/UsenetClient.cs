namespace TorBoxSDK.Main.Usenet;

/// <summary>
/// Default implementation of <see cref="IUsenetClient"/> for managing
/// usenet downloads through the TorBox Main API.
/// </summary>
public sealed class UsenetClient : IUsenetClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsenetClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public UsenetClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
