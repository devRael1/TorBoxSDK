using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents the wrapper response for a usenet search containing metadata and results.
/// </summary>
public sealed record UsenetSearchResponse
{
	/// <summary>
	/// Gets the metadata about the search result, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("metadata")]
	public MetaSearchResult? Metadata { get; init; }

	/// <summary>
	/// Gets the list of usenet search results.
	/// </summary>
	[JsonPropertyName("nzbs")]
	public IReadOnlyList<UsenetSearchResult> Nzbs { get; init; } = [];
}
