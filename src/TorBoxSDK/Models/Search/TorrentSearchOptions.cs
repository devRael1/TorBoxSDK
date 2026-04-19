using TorBoxSDK.Http;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Options for filtering and customizing torrent search requests.
/// </summary>
public sealed record TorrentSearchOptions
{
	/// <summary>
	/// Gets a value indicating whether to include metadata in the search results,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("metadata")]
	public bool? Metadata { get; init; }

	/// <summary>
	/// Gets the season number to filter results by,
	/// or <see langword="null"/> to include all seasons.
	/// </summary>
	[QueryParameterName("season")]
	public int? Season { get; init; }

	/// <summary>
	/// Gets the episode number to filter results by,
	/// or <see langword="null"/> to include all episodes.
	/// </summary>
	[QueryParameterName("episode")]
	public int? Episode { get; init; }

	/// <summary>
	/// Gets a value indicating whether to check if the results are cached,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("check_cache")]
	public bool? CheckCache { get; init; }

	/// <summary>
	/// Gets a value indicating whether to check if the results are already owned,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("check_owned")]
	public bool? CheckOwned { get; init; }

	/// <summary>
	/// Gets a value indicating whether to use custom user search engines,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("search_user_engines")]
	public bool? SearchUserEngines { get; init; }

	/// <summary>
	/// Gets a value indicating whether to only return cached results,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("cached_only")]
	public bool? CachedOnly { get; init; }
}
