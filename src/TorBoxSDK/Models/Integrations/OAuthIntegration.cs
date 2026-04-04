using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents an OAuth-based integration connected to a TorBox account.
/// </summary>
public sealed record OAuthIntegration
{
    /// <summary>
    /// Gets the name of the integration provider (e.g., Google Drive, OneDrive),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; init; }

    /// <summary>
    /// Gets a value indicating whether the integration is currently connected.
    /// </summary>
    [JsonPropertyName("connected")]
    public bool Connected { get; init; }

    /// <summary>
    /// Gets the date and time when the integration was connected,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
