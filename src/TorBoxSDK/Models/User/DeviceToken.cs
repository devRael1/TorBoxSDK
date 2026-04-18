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
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    /// <summary>
    /// Gets the token expiration time, or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; init; }

    /// <summary>
    /// Gets additional token-related data, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("extra")]
    public object? Extra { get; init; }
}
