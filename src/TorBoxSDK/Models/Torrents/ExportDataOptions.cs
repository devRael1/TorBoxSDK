using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents the options for exporting torrent data.
/// </summary>
public sealed record ExportDataOptions
{
    /// <summary>
    /// Gets the unique identifier of the torrent to export data for.
    /// </summary>
    [JsonPropertyName("torrent_id")]
    public long TorrentId { get; init; }

    /// <summary>
    /// Gets the export type format, or <see langword="null"/> for the default format.
    /// </summary>
    [JsonPropertyName("type")]
    public string? ExportType { get; init; }
}
