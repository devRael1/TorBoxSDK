using System.Net;
using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents a TorBox API response with a typed data payload.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public sealed record TorBoxResponse<T>
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
	/// Gets the deserialized data, or <see langword="null"/> when the response has no data.
	/// </summary>
	[JsonPropertyName("data")]
	public T? Data { get; init; }

	/// <summary>
	/// Gets the HTTP status associated with the response.
	/// </summary>
	[JsonIgnore]
	public HttpStatusCode StatusCode { get; init; }
}
