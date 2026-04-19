using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Rss;

/// <summary>
/// Represents a request to add a new RSS feed to TorBox.
/// </summary>
public sealed record AddRssRequest
{
	/// <summary>
	/// Gets the URL of the RSS feed to add.
	/// </summary>
	[JsonPropertyName("url")]
	public string Url { get; init; } = string.Empty;

	/// <summary>
	/// Gets an optional display name for the RSS feed,
	/// or <see langword="null"/> to use the default name from the feed.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets the regex pattern to filter included items,
	/// or <see langword="null"/> for no include filter.
	/// </summary>
	[JsonPropertyName("do_regex")]
	public string? RegexFilter { get; init; }

	/// <summary>
	/// Gets the regex pattern to exclude items,
	/// or <see langword="null"/> for no exclude filter.
	/// </summary>
	[JsonPropertyName("dont_regex")]
	public string? RegexFilterExclude { get; init; }

	/// <summary>
	/// Gets the scan interval in minutes for checking the feed,
	/// or <see langword="null"/> to use the default interval.
	/// </summary>
	[JsonPropertyName("scan_interval")]
	public int? ScanInterval { get; init; }

	/// <summary>
	/// Gets the type of the RSS feed (e.g., torrent, usenet),
	/// or <see langword="null"/> to auto-detect.
	/// </summary>
	[JsonPropertyName("rss_type")]
	public string? RssType { get; init; }

	/// <summary>
	/// Gets the maximum age in days for included items,
	/// or <see langword="null"/> for no age restriction.
	/// </summary>
	[JsonPropertyName("dont_older_than")]
	public int? DontOlderThan { get; init; }

	/// <summary>
	/// Gets a value indicating whether to pass the check,
	/// or <see langword="null"/> to use the default behavior.
	/// </summary>
	[JsonPropertyName("pass_check")]
	public bool? PassCheck { get; init; }

	/// <summary>
	/// Gets the torrent seeding preference,
	/// or <see langword="null"/> to use the default seeding behavior.
	/// </summary>
	[JsonPropertyName("torrent_seeding")]
	public int? TorrentSeeding { get; init; }
}
