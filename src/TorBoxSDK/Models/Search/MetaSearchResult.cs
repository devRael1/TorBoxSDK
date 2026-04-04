using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents a single metadata search result returned by the TorBox Search API.
/// </summary>
public sealed record MetaSearchResult
{
    /// <summary>
    /// Gets the unique identifier of the metadata entry, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the display name of the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the media type descriptor (e.g. "movie", "tv"),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the release year of the media item, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; init; }

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
    /// Gets the URL to the poster image, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("poster")]
    public string? Poster { get; init; }

    /// <summary>
    /// Gets a brief overview or synopsis of the media item,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("overview")]
    public string? Overview { get; init; }
}
