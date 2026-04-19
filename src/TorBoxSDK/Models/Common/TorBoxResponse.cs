using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Common;

/// <summary>
/// Represents a standard TorBox API response without a typed data payload.
/// </summary>
/// <remarks>
/// All TorBox API responses share a common envelope with <see cref="Success"/>,
/// <see cref="Error"/>, and <see cref="Detail"/> fields. Use <see cref="TorBoxResponse{T}"/>
/// when the response includes a typed <c>data</c> payload.
/// </remarks>
public sealed record TorBoxResponse
{
	/// <summary>
	/// Gets a value indicating whether the API request was successful.
	/// </summary>
	[JsonPropertyName("success")]
	public bool Success { get; init; }

	/// <summary>
	/// Gets the error message returned by the API, or <see langword="null"/> if no error occurred.
	/// </summary>
	[JsonPropertyName("error")]
	public string? Error { get; init; }

	/// <summary>
	/// Gets additional detail about the response or error, or <see langword="null"/> if none was provided.
	/// </summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }
}

/// <summary>
/// Represents a standard TorBox API response with a typed data payload.
/// </summary>
/// <typeparam name="T">The type of the data payload.</typeparam>
/// <remarks>
/// All TorBox API responses share a common envelope with <see cref="Success"/>,
/// <see cref="Error"/>, and <see cref="Detail"/> fields. The <see cref="Data"/>
/// property contains the deserialized payload of type <typeparamref name="T"/>.
/// </remarks>
public sealed record TorBoxResponse<T>
{
	/// <summary>
	/// Gets a value indicating whether the API request was successful.
	/// </summary>
	[JsonPropertyName("success")]
	public bool Success { get; init; }

	/// <summary>
	/// Gets the error message returned by the API, or <see langword="null"/> if no error occurred.
	/// </summary>
	[JsonPropertyName("error")]
	public string? Error { get; init; }

	/// <summary>
	/// Gets additional detail about the response or error, or <see langword="null"/> if none was provided.
	/// </summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>
	/// Gets the deserialized data payload, or <see langword="null"/> if the response contained no data.
	/// </summary>
	[JsonPropertyName("data")]
	public T? Data { get; init; }
}
