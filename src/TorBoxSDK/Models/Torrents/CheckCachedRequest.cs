using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to check whether one or more torrents are cached on TorBox.
/// </summary>
public sealed record CheckCachedRequest
{
	/// <summary>
	/// Gets the list of torrent info hashes to check for cache availability.
	/// </summary>
	[JsonPropertyName("hashes")]
	public IReadOnlyList<string> Hashes { get; init; } = [];

	/// <summary>
	/// Gets the desired response format, or <see langword="null"/> for the default format.
	/// </summary>
	[JsonPropertyName("format")]
	public string? Format { get; init; }

	/// <summary>
	/// Gets a value indicating whether to include file listings in the response,
	/// or <see langword="null"/> to use the default behavior.
	/// </summary>
	[JsonPropertyName("list_files")]
	public bool? ListFiles { get; init; }
}
