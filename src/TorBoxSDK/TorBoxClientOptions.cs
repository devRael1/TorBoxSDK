using System.ComponentModel.DataAnnotations;

namespace TorBoxSDK;

/// <summary>
/// Configuration options for the TorBox SDK client.
/// </summary>
/// <remarks>
/// Use this class to configure API keys, base URLs, and timeout settings
/// for the <see cref="TorBoxClient"/> and its sub-clients.
/// </remarks>
public sealed class TorBoxClientOptions
{
    /// <summary>
    /// Gets or sets the API key used to authenticate requests to the TorBox API.
    /// </summary>
    /// <remarks>
    /// This value is required. Obtain an API key from your TorBox account settings.
    /// </remarks>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the Main API.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://api.torbox.app/v1/api</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// </remarks>
    public string MainApiBaseUrl { get; set; } = "https://api.torbox.app/v1/api";

    /// <summary>
    /// Gets or sets the base URL for the Search API.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://search-api.torbox.app</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// </remarks>
    public string SearchApiBaseUrl { get; set; } = "https://search-api.torbox.app";

    /// <summary>
    /// Gets or sets the base URL for the Relay API.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://relay.torbox.app</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// </remarks>
    public string RelayApiBaseUrl { get; set; } = "https://relay.torbox.app";

    /// <summary>
    /// Gets or sets the HTTP request timeout.
    /// </summary>
    /// <remarks>
    /// Defaults to 30 seconds. Increase this value for large file transfers or slow connections.
    /// </remarks>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
