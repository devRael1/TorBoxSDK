using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Represents audio track metadata from a stream.
/// </summary>
public sealed record AudioTrackInfo
{
    /// <summary>
    /// Gets the audio track index.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>
    /// Gets the audio language code, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>
    /// Gets additional track information, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("extra")]
    public object? Extra { get; init; }
}

/// <summary>
/// Represents subtitle track metadata from a stream.
/// </summary>
public sealed record SubtitleTrackInfo
{
    /// <summary>
    /// Gets the subtitle track index.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>
    /// Gets the subtitle language code, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>
    /// Gets the subtitle URL, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets additional subtitle information, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("extra")]
    public object? Extra { get; init; }
}

/// <summary>
/// Represents stream resolution option metadata.
/// </summary>
public sealed record ResolutionInfo
{
    /// <summary>
    /// Gets the resolution index.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>
    /// Gets the resolution label (e.g., "1080p"), or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; init; }

    /// <summary>
    /// Gets additional resolution information, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("extra")]
    public object? Extra { get; init; }
}

/// <summary>
/// Represents the stream data response for direct playback.
/// </summary>
public sealed record StreamData
{
    /// <summary>
    /// Gets the streaming URL for playback.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the list of available subtitle tracks.
    /// </summary>
    [JsonPropertyName("subtitles")]
    public IReadOnlyList<SubtitleTrackInfo> Subtitles { get; init; } = [];

    /// <summary>
    /// Gets the list of available audio tracks.
    /// </summary>
    [JsonPropertyName("audio_tracks")]
    public IReadOnlyList<AudioTrackInfo> AudioTracks { get; init; } = [];

    /// <summary>
    /// Gets the list of available resolutions.
    /// </summary>
    [JsonPropertyName("resolutions")]
    public IReadOnlyList<ResolutionInfo> Resolutions { get; init; } = [];
}
