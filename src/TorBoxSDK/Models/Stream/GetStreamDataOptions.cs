using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Represents the query parameters for getting stream data.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record GetStreamDataOptions
{
    /// <summary>
    /// Gets the presigned token for stream authentication.
    /// </summary>
    [JsonPropertyName("presigned_token")]
    public string PresignedToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the authentication token.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

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
