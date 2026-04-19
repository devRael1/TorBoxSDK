using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.Integrations;

/// <summary>
/// Represents the response from an OAuth callback endpoint.
/// </summary>
public sealed record OAuthCallback
{
	/// <summary>
	/// Gets the callback status or result, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("status")]
	public string? Status { get; init; }

	/// <summary>
	/// Gets callback-specific data, or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("data")]
	public object? Data { get; init; }
}
