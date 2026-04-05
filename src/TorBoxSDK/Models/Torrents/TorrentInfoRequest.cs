using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to retrieve torrent metadata from a torrent file,
/// magnet URI, or info hash via the POST endpoint.
/// </summary>
/// <remarks>
/// The <see cref="File"/> and <see cref="FileName"/> properties are excluded from
/// JSON serialization because this request is sent as multipart form data.
/// </remarks>
public sealed record TorrentInfoRequest
{
    /// <summary>
    /// Gets the raw torrent file bytes, or <see langword="null"/> if a magnet URI or hash is provided instead.
    /// </summary>
    /// <remarks>
    /// This property is excluded from JSON serialization and is handled separately
    /// as a file upload in multipart form data requests.
    /// </remarks>
    [JsonIgnore]
    public byte[]? File { get; init; }

    /// <summary>
    /// Gets the file name to use when uploading the torrent file,
    /// or <see langword="null"/> to use the default name.
    /// </summary>
    /// <remarks>
    /// This property is excluded from JSON serialization and is only used
    /// as the filename parameter in multipart form data requests.
    /// </remarks>
    [JsonIgnore]
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the magnet URI to retrieve torrent metadata from,
    /// or <see langword="null"/> if a file or hash is provided instead.
    /// </summary>
    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    /// <summary>
    /// Gets the info hash to retrieve torrent metadata from,
    /// or <see langword="null"/> if a file or magnet URI is provided instead.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Gets the timeout in seconds for metadata retrieval,
    /// or <see langword="null"/> to use the default timeout.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; init; }

    /// <summary>
    /// Gets a value indicating whether to use cache lookup for the torrent info,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("use_cache_lookup")]
    public bool? UseCacheLookup { get; init; }

    /// <summary>
    /// Gets a value indicating whether to return only peer information,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("peers_only")]
    public bool? PeersOnly { get; init; }
}
