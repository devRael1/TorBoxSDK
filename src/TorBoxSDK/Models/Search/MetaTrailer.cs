using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents trailer information for a metadata search result.
/// </summary>
public sealed record MetaTrailer
{
    /// <summary>
    /// Gets the YouTube video identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("youtube_id")]
    public string? YoutubeId { get; init; }

    /// <summary>
    /// Gets the full YouTube URL, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("full_url")]
    public string? FullUrl { get; init; }

    /// <summary>
    /// Gets the thumbnail image URL, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; init; }
}
