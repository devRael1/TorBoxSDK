using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Search;

/// <summary>
/// Represents a single usenet search result returned by the TorBox Search API.
/// </summary>
public sealed record UsenetSearchResult
{
	/// <summary>
	/// Gets the unique identifier of the usenet article, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; init; }

	/// <summary>
	/// Gets the display name of the usenet article, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets the total size of the usenet article in bytes.
	/// </summary>
	[JsonPropertyName("size")]
	public long Size { get; init; }

	/// <summary>
	/// Gets the source indexer that provided this result, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("source")]
	public string? Source { get; init; }

	/// <summary>
	/// Gets the category of the usenet article, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("category")]
	public string? Category { get; init; }

	/// <summary>
	/// Gets the direct NZB download link, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("nzb_link")]
	public string? NzbLink { get; init; }

	/// <summary>
	/// Gets the age of the article in days, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("age")]
	public int? Age { get; init; }

	/// <summary>
	/// Gets the date and time when this article was posted,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("posted_at")]
	public DateTimeOffset? PostedAt { get; init; }
}
