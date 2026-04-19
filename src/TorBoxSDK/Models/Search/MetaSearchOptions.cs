using TorBoxSDK.Http;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Options for filtering and customizing metadata search requests.
/// </summary>
public sealed record MetaSearchOptions
{
	/// <summary>
	/// Gets the content type to filter metadata results by,
	/// or <see langword="null"/> to include all content types.
	/// </summary>
	[QueryParameterName("type")]
	public string? Type { get; init; }
}
