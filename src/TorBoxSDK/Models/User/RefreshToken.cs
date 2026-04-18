using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the response from refreshing an authentication token.
/// </summary>
public sealed record RefreshToken
{
    /// <summary>
    /// Gets the authentication token, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    /// <summary>
    /// Gets additional token-related data, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("extra")]
    public object? Extra { get; init; }
}
