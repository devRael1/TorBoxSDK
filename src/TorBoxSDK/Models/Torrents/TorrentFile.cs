using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents basic file information from a torrent's metadata.
/// </summary>
public sealed record TorrentFile
{
	/// <summary>
	/// Gets the name of the file within the torrent.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; init; } = string.Empty;

	/// <summary>
	/// Gets the size of the file in bytes.
	/// </summary>
	[JsonPropertyName("size")]
	public long Size { get; init; }
}
