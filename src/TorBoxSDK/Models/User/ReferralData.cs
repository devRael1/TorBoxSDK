using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents referral data for the authenticated user's account.
/// </summary>
public sealed record ReferralData
{
    /// <summary>
    /// Gets the total number of referrals made by the user.
    /// </summary>
    [JsonPropertyName("total_referrals")]
    public int TotalReferrals { get; init; }

    /// <summary>
    /// Gets the user's referral code, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("referral_code")]
    public string? ReferralCode { get; init; }
}
