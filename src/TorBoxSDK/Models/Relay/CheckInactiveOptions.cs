using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Relay;

/// <summary>
/// Represents the parameters for checking whether a torrent is inactive on the relay.
/// </summary>
/// <remarks>
/// These values are used as URL path segments for the inactive check endpoint.
/// </remarks>
public sealed record CheckInactiveOptions
{
    /// <summary>
    /// Gets the authentication identifier of the user.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string AuthId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier of the torrent to check.
    /// </summary>
    [JsonPropertyName("torrent_id")]
    public long TorrentId { get; init; }
}
