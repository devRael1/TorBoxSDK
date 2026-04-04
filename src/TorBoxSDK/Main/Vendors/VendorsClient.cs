namespace TorBoxSDK.Main.Vendors;

/// <summary>
/// Default implementation of <see cref="IVendorsClient"/> for managing
/// vendors through the TorBox Main API.
/// </summary>
public sealed class VendorsClient : IVendorsClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendorsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured for the Main API.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpClient"/> is <see langword="null"/>.
    /// </exception>
    public VendorsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
