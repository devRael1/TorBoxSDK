using System.Net;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents a TorBox API response without a data payload.
/// </summary>
public sealed record TorBoxResponse
{
	/// <summary>
	/// Gets a value indicating whether the API reports success.
	/// </summary>
	[JsonPropertyName("success")]
	public bool Success { get; init; }

	/// <summary>
	/// Gets the TorBox error identifier, or <see langword="null"/> when none was returned.
	/// </summary>
	[JsonPropertyName("error")]
	public string? Error { get; init; }

	/// <summary>
	/// Gets the response detail, or <see langword="null"/> when none was returned.
	/// </summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>
	/// Gets the HTTP status associated with the response.
	/// </summary>
	[JsonIgnore]
	public HttpStatusCode StatusCode { get; init; }
}
