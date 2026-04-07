using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a financial transaction on a TorBox user account.
/// </summary>
public sealed record Transaction
{
    /// <summary>
    /// Gets the date and time when the transaction occurred,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("at")]
    public DateTimeOffset? At { get; init; }

    /// <summary>
    /// Gets the payment provider type (e.g. sellix, nowpayments),
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public double Amount { get; init; }

    /// <summary>
    /// Gets the external reference ID for the transaction,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; init; }
}
