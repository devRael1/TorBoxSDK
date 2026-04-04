using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a TorBox user account returned by the API.
/// </summary>
public sealed record UserProfile
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the email address of the user, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the numeric identifier of the user's current plan.
    /// </summary>
    [JsonPropertyName("plan")]
    public int Plan { get; init; }

    /// <summary>
    /// Gets the total number of bytes downloaded by the user.
    /// </summary>
    [JsonPropertyName("total_downloaded")]
    public long TotalDownloaded { get; init; }

    /// <summary>
    /// Gets the date and time when the user account was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the user account was last updated,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user has an active subscription.
    /// </summary>
    [JsonPropertyName("is_subscribed")]
    public bool IsSubscribed { get; init; }

    /// <summary>
    /// Gets the date and time when the user's premium subscription expires,
    /// or <see langword="null"/> if not subscribed.
    /// </summary>
    [JsonPropertyName("premium_expires_at")]
    public DateTimeOffset? PremiumExpiresAt { get; init; }

    /// <summary>
    /// Gets the date and time until which the user is on cooldown,
    /// or <see langword="null"/> if not on cooldown.
    /// </summary>
    [JsonPropertyName("cooldown_until")]
    public DateTimeOffset? CooldownUntil { get; init; }

    /// <summary>
    /// Gets the authentication identifier of the user,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string? AuthId { get; init; }

    /// <summary>
    /// Gets the user's referral code, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("user_referral_code")]
    public string? UserReferralCode { get; init; }

    /// <summary>
    /// Gets the base email address of the user (without aliases),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("base_email")]
    public string? BaseEmail { get; init; }

    /// <summary>
    /// Gets the user's account settings, or <see langword="null"/> if not included in the response.
    /// </summary>
    [JsonPropertyName("settings")]
    public UserSettings? Settings { get; init; }
}
