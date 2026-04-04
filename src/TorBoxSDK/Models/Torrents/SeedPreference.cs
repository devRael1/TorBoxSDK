using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Specifies the seeding behavior for a torrent after download completion.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeedPreference
{
    /// <summary>Automatically determine seeding behavior based on user settings.</summary>
    Auto = 1,

    /// <summary>Seed the torrent after download completion.</summary>
    Seed = 2,

    /// <summary>Do not seed the torrent after download completion.</summary>
    NoSeed = 3,
}
