using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Options for filtering and customizing usenet search requests.
/// </summary>
public sealed record UsenetSearchOptions
{
    /// <summary>
    /// Gets a value indicating whether to include metadata in the search results,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("metadata")]
    public bool? Metadata { get; init; }

    /// <summary>
    /// Gets the season number to filter results by,
    /// or <see langword="null"/> to include all seasons.
    /// </summary>
    [JsonPropertyName("season")]
    public int? Season { get; init; }

    /// <summary>
    /// Gets the episode number to filter results by,
    /// or <see langword="null"/> to include all episodes.
    /// </summary>
    [JsonPropertyName("episode")]
    public int? Episode { get; init; }

    /// <summary>
    /// Gets a value indicating whether to check if the results are cached,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("check_cache")]
    public bool? CheckCache { get; init; }

    /// <summary>
    /// Gets a value indicating whether to check if the results are already owned,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("check_owned")]
    public bool? CheckOwned { get; init; }

    /// <summary>
    /// Gets a value indicating whether to use custom user search engines,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("search_user_engines")]
    public bool? SearchUserEngines { get; init; }

    /// <summary>
    /// Gets a value indicating whether to only return cached results,
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    [JsonPropertyName("cached_only")]
    public bool? CachedOnly { get; init; }
}
