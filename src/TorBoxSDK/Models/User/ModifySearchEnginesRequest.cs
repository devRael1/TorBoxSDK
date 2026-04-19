using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to modify an existing search engine in the user's settings.
/// </summary>
public sealed record ModifySearchEnginesRequest
{
	/// <summary>
	/// Gets the identifier of the search engine to modify.
	/// </summary>
	[JsonPropertyName("id")]
	public long Id { get; init; }

	/// <summary>
	/// Gets the type of search engine, or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("type")]
	public string? Type { get; init; }

	/// <summary>
	/// Gets the URL of the search engine, or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; init; }

	/// <summary>
	/// Gets the API key for the search engine, or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("apikey")]
	public string? Apikey { get; init; }

	/// <summary>
	/// Gets the download type for the search engine, or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("download_type")]
	public string? DownloadType { get; init; }
}
