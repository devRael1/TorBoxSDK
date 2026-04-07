using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents the query parameters for retrieving torrent metadata from a hash (GET endpoint).
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record GetTorrentInfoOptions
{
    /// <summary>
    /// Gets the info hash of the torrent.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional timeout in seconds for metadata retrieval,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; init; }

    /// <summary>
    /// Gets a value indicating whether to enable cache lookup for the torrent info,
    /// or <see langword="null"/> to use the default behavior.
    /// </summary>
    [JsonPropertyName("use_cache_lookup")]
    public bool? UseCacheLookup { get; init; }
}
