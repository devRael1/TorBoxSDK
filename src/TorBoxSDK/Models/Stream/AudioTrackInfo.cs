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
	[JsonPropertyName("language")]
	public string? Language { get; init; }

	/// <summary>
	/// Gets the full language name, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("language_full")]
	public string? LanguageFull { get; init; }

	/// <summary>
	/// Gets the audio codec name, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("codec")]
	public string? Codec { get; init; }

	/// <summary>
	/// Gets the codec type, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("codec_type")]
	public string? CodecType { get; init; }

	/// <summary>
	/// Gets a value indicating whether this audio track is the default track.
	/// </summary>
	[JsonPropertyName("default")]
	public bool? Default { get; init; }

	/// <summary>
	/// Gets the audio sample rate, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("sample_rate")]
	public string? SampleRate { get; init; }

	/// <summary>
	/// Gets the number of channels, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("channels")]
	public int? Channels { get; init; }

	/// <summary>
	/// Gets the channel layout, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("channel_layout")]
	public string? ChannelLayout { get; init; }

	/// <summary>
	/// Gets the track title, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; init; }
}
