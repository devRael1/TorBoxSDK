using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>
/// Represents aggregate statistics about the TorBox platform.
/// </summary>
public sealed record Stats
{
    /// <summary>
    /// Gets the total number of registered users on the platform.
    /// </summary>
    [JsonPropertyName("total_users")]
    public long TotalUsers { get; init; }

    /// <summary>
    /// Gets the total number of servers currently available.
    /// </summary>
    [JsonPropertyName("total_servers")]
    public int TotalServers { get; init; }
}
