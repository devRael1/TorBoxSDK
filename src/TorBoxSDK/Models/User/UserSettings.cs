using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the configurable settings for a TorBox user account.
/// </summary>
public sealed record UserSettings
{
    /// <summary>
    /// Gets a value indicating whether magnet history saving is enabled,
    /// or <see langword="null"/> if the setting is not specified.
    /// </summary>
    [JsonPropertyName("save_magnet_history")]
    public bool? SaveMagnetHistory { get; init; }

    /// <summary>
    /// Gets the default download behavior setting,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("download_behavior")]
    public string? DownloadBehavior { get; init; }

    /// <summary>
    /// Gets the torrent seed preference as a numeric value,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("torrent_seed_preference")]
    public int? TorrentSeedPreference { get; init; }

    /// <summary>
    /// Gets the default name to use for new torrents,
    /// or <see langword="null"/> if not specified.
    /// </summary>
    [JsonPropertyName("default_torrent_name")]
    public string? DefaultTorrentName { get; init; }

    /// <summary>
    /// Gets a value indicating whether notifications are enabled,
    /// or <see langword="null"/> if the setting is not specified.
    /// </summary>
    [JsonPropertyName("enable_notifications")]
    public bool? EnableNotifications { get; init; }
}
