using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents a request to convert a magnet link to a torrent file.
/// </summary>
public sealed record MagnetToFileRequest
{
	/// <summary>
	/// Gets the magnet link to convert.
	/// </summary>
	[JsonPropertyName("magnet")]
	public string Magnet { get; init; } = string.Empty;
}
