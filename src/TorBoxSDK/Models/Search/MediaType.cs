using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Specifies the type of media to filter search results by.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MediaType
{
    /// <summary>Unknown or unspecified media type.</summary>
    Unknown = 0,

    /// <summary>Movie content.</summary>
    Movie,

    /// <summary>Television content.</summary>
    Tv,

    /// <summary>Anime content.</summary>
    Anime,

    /// <summary>Music content.</summary>
    Music,

    /// <summary>Game content.</summary>
    Game,

    /// <summary>Book content.</summary>
    Book,

    /// <summary>Software content.</summary>
    Software,

    /// <summary>Other or uncategorized content.</summary>
    Other
}
