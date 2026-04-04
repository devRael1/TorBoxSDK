using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.WebDownloads;

/// <summary>
/// Represents a supported file hoster and its current bandwidth status.
/// </summary>
public sealed record Hoster
{
    /// <summary>
    /// Gets the name of the file hoster.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the daily bandwidth limit in bytes for this hoster,
    /// or <see langword="null"/> if no limit is set.
    /// </summary>
    [JsonPropertyName("daily_bandwidth_limit")]
    public long? DailyBandwidthLimit { get; init; }

    /// <summary>
    /// Gets the amount of daily bandwidth already used in bytes for this hoster,
    /// or <see langword="null"/> if not tracked.
    /// </summary>
    [JsonPropertyName("daily_bandwidth_used")]
    public long? DailyBandwidthUsed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the hoster is currently operational.
    /// </summary>
    [JsonPropertyName("status")]
    public bool Status { get; init; }
}
