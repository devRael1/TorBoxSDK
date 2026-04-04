using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to edit the properties of an existing torrent.
/// </summary>
public sealed record EditTorrentRequest
{
    /// <summary>
    /// Gets the unique identifier of the torrent to edit.
    /// </summary>
    [JsonPropertyName("torrent_id")]
    public long TorrentId { get; init; }

    /// <summary>
    /// Gets the new name for the torrent,
    /// or <see langword="null"/> to leave the name unchanged.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the tags to apply to the torrent,
    /// or <see langword="null"/> to leave tags unchanged.
    /// </summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>
    /// Gets the alternative info hashes for the torrent,
    /// or <see langword="null"/> to leave alternative hashes unchanged.
    /// </summary>
    [JsonPropertyName("alternative_hashes")]
    public IReadOnlyList<string>? AlternativeHashes { get; init; }
}
