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
    /// Gets the authentication identifier of the user who owns this download,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string? AuthId { get; init; }

    /// <summary>
    /// Gets the display name of the queued download,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the type of download (e.g., torrent, usenet, web_download),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("download_type")]
    public string? DownloadType { get; init; }

    /// <summary>
    /// Gets the magnet URI for the queued download,
    /// or <see langword="null"/> if not applicable.
    /// </summary>
    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    /// <summary>
    /// Gets the hash of the queued download, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Gets the size of the download in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Gets the date and time when the download was queued,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the current status of the queued download,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
