using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to perform a control operation on the user's search engine settings.
/// </summary>
public sealed record ControlSearchEnginesRequest
{
	/// <summary>
	/// Gets the operation to perform on the search engines.
	/// </summary>
	[JsonPropertyName("operation")]
	public string Operation { get; init; } = string.Empty;

	/// <summary>
	/// Gets the identifier of the search engine to operate on,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("id")]
	public long? Id { get; init; }

	/// <summary>
	/// Gets a value indicating whether the operation should apply to all search engines,
	/// or <see langword="null"/> if not specified.
	/// </summary>
	[JsonPropertyName("all")]
	public bool? All { get; init; }
}
