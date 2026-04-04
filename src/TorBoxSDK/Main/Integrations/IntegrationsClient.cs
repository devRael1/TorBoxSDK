namespace TorBoxSDK.Main.Integrations;

/// <summary>
/// Default implementation of <see cref="IIntegrationsClient"/> for managing
/// third-party integrations through the TorBox Main API.
/// </summary>
public sealed class IntegrationsClient : IIntegrationsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public IntegrationsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
