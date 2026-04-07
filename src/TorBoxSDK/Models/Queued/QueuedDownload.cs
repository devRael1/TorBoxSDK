using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Queued;

/// <summary>
/// Represents a download that has been queued for later processing.
/// </summary>
public sealed record QueuedDownload
{
    /// <summary>
    /// Gets the unique identifier of the queued download.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the date and time when the download was queued,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the magnet URI for the queued download,
    /// or <see langword="null"/> if not applicable.
    /// </summary>
    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    /// <summary>
    /// Gets the path to the torrent file for the queued download,
    /// or <see langword="null"/> if not applicable.
    /// </summary>
    [JsonPropertyName("torrent_file")]
    public string? TorrentFile { get; init; }

    /// <summary>
    /// Gets the hash of the queued download, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Gets the display name of the queued download,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the type of download (e.g., "torrent", "usenet", "web_download"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the name override for the queued download,
    /// or <see langword="null"/> if not set.
    /// </summary>
    [JsonPropertyName("name_override")]
    public string? NameOverride { get; init; }

    /// <summary>
    /// Gets the seed torrent override value,
    /// or <see langword="null"/> if not set.
    /// </summary>
    [JsonPropertyName("seed_torrent_override")]
    public int? SeedTorrentOverride { get; init; }
}
