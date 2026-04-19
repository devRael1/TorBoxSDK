using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

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
	[JsonPropertyName("language")]
	public string? Language { get; init; }

	/// <summary>
	/// Gets the full subtitle language name, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("language_full")]
	public string? LanguageFull { get; init; }

	/// <summary>
	/// Gets the subtitle track title, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; init; }
}
