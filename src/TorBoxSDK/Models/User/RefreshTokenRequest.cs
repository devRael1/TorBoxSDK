using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to refresh an authentication session token.
/// </summary>
public sealed record RefreshTokenRequest
{
    /// <summary>
    /// Gets the current session token to refresh.
    /// </summary>
    [JsonPropertyName("session_token")]
    public string SessionToken { get; init; } = string.Empty;
}
