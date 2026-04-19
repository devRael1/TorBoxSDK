using TorBoxSDK.Http;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents the query parameters for checking whether hashes are cached (GET endpoint).
/// Shared by torrents, usenet, and web download cache check endpoints.
/// </summary>
/// <remarks>
/// These options are sent as query string parameters, not as a JSON body.
/// </remarks>
public sealed record CheckCachedOptions
{
	/// <summary>
	/// Gets the optional response format,
	/// or <see langword="null"/> to use the default format.
	/// </summary>
	[QueryParameterName("format")]
	public string? Format { get; init; }

	/// <summary>
	/// Gets a value indicating whether to include file listings in the response,
	/// or <see langword="null"/> to use the default behavior.
	/// </summary>
	[QueryParameterName("list_files")]
	public bool? ListFiles { get; init; }
}
