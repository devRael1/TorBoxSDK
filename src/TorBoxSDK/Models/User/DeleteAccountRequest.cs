using System.Text.Json.Serialization;

namespace TorBoxSDK.Models.User;

/// <summary>
/// Represents a request to delete the authenticated user's account.
/// </summary>
public sealed record DeleteAccountRequest
{
    /// <summary>
    /// Gets the confirmation string required to delete the account,
    /// or <see langword="null"/> if not required.
    /// </summary>
    [JsonPropertyName("confirmation")]
    public string? Confirmation { get; init; }
}
