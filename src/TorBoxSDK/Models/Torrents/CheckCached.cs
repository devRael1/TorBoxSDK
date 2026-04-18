using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents the response from checking whether torrents are cached on TorBox.
/// </summary>
public sealed record CheckCached
{
    /// <summary>
    /// Gets the cache status data, where each key is a hash and value indicates cache status.
    /// The value can be an integer (number of cached chunks) or a hash info object depending on format.
    /// </summary>
    [JsonPropertyName("data")]
    public IReadOnlyDictionary<string, object?>? Data { get; init; }
}
