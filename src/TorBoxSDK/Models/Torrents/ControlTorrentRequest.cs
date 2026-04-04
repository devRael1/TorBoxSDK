using System.Text.Json.Serialization;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to perform a control operation on one or more torrents.
/// </summary>
public sealed record ControlTorrentRequest
{
    /// <summary>
    /// Gets the identifier of the torrent to control,
    /// or <see langword="null"/> when <see cref="All"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("torrent_id")]
    public long? TorrentId { get; init; }

    /// <summary>
    /// Gets the control operation to perform on the torrent.
    /// </summary>
    [JsonPropertyName("operation")]
    public ControlOperation Operation { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should apply to all torrents,
    /// or <see langword="null"/> to apply to a single torrent identified by <see cref="TorrentId"/>.
    /// </summary>
    [JsonPropertyName("all")]
    public bool? All { get; init; }
}
