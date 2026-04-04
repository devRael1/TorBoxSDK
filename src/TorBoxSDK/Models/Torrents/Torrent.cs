using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a torrent download item returned by the TorBox API.
/// </summary>
public sealed record Torrent
{
    /// <summary>
    /// Gets the unique identifier of the torrent.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the authentication identifier of the user who owns this torrent,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string? AuthId { get; init; }

    /// <summary>
    /// Gets the info hash of the torrent, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Gets the display name of the torrent.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the magnet URI of the torrent, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    /// <summary>
    /// Gets the total size of the torrent in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Gets a value indicating whether the torrent is currently active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool Active { get; init; }

    /// <summary>
    /// Gets the date and time when the torrent was created, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the torrent was last updated, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the torrent will expire, or <see langword="null"/> if not set.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Gets the current download state as a string descriptor,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("download_state")]
    public string? DownloadState { get; init; }

    /// <summary>
    /// Gets the current download speed in bytes per second.
    /// </summary>
    [JsonPropertyName("download_speed")]
    public long DownloadSpeed { get; init; }

    /// <summary>
    /// Gets the current upload speed in bytes per second.
    /// </summary>
    [JsonPropertyName("upload_speed")]
    public long UploadSpeed { get; init; }

    /// <summary>
    /// Gets the number of seeds connected to this torrent.
    /// </summary>
    [JsonPropertyName("seeds")]
    public int Seeds { get; init; }

    /// <summary>
    /// Gets the number of peers connected to this torrent.
    /// </summary>
    [JsonPropertyName("peers")]
    public int Peers { get; init; }

    /// <summary>
    /// Gets the upload-to-download ratio of the torrent.
    /// </summary>
    [JsonPropertyName("ratio")]
    public double Ratio { get; init; }

    /// <summary>
    /// Gets the download progress as a value between 0.0 and 1.0.
    /// </summary>
    [JsonPropertyName("progress")]
    public double Progress { get; init; }

    /// <summary>
    /// Gets the availability of the torrent data across connected peers.
    /// </summary>
    [JsonPropertyName("availability")]
    public double Availability { get; init; }

    /// <summary>
    /// Gets the estimated time of arrival (completion) in seconds.
    /// </summary>
    [JsonPropertyName("eta")]
    public long Eta { get; init; }

    /// <summary>
    /// Gets a value indicating whether the download has finished.
    /// </summary>
    [JsonPropertyName("download_finished")]
    public bool DownloadFinished { get; init; }

    /// <summary>
    /// Gets a value indicating whether the downloaded data is present on the server.
    /// </summary>
    [JsonPropertyName("download_present")]
    public bool DownloadPresent { get; init; }

    /// <summary>
    /// Gets a value indicating whether the original torrent file is available.
    /// </summary>
    [JsonPropertyName("torrent_file")]
    public bool TorrentFile { get; init; }

    /// <summary>
    /// Gets the inactive check interval in seconds.
    /// </summary>
    [JsonPropertyName("inactive_check")]
    public long InactiveCheck { get; init; }

    /// <summary>
    /// Gets the server identifier where the torrent is hosted.
    /// </summary>
    [JsonPropertyName("server")]
    public int Server { get; init; }

    /// <summary>
    /// Gets the list of files contained in this torrent.
    /// </summary>
    [JsonPropertyName("files")]
    public IReadOnlyList<DownloadFile> Files { get; init; } = [];
}
