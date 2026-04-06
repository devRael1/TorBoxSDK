using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents the wrapper response for a torrent search containing metadata and results.
/// </summary>
public sealed record TorrentSearchResponse
{
    /// <summary>
    /// Gets the metadata about the search result, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("metadata")]
    public MetaSearchResult? Metadata { get; init; }

    /// <summary>
    /// Gets the list of torrent search results.
    /// </summary>
    [JsonPropertyName("torrents")]
    public IReadOnlyList<TorrentSearchResult> Torrents { get; init; } = [];
}
