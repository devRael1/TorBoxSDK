using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a user subscription plan on TorBox.
/// </summary>
public sealed record Subscription
{
    /// <summary>
    /// Gets the unique identifier of the subscription.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the name of the subscription plan,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; init; }

    /// <summary>
    /// Gets the subscription amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public double Amount { get; init; }

    /// <summary>
    /// Gets the currency code for the subscription amount,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the current status of the subscription,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the date and time when the subscription was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the subscription expires,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }
}
