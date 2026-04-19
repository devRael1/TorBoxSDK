using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents a single torrent search result returned by the TorBox Search API.
/// </summary>
public sealed record TorrentSearchResult
{
	/// <summary>
	/// Gets the info hash of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("hash")]
	public string? Hash { get; init; }

	/// <summary>
	/// Gets the display name of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets the total size of the torrent in bytes.
	/// </summary>
	[JsonPropertyName("size")]
	public long Size { get; init; }

	/// <summary>
	/// Gets the number of seeders currently available for this torrent.
	/// </summary>
	[JsonPropertyName("seeders")]
	public int Seeders { get; init; }

	/// <summary>
	/// Gets the number of leechers currently connected to this torrent.
	/// </summary>
	[JsonPropertyName("leechers")]
	public int Leechers { get; init; }

	/// <summary>
	/// Gets the source indexer that provided this result, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>
	/// Gets the category of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("category")]
	public string? Category { get; init; }

	/// <summary>
	/// Gets the magnet URI for the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("magnet")]
	public string? Magnet { get; init; }

	/// <summary>
	/// Gets the last known number of seeders, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("last_known_seeders")]
	public int? LastKnownSeeders { get; init; }

	/// <summary>
	/// Gets the last known number of leechers, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("last_known_leechers")]
	public int? LastKnownLeechers { get; init; }

	/// <summary>
	/// Gets the date and time when this result was last updated,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("updated_at")]
	public DateTimeOffset? UpdatedAt { get; init; }

	/// <summary>
	/// Gets whether the torrent is already owned by the user, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("owned")]
	public bool? Owned { get; init; }

	/// <summary>
	/// Gets whether the torrent is cached, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("cached")]
	public bool? Cached { get; init; }

	/// <summary>
	/// Gets the website URL of the source indexer, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("website")]
	public string? Website { get; init; }

	/// <summary>
	/// Gets the unique identifier of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; init; }

	/// <summary>
	/// Gets the quality tag of the torrent (e.g. "1080p"), or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("quality")]
	public string? Quality { get; init; }

	/// <summary>
	/// Gets the raw unparsed title of the torrent, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("raw_title")]
	public string? RawTitle { get; init; }
}
