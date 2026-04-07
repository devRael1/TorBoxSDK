using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents referral data for the authenticated user's account.
/// </summary>
public sealed record ReferralData
{
    /// <summary>
    /// Gets the number of accounts referred by the user.
    /// </summary>
    [JsonPropertyName("referred_accounts")]
    public int ReferredAccounts { get; init; }

    /// <summary>
    /// Gets the user's referral code, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("referral_code")]
    public string? ReferralCode { get; init; }

    /// <summary>
    /// Gets the number of purchases made by referred accounts.
    /// </summary>
    [JsonPropertyName("purchases_referred")]
    public int PurchasesReferred { get; init; }
}
