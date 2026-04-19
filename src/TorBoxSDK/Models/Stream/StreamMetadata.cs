using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Represents nested stream metadata from the stream data response.
/// </summary>
public sealed record StreamMetadata
{
    /// <summary>
    /// Gets raw video metadata, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("video")]
    public object? Video { get; init; }

    /// <summary>
    /// Gets available audio tracks metadata.
    /// </summary>
    [JsonPropertyName("audios")]
    public IReadOnlyList<AudioTrackInfo> Audios { get; init; } = [];

    /// <summary>
    /// Gets available subtitle tracks metadata.
    /// </summary>
    [JsonPropertyName("subtitles")]
    public IReadOnlyList<SubtitleTrackInfo> Subtitles { get; init; } = [];

    /// <summary>
    /// Gets the thumbnail filename or URL, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; init; }

    /// <summary>
    /// Gets the chapters track filename, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("chapters")]
    public string? Chapters { get; init; }
}
