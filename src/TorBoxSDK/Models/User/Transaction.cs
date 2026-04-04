using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a financial transaction on a TorBox user account.
/// </summary>
public sealed record Transaction
{
    /// <summary>
    /// Gets the unique identifier of the transaction.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public double Amount { get; init; }

    /// <summary>
    /// Gets the currency code for the transaction amount,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the description of the transaction,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the current status of the transaction,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the date and time when the transaction was created,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
