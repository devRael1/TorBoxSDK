using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents the response from a device code authorization request.
/// </summary>
public sealed record DeviceCodeResponse
{
	/// <summary>
	/// Gets the device code used to poll for authorization,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("device_code")]
	public string? DeviceCode { get; init; }

	/// <summary>
	/// Gets the user-facing code to enter on the verification page,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("code")]
	public string? Code { get; init; }

	/// <summary>
	/// Gets the URL where the user should enter the code,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("verification_url")]
	public string? VerificationUrl { get; init; }

	/// <summary>
	/// Gets the user-friendly short verification URL,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("friendly_verification_url")]
	public string? FriendlyVerificationUrl { get; init; }

	/// <summary>
	/// Gets the date and time when the device code expires,
	/// or <see langword="null"/> if not available.
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTimeOffset? ExpiresAt { get; init; }

	/// <summary>
	/// Gets the polling interval in seconds for checking authorization status.
	/// </summary>
	[JsonPropertyName("interval")]
	public int Interval { get; init; }
}
