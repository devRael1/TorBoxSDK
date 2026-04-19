using TorBoxSDK.Http;

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
	/// Gets the optional index of the chosen subtitle track,
	/// or <see langword="null"/> to use the default.
	/// </summary>
	[QueryParameterName("chosen_subtitle_index")]
	public int? ChosenSubtitleIndex { get; init; }

	/// <summary>
	/// Gets the optional index of the chosen audio track,
	/// or <see langword="null"/> to use the default.
	/// </summary>
	[QueryParameterName("chosen_audio_index")]
	public int? ChosenAudioIndex { get; init; }

	/// <summary>
	/// Gets the optional index of the chosen resolution,
	/// or <see langword="null"/> to use the default.
	/// </summary>
	[QueryParameterName("chosen_resolution_index")]
	public int? ChosenResolutionIndex { get; init; }
}
