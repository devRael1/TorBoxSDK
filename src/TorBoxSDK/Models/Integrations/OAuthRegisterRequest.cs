using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents a request to register an OAuth integration with a provider.
/// </summary>
public sealed record OAuthRegisterRequest
{
    /// <summary>
    /// Gets the OAuth provider name (e.g., "google", "dropbox").
    /// </summary>
    [JsonIgnore]
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the OAuth access token for the provider.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Gets the OAuth refresh token for the provider.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;
}
