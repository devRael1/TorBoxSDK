using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to add a referral code to the user's account.
/// </summary>
public sealed record AddReferralRequest
{
    /// <summary>
    /// Gets the referral code to apply.
    /// </summary>
    [JsonPropertyName("referral_code")]
    public string ReferralCode { get; init; } = string.Empty;
}
