using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Torrents;

/// <summary>
/// Represents metadata information about a torrent obtained from its hash or magnet link.
/// </summary>
public sealed record TorrentInfo
{
	/// <summary>
	/// Gets the info hash of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("hash")]
	public string? Hash { get; init; }

	/// <summary>
	/// Gets the name of the torrent.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; init; } = string.Empty;

	/// <summary>
	/// Gets the total size of the torrent in bytes.
	/// </summary>
	[JsonPropertyName("size")]
	public long Size { get; init; }

	/// <summary>
	/// Gets the number of peers available for this torrent.
	/// </summary>
	[JsonPropertyName("peers")]
	public int Peers { get; init; }

	/// <summary>
	/// Gets the number of seeds available for this torrent.
	/// </summary>
	[JsonPropertyName("seeds")]
	public int Seeds { get; init; }

	/// <summary>
	/// Gets the list of files contained in this torrent.
	/// </summary>
	[JsonPropertyName("files")]
	public IReadOnlyList<TorrentFile> Files { get; init; } = [];

	/// <summary>
	/// Gets the list of tracker URLs associated with this torrent.
	/// </summary>
	[JsonPropertyName("trackers")]
	public IReadOnlyList<string> Trackers { get; init; } = [];
}
