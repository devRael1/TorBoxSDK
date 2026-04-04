using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Notifications;

/// <summary>
/// Represents aggregate statistics about TorBox service usage.
/// </summary>
public sealed record Stats
{
    /// <summary>
    /// Gets the number of currently active torrent downloads.
    /// </summary>
    [JsonPropertyName("active_torrents")]
    public int ActiveTorrents { get; init; }

    /// <summary>
    /// Gets the number of currently active Usenet downloads.
    /// </summary>
    [JsonPropertyName("active_usenet")]
    public int ActiveUsenet { get; init; }

    /// <summary>
    /// Gets the number of currently active web downloads.
    /// </summary>
    [JsonPropertyName("active_web_downloads")]
    public int ActiveWebDownloads { get; init; }

    /// <summary>
    /// Gets the total number of torrent downloads completed.
    /// </summary>
    [JsonPropertyName("total_torrents_downloaded")]
    public long TotalTorrentsDownloaded { get; init; }

    /// <summary>
    /// Gets the total number of Usenet downloads completed.
    /// </summary>
    [JsonPropertyName("total_usenet_downloaded")]
    public long TotalUsenetDownloaded { get; init; }

    /// <summary>
    /// Gets the total number of web downloads completed.
    /// </summary>
    [JsonPropertyName("total_web_downloads_downloaded")]
    public long TotalWebDownloadsDownloaded { get; init; }

    /// <summary>
    /// Gets the total number of bytes downloaded across all download types.
    /// </summary>
    [JsonPropertyName("total_bytes_downloaded")]
    public long TotalBytesDownloaded { get; init; }

    /// <summary>
    /// Gets the total number of bytes uploaded across all download types.
    /// </summary>
    [JsonPropertyName("total_bytes_uploaded")]
    public long TotalBytesUploaded { get; init; }
}
