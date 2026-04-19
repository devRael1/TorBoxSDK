using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to add a search engine to the user's settings.
/// </summary>
public sealed record AddSearchEnginesRequest
{
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
