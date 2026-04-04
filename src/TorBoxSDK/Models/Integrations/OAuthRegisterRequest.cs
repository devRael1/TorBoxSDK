using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents a request to register an OAuth integration with a provider.
/// </summary>
public sealed record OAuthRegisterRequest
{
    /// <summary>
    /// Gets the provider name,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; init; }

    /// <summary>
    /// Gets the OAuth authorization code,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Gets the redirect URI used during the OAuth flow,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; init; }
}
