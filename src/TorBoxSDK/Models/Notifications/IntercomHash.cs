using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents the Intercom identity hash for the authenticated user.
/// </summary>
public sealed record IntercomHash
{
    /// <summary>
    /// Gets the Intercom identity verification hash,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }
}
