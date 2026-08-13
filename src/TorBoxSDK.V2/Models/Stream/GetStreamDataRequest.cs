namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Describes the tokens and optional track selections used to retrieve stream data.
/// </summary>
public sealed record GetStreamDataRequest
{
	/// <summary>
	/// Gets the presigned stream token.
	/// </summary>
	public string PresignedToken { get; init; } = string.Empty;

	/// <summary>
	/// Gets the stream token.
	/// </summary>
	public string Token { get; init; } = string.Empty;

	/// <summary>
	/// Gets the optional selected subtitle track index.
	/// </summary>
	public int? ChosenSubtitleIndex { get; init; }

	/// <summary>
	/// Gets the optional selected audio track index.
	/// </summary>
	public int? ChosenAudioIndex { get; init; }

	/// <summary>
	/// Gets the optional selected resolution index.
	/// </summary>
	public int? ChosenResolutionIndex { get; init; }
}
