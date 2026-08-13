namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Describes the query parameters used to create a stream.
/// </summary>
public sealed record CreateStreamRequest
{
	/// <summary>
	/// Gets the identifier of the download to stream.
	/// </summary>
	public long DownloadId { get; init; }

	/// <summary>
	/// Gets the optional identifier of the file within the download.
	/// </summary>
	public long? FileId { get; init; }

	/// <summary>
	/// Gets the optional stream type.
	/// </summary>
	public string? Type { get; init; }

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

	/// <summary>
	/// Gets whether TorBox should enable scrobbling, or <see langword="null"/> to use its default.
	/// </summary>
	public bool? ScrobblingEnabled { get; init; }
}
