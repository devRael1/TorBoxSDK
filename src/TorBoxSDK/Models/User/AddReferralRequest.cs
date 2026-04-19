using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to apply a referral code to the authenticated user account.
/// </summary>
/// <remarks>
/// The referral code is currently sent as a query string parameter by the API client.
/// </remarks>
public sealed record AddReferralRequest
{
	/// <summary>
	/// Gets the referral code to apply.
	/// </summary>
	[JsonPropertyName("referral")]
	public string Referral { get; init; } = string.Empty;
}
