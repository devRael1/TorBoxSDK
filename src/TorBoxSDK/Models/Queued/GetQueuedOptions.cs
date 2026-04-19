using TorBoxSDK.Http;

namespace TorBoxSDK.Models.Queued;

/// <summary>
/// Represents the query parameters for retrieving queued downloads.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record GetQueuedOptions
{
	/// <summary>
	/// Gets the optional queued download ID to retrieve a single item,
	/// or <see langword="null"/> to retrieve the full list.
	/// </summary>
	[QueryParameterName("id")]
	public long? Id { get; init; }

	/// <summary>
	/// Gets the optional offset for pagination,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("offset")]
	public int? Offset { get; init; }

	/// <summary>
	/// Gets the optional limit for pagination,
	/// or <see langword="null"/> to use the server default.
	/// </summary>
	[QueryParameterName("limit")]
	public int? Limit { get; init; }

	/// <summary>
	/// Gets a value indicating whether to bypass the cache,
	/// or <see langword="null"/> to use the default caching behavior.
	/// </summary>
	[QueryParameterName("bypass_cache")]
	public bool? BypassCache { get; init; }

	/// <summary>
	/// Gets the optional type filter for queued downloads,
	/// or <see langword="null"/> to include all types.
	/// </summary>
	[QueryParameterName("type")]
	public string? Type { get; init; }
}
