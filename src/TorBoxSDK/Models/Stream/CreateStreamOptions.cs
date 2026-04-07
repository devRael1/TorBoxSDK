using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Represents the query parameters for creating a stream.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record CreateStreamOptions
{
    /// <summary>
    /// Gets the identifier of the download.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the identifier of the file within the download.
    /// </summary>
    [JsonPropertyName("file_id")]
    public long FileId { get; init; }

    /// <summary>
    /// Gets the type of download (e.g., torrent, usenet).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional index of the chosen subtitle track,
    /// or <see langword="null"/> to use the default.
    /// </summary>
    [JsonPropertyName("chosen_subtitle_index")]
    public int? ChosenSubtitleIndex { get; init; }

    /// <summary>
    /// Gets the optional index of the chosen audio track,
    /// or <see langword="null"/> to use the default.
    /// </summary>
    [JsonPropertyName("chosen_audio_index")]
    public int? ChosenAudioIndex { get; init; }

    /// <summary>
    /// Gets the optional index of the chosen resolution,
    /// or <see langword="null"/> to use the default.
    /// </summary>
    [JsonPropertyName("chosen_resolution_index")]
    public int? ChosenResolutionIndex { get; init; }
}
