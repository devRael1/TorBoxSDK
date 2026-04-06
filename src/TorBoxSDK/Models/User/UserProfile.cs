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
    /// Gets the authentication identifier of the user,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("auth_id")]
    public string? AuthId { get; init; }

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
    /// Gets the numeric identifier of the user's current plan.
    /// </summary>
    [JsonPropertyName("plan")]
    public int Plan { get; init; }

    /// <summary>
    /// Gets the total bytes downloaded by the user.
    /// </summary>
    [JsonPropertyName("total_downloaded")]
    public long TotalDownloaded { get; init; }

    /// <summary>
    /// Gets the customer identifier from the payment provider,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("customer")]
    public string? Customer { get; init; }

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
    /// Gets the email address of the user, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the user's referral identifier, or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("user_referral")]
    public string? UserReferral { get; init; }

    /// <summary>
    /// Gets the base email address of the user (without aliases),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("base_email")]
    public string? BaseEmail { get; init; }

    /// <summary>
    /// Gets the total number of bytes downloaded by the user.
    /// </summary>
    [JsonPropertyName("total_bytes_downloaded")]
    public long TotalBytesDownloaded { get; init; }

    /// <summary>
    /// Gets the total number of bytes uploaded by the user.
    /// </summary>
    [JsonPropertyName("total_bytes_uploaded")]
    public long TotalBytesUploaded { get; init; }

    /// <summary>
    /// Gets the number of torrents downloaded by the user.
    /// </summary>
    [JsonPropertyName("torrents_downloaded")]
    public long TorrentsDownloaded { get; init; }

    /// <summary>
    /// Gets the number of web downloads completed by the user.
    /// </summary>
    [JsonPropertyName("web_downloads_downloaded")]
    public long WebDownloadsDownloaded { get; init; }

    /// <summary>
    /// Gets the number of usenet downloads completed by the user.
    /// </summary>
    [JsonPropertyName("usenet_downloads_downloaded")]
    public long UsenetDownloadsDownloaded { get; init; }

    /// <summary>
    /// Gets the number of additional concurrent download slots.
    /// </summary>
    [JsonPropertyName("additional_concurrent_slots")]
    public int AdditionalConcurrentSlots { get; init; }

    /// <summary>
    /// Gets a value indicating whether long-term seeding is enabled.
    /// </summary>
    [JsonPropertyName("long_term_seeding")]
    public bool LongTermSeeding { get; init; }

    /// <summary>
    /// Gets a value indicating whether long-term storage is enabled.
    /// </summary>
    [JsonPropertyName("long_term_storage")]
    public bool LongTermStorage { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user is a vendor.
    /// </summary>
    [JsonPropertyName("is_vendor")]
    public bool IsVendor { get; init; }

    /// <summary>
    /// Gets the vendor identifier, or <see langword="null"/> if the user is not a vendor.
    /// </summary>
    [JsonPropertyName("vendor_id")]
    public string? VendorId { get; init; }

    /// <summary>
    /// Gets the number of purchases referred by the user.
    /// </summary>
    [JsonPropertyName("purchases_referred")]
    public int PurchasesReferred { get; init; }

    /// <summary>
    /// Gets the user's account settings, or <see langword="null"/> if not included in the response.
    /// </summary>
    [JsonPropertyName("settings")]
    public UserSettings? Settings { get; init; }
}
