using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to create a new torrent download.
/// </summary>
/// <remarks>
/// Either <see cref="Magnet"/> or <see cref="File"/> must be provided, but not both.
/// The <see cref="File"/> property is excluded from JSON serialization because this
/// request is sent as multipart form data.
/// </remarks>
public sealed record CreateTorrentRequest
{
    /// <summary>
    /// Gets the magnet URI of the torrent to download,
    /// or <see langword="null"/> if a torrent file is provided instead.
    /// </summary>
    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    /// <summary>
    /// Gets the raw torrent file bytes, or <see langword="null"/> if a magnet URI is provided instead.
    /// </summary>
    /// <remarks>
    /// This property is excluded from JSON serialization and is handled separately
    /// as a file upload in multipart form data requests.
    /// </remarks>
    [JsonIgnore]
    public byte[]? File { get; init; }

    /// <summary>
    /// Gets an optional override name for the torrent,
    /// or <see langword="null"/> to use the default name from the torrent metadata.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the preferred seeding behavior after download completion,
    /// or <see langword="null"/> to use the user's default setting.
    /// </summary>
    [JsonPropertyName("seed")]
    public SeedPreference? Seed { get; init; }

    /// <summary>
    /// Gets a value indicating whether to allow zipping the completed download,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("allow_zip")]
    public bool? AllowZip { get; init; }

    /// <summary>
    /// Gets a value indicating whether to add the torrent as a queued download,
    /// or <see langword="null"/> to start immediately.
    /// </summary>
    [JsonPropertyName("as_queued")]
    public bool? AsQueued { get; init; }

    /// <summary>
    /// Gets a value indicating whether to only add the torrent if it is already cached,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("add_only_if_cached")]
    public bool? AddOnlyIfCached { get; init; }
}
