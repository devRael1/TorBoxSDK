using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Stream;

/// <summary>
/// Represents the stream data response for direct playback.
/// </summary>
public sealed record StreamData
{
	/// <summary>
	/// Gets the HLS playback URL, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("hls_url")]
	public string? HlsUrl { get; init; }

	/// <summary>
	/// Gets the stream domain, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("domain")]
	public string? Domain { get; init; }

	/// <summary>
	/// Gets the presigned token for stream access, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("presigned_token")]
	public string? PresignedToken { get; init; }

	/// <summary>
	/// Gets the selected subtitle track index, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("subtitle_index")]
	public int? SubtitleIndex { get; init; }

	/// <summary>
	/// Gets the selected audio track index, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("audio_index")]
	public int? AudioIndex { get; init; }

	/// <summary>
	/// Gets the selected resolution index, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("resolution_index")]
	public int? ResolutionIndex { get; init; }

	/// <summary>
	/// Gets the file token, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("file_token")]
	public string? FileToken { get; init; }

	/// <summary>
	/// Gets the stream token, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("token")]
	public string? Token { get; init; }

	/// <summary>
	/// Gets a value indicating whether transcoding is currently active.
	/// </summary>
	[JsonPropertyName("is_transcoding")]
	public bool? IsTranscoding { get; init; }

	/// <summary>
	/// Gets a value indicating whether transcoding is required.
	/// </summary>
	[JsonPropertyName("needs_transcoding")]
	public bool? NeedsTranscoding { get; init; }

	/// <summary>
	/// Gets nested stream metadata, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("metadata")]
	public StreamMetadata? Metadata { get; init; }

	/// <summary>
	/// Gets search metadata payload, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("search_metadata")]
	public object? SearchMetadata { get; init; }
}
