using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the response from exchanging a device code for an access token.
/// </summary>
public sealed record DeviceToken
{
    /// <summary>
    /// Gets the access token, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    /// <summary>
    /// Gets the token type (usually <c>Bearer</c>), or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }
}
