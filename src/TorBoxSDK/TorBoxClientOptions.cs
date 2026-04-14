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
    /// Gets or sets the base URL for the Main API (host only, without version path).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://api.torbox.app/</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// The trailing slash is required for correct relative URI resolution.
    /// Do not include the version segment — use <see cref="ApiVersion"/> instead.
    /// </remarks>
    public string MainApiBaseUrl { get; set; } = "https://api.torbox.app/";

    /// <summary>
    /// Gets or sets the API version segment used to build the Main API path.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>v1</c>. Combined with <see cref="MainApiBaseUrl"/>
    /// to produce the full base URL (e.g. <c>https://api.torbox.app/v1/api/</c>).
    /// </remarks>
    [Required]
    public string ApiVersion { get; set; } = "v1";

    /// <summary>
    /// Gets the fully-qualified Main API base URL including the version path.
    /// </summary>
    internal string MainApiVersionedUrl
    {
        get
        {
            string trimmed = MainApiBaseUrl.TrimEnd('/');

            // Detect if the base URL already contains a version path (e.g. "/v1/api/")
            // to avoid doubling the path when a consumer has legacy configuration.
            if (trimmed.EndsWith($"/{ApiVersion}/api", StringComparison.OrdinalIgnoreCase) ||
                trimmed.EndsWith($"/{ApiVersion}", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed + "/";
            }

            return $"{trimmed}/{ApiVersion}/api/";
        }
    }

    /// <summary>
    /// Gets or sets the base URL for the Search API.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://search-api.torbox.app/</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// The trailing slash is required for correct relative URI resolution.
    /// </remarks>
    public string SearchApiBaseUrl { get; set; } = "https://search-api.torbox.app/";

    /// <summary>
    /// Gets or sets the base URL for the Relay API (host only, without version path).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>https://relay.torbox.app/</c>.
    /// Override this value for testing or when using a custom endpoint.
    /// The trailing slash is required for correct relative URI resolution.
    /// Do not include the version segment — use <see cref="ApiVersion"/> instead.
    /// </remarks>
    public string RelayApiBaseUrl { get; set; } = "https://relay.torbox.app/";

    /// <summary>
    /// Gets the fully-qualified Relay API base URL including the version path.
    /// </summary>
    internal string RelayApiVersionedUrl
    {
        get
        {
            string trimmed = RelayApiBaseUrl.TrimEnd('/');

            // Detect if the base URL already contains the version path
            // to avoid doubling the path when a consumer has legacy configuration.
            if (trimmed.EndsWith($"/{ApiVersion}", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed + "/";
            }

            return $"{trimmed}/{ApiVersion}/";
        }
    }

    /// <summary>
    /// Gets or sets the HTTP request timeout.
    /// </summary>
    /// <remarks>
    /// Defaults to 30 seconds. Increase this value for large file transfers or slow connections.
    /// </remarks>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
