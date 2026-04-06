using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents a single metadata search result returned by the TorBox Search API.
/// </summary>
public sealed record MetaSearchResult
{
    /// <summary>
    /// Gets the global unique identifier for this metadata entry,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("globalID")]
    public string? GlobalId { get; init; }

    /// <summary>
    /// Gets the composite identifier of the metadata entry (e.g. "imdb_id:tt0080684"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the IMDb identifier (e.g. "tt1234567"), or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; init; }

    /// <summary>
    /// Gets the TMDb identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("tmdb_id")]
    public long? TmdbId { get; init; }

    /// <summary>
    /// Gets the TVDb identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("tvdb_id")]
    public long? TvdbId { get; init; }

    /// <summary>
    /// Gets the TVmaze identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("tvmaze_id")]
    public long? TvmazeId { get; init; }

    /// <summary>
    /// Gets the Trakt identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("trakt_id")]
    public long? TraktId { get; init; }

    /// <summary>
    /// Gets the MyAnimeList identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("mal_id")]
    public long? MalId { get; init; }

    /// <summary>
    /// Gets the AniList identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("anilist_id")]
    public long? AnilistId { get; init; }

    /// <summary>
    /// Gets the Kitsu identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("kitsu_id")]
    public long? KitsuId { get; init; }

    /// <summary>
    /// Gets the Simkl identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("simkl_id")]
    public long? SimklId { get; init; }

    /// <summary>
    /// Gets the title of the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets alternate titles for the media item.
    /// </summary>
    [JsonPropertyName("titles")]
    public IReadOnlyList<string> Titles { get; init; } = [];

    /// <summary>
    /// Gets keywords associated with the media item.
    /// </summary>
    [JsonPropertyName("keywords")]
    public IReadOnlyList<string> Keywords { get; init; } = [];

    /// <summary>
    /// Gets the genres of the media item.
    /// </summary>
    [JsonPropertyName("genres")]
    public IReadOnlyList<string> Genres { get; init; } = [];

    /// <summary>
    /// Gets the actors featured in the media item.
    /// </summary>
    [JsonPropertyName("actors")]
    public IReadOnlyList<string> Actors { get; init; } = [];

    /// <summary>
    /// Gets the release year of the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("releaseYears")]
    public int? ReleaseYears { get; init; }

    /// <summary>
    /// Gets the media type descriptor (e.g. "movie", "tv"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("mediaType")]
    public string? MediaType { get; init; }

    /// <summary>
    /// Gets the characters featured in the media item.
    /// </summary>
    [JsonPropertyName("characters")]
    public IReadOnlyList<string> Characters { get; init; } = [];

    /// <summary>
    /// Gets the external link for the media item (e.g. IMDb page),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>
    /// Gets a brief description or synopsis of the media item,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the rating of the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("rating")]
    public double? Rating { get; init; }

    /// <summary>
    /// Gets the languages available for the media item.
    /// </summary>
    [JsonPropertyName("languages")]
    public IReadOnlyList<string> Languages { get; init; } = [];

    /// <summary>
    /// Gets the content rating (e.g. "PG", "R"), or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("contentRating")]
    public string? ContentRating { get; init; }

    /// <summary>
    /// Gets trailer information for the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("trailer")]
    public MetaTrailer? Trailer { get; init; }

    /// <summary>
    /// Gets the URL to the poster/cover image, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; init; }
}
