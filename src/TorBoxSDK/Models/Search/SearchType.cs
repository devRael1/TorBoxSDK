using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Specifies the type of search to perform against the TorBox Search API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchType
{
    /// <summary>Unknown or unspecified search type.</summary>
    Unknown = 0,

    /// <summary>Search for torrent results.</summary>
    Torrent,

    /// <summary>Search for usenet results.</summary>
    Usenet,

    /// <summary>Search for metadata results.</summary>
    Meta
}
